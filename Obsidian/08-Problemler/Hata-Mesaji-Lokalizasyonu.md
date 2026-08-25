# Hata mesajları kullanıcıya ham CLR metni olarak görünüyordu

Tarih: 25 Ağustos 2026, Salı (Gün 17 sonrası — yönetici geri bildirimi üzerine)

## Bildirilen sorun

Yöneticim, `main`'deki son hâli inceledikten sonra bir ekran görüntüsüyle şunu bildirdi: bir `BusinessException` fırlatıldığında (örnekte: Mentor = Stajyer ile aynı kullanıcı seçilmiş, `MentorCannotBeSameAsIntern` kuralı ihlal edilmiş) ekranda şu görünüyordu:

> Exception of type 'Volo.Abp.BusinessException' was thrown.

Gerçek kuralın ("Mentor, stajyerin kendisiyle aynı kullanıcı olamaz.") hiçbir izi yok. Geri bildirim: *"Aslında bu tarz validasyonlarda kullanıcılara direkt ex basmak yerine hatanın ne olduğunu söylersen daha iyi olabilir."* Talimat: her olası validasyon durumu için uygun bir hata açıklaması gösterilsin.

## Kök neden

Tüm `catch (BusinessException ex)` bloklarında (`Workplaces`, `InternProfiles`, `Profile`, `DailyLogs` sayfalarında, 9 farklı yer) doğrudan `ex.Message` kullanılıyordu. ABP'de `BusinessException(code)` şu şekilde çağrıldığında:

```csharp
throw new BusinessException(InternshipJournalDomainErrorCodes.MentorCannotBeSameAsIntern);
```

`.Message` özelliği **otomatik olarak lokalize olmaz** — yalnızca ASP.NET Core'un kendi exception-handling middleware'i (HTTP API hata yanıtları için) `Code` üzerinden bir lokalizasyon araması yapar. Razor Pages'te `try/catch` ile exception'ı biz yakalayınca bu otomatik mekanizma hiç devreye girmiyor; `.Message` sabit .NET varsayılanına (`"Exception of type '...' was thrown."`) düşüyor.

İkinci bir kök neden: proje genelinde (Gün 6'dan beri) hiçbir hata kodu için `tr.json`/`en.json`'da bir çeviri kaydı yoktu — yalnızca kod tanımlarının üzerindeki XML doc yorumlarında Türkçe açıklama vardı, kullanıcıya hiç ulaşmıyordu.

## Uygulanan çözüm

1. `InternshipJournalPageModel`'e paylaşılan bir `GetErrorMessage(BusinessException ex)` metodu eklendi: `ex.Code`'a karşılık gelen bir çeviri anahtarı varsa (`L[ex.Code].ResourceNotFound == false`) onu döndürür; yoksa (örneğin başka bir modülden gelen bir istisna) `ex.Message`'a geri düşer.
2. `InternshipJournalDomainErrorCodes.cs`'deki **40 hata kodunun tamamı** için `tr.json` ve `en.json`'a gerçek kullanıcı mesajı eklendi — anahtar olarak kodun kendisi (`"InternshipJournal:MentorCannotBeSameAsIntern"`) kullanıldı, bu ABP'nin kendi `BusinessException` + `LocalizationResource` eşleştirme kuralıyla (dosyanın kendi başındaki "Norm" yorumuyla) birebir uyumlu.
3. 9 `catch` bloğunun tamamında `ex.Message` → `GetErrorMessage(ex)` olarak değiştirildi (`Workplaces/Create`, `Workplaces/Edit`, `InternProfiles/Create`, `InternProfiles/Edit`, `Profile/Edit`, `DailyLogs/Create`, `DailyLogs/Index`, `DailyLogs/Detail`).
4. Regresyonu önlemek için bir test eklendi: `ErrorCodeLocalizationTests.AllErrorCodes_ShouldHaveLocalizedMessage` — reflection ile `InternshipJournalDomainErrorCodes`'daki her `const string` alanını okuyup hem `tr` hem `en` kültüründe bir çeviri bulunduğunu doğruluyor. Yeni bir hata kodu eklenip çevirisi unutulursa bu test kırılır.

## Doğrulama

- Tam çözüm derlemesi: 0 hata.
- `ErrorCodeLocalizationTests` (2 test, tr+en): 40 kodun tamamı için geçiyor.
- `EntityFrameworkCore.Tests` 47/47, `Domain.Tests` 31/31, `Web.Tests` 3/3 (yeni testler dahil) — hiçbir mevcut test bozulmadı.
- Canlı tarayıcıda yeniden denemedim (yerel Postgres'e erişimim yok); yöneticimin bildirdiği senaryoyu (Mentor = Intern) tekrar deneyip artık "Mentor, stajyerin kendisiyle aynı kullanıcı olamaz." mesajını gördüğünü teyit etmesi gerekiyor.

## Öğrendiğim konu

`BusinessException.Code` + kaynak dosyasındaki anahtar eşleşmesi, ABP'nin HTTP API hata yanıtları için otomatik çalışan bir mekanizma — ama yalnızca çerçevenin kendi exception-handling boru hattından geçen istisnalar için. Razor Pages'te `try/catch` ile manuel yakalanan bir exception, o boru hattını hiç görmüyor; lokalizasyonu biz elle tetiklemek zorundayız. "Hata kodu yazdım, `docs`'a açıklamasını da yazdım" adımı kullanıcıya hiçbir şey ulaştırmıyormuş — mesajın gerçekten görüneceği yeri (burada: `IStringLocalizer` + Razor Pages catch bloğu) ayrıca düşünmek gerekiyor.
