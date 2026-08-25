# Gün 18

Tarih: 26 Ağustos 2026, Çarşamba

## Kapsam notu — son 3 gün, teslim Gün 20

Kullanıcı bugün programın 20 günde bittiğini ve teslim edileceğini hatırlattı. 4. Hafta'nın gün gün müfredatı olmadığı için (bkz. Gün 16), kalan işi kendim sıkıştırdım: **Gün 18 = `MentorReview`'ı uçtan uca (Domain+EF+AppService) bitirmek**, Gün 19 = yetkilendirme (Stajyer/Mentor rolleri) + mentor tarafı Razor Pages, Gün 20 = final test/dokümantasyon/teslim. Master dokümanın "13. Mentor incelemesi" bölümündeki tam spesifikasyonu (alanlar, karar türleri, davranışlar, iş kuralları) birebir uyguladım.

## Bugün tamamladığım işler

- `MentorReviewDecision` enum'unu (`Approved`/`RevisionRequested`) ve `MentorReviewConsts.MaxCommentLength` (1000) sabitini ekledim.
- `MentorReview` aggregate'ini (`FullAuditedAggregateRoot<Guid>`) yazdım — `DailyLogId`, `MentorUserId`, `Decision`, `Comment`, `ReviewedAt`. Bu, `DailyLog` gibi durumdan-duruma geçen bir varlık değil; **tek seferlik bir inceleme kaydı** olduğu için `Approve`/`RequestRevision` instance metodu değil, `internal static` factory metodu olarak modellendi (`MentorReview.Approve(...)`, `MentorReview.RequestRevision(...)`) — bir MentorReview nesnesi oluşturulduktan sonra hiç değişmiyor.
- `RequestRevision` factory'sinde "düzeltme talebinde yorum zorunludur" kuralını (`string.IsNullOrWhiteSpace` kontrolü) doğrudan uyguladım.
- `IMentorReviewRepository` (yalnızca `GetListByDailyLogIdAsync` — ileride inceleme geçmişi ekranı için).
- `MentorReviewManager` domain servisini yazdım: `ApproveAsync`/`RequestRevisionAsync(dailyLogId, mentorUserId, comment)`. Cross-aggregate kontroller: günlük bulunuyor mu, mentor gerçekten o stajyerin mentoru mu (`InternProfile.MentorUserId == mentorUserId`) — "mentor günlük içeriğini değiştiremez" kuralı zaten doğal olarak sağlanıyor çünkü Manager `DailyLog`'un yalnızca `Approve()`/`RequestRevision()` durum metotlarını çağırıyor, Items/Skills/Problems'a hiç dokunmuyor. "Yalnızca Submitted günlük incelenebilir" kuralını ayrıca kontrol etmedim — zaten `DailyLog.Approve()`/`RequestRevision()` bunu Gün 14'ten beri kendi içinde koruyor, tekrar yazmak gereksiz olurdu.
- Manager metotları `(MentorReview Review, DailyLog DailyLog)` tuple'ı döndürüyor — Manager, `DailyLog`'u kendi içinde `GetWithDetailsAsync` ile ayrıca çekip mutasyona uğrattığı için, AppService'in persist edebilmesi için AYNI (mutasyona uğramış) örneği geri vermesi gerekiyordu; yalnızca `MentorReview`'i döndürseydim `DailyLog`'un durum değişikliği hiç kaydedilmezdi.
- EF Core mapping (`AppMentorReviews` tablosu, `DailyLogId` FK **Restrict** — ayrı aggregate olduğu için günlükle otomatik silinmiyor, `MentorUserId`'ye FK yok — modüller arası FK kurmama kuralıyla tutarlı) ve `Added_MentorReview_Module` migration'ı; `docs/database/table-catalog.md`/`constraints.md`'deki (Gün 8) tasarımla birebir eşleşti.
- `MentorReviewRepository`, `MentorReviewDto`, `ApproveDailyLogReviewInput` (Comment opsiyonel), `RequestDailyLogRevisionInput` (Comment `[Required]`), `IMentorReviewAppService`/`MentorReviewAppService` (`GetListByDailyLogAsync`, `ApproveAsync`, `RequestRevisionAsync` — mentor kimliği `CurrentUser.GetId()`'den çözülüyor, DTO'da hiç yok).
- 2 yeni hata kodu (`MentorReviewCommentRequiredForRevision`, `MentorReviewNotAuthorized`, `MentorReviewDailyLogNotFound`) — hepsi tr/en çevirileriyle birlikte eklendi (`ErrorCodeLocalizationTests` bunu otomatik doğruladı).
- Testler: `MentorReviewTests` (4, saf) + `MentorReviewManagerTests` (6, NSubstitute) + `MentorReviewAppServiceTests` (5, gerçek DI). Toplam 15 yeni test. `Domain.Tests` 41/41, `EntityFrameworkCore.Tests` 52/52 geçiyor.

## Öğrendiğim / pekiştirdiğim konular

- **Her aggregate'in "durum makinesi" olması gerekmiyor:** `DailyLog` durumdan duruma geçen (Draft→Submitted→...) bir varlık; `MentorReview` ise tek bir anlık kararın değişmez kaydı. İkisini de `FullAuditedAggregateRoot` yaptım ama davranış modellemesi tamamen farklı — biri instance metotlarıyla mutasyona uğruyor, diğeri yalnızca factory metoduyla bir kere oluşturuluyor ve bir daha değişmiyor.
- **ABP'nin `[Required]` doğrulamasının DataAnnotations'tan daha sıkı olduğu:** `RequestDailyLogRevisionInput.Comment`'i yalnızca boşluklardan oluşan bir string ile test ederken, beklediğim domain seviyesi `BusinessException` değil, ABP'nin kendi `AbpValidationException`'ı fırlatıldı — ABP'nin `MethodInvocationValidator`'ı `[Required]` alanlar için düz `IsNullOrEmpty` değil `IsNullOrWhiteSpace` benzeri bir kontrol yapıyor. Bu, domain katmanındaki aynı kontrolün (`MentorReview.RequestRevision`) AppService üzerinden HİÇBİR ZAMAN tetiklenmeyeceği, yalnızca Manager doğrudan (validasyonsuz) çağrıldığında bir işe yarayacağı anlamına geliyor — yine de bilinçli bir "ikinci savunma hattı" olarak bıraktım, testlerde bunu açıkça yorumla belirttim.
- **Manager'ın iki aggregate'i aynı anda mutasyona uğrattığı senaryoda "kimin neyi kaydettiği" netliği:** `MentorReviewManager` hem yeni bir `MentorReview` üretiyor hem de var olan bir `DailyLog`'u mutasyona uğratıyor; ama hiçbirini persist etmiyor (Domain katmanı repository'ye yazmıyor, yalnızca okuyor). AppService'in her iki nesneyi de (`InsertAsync` + `UpdateAsync`) ayrı ayrı kaydetmesi gerekiyor — Manager'ın dönüş tipini tuple yapmasaydım, `DailyLog` tarafı sessizce kaybolurdu (yazıp fark etmeden bırakabileceğim bir hataydı, kod yazarken kendim yakaladım).

## Alınan kararlar

1. `MentorReviewManager`, `DailyLog.Approve()`/`RequestRevision()`'ın zaten yaptığı "yalnızca Submitted günlük incelenebilir" kontrolünü tekrar etmedi — aynı kural iki yerde farklı şekilde ifade edilirse (ör. biri güncellenip diğeri unutulursa) tutarsızlık riski doğar; tek doğruluk kaynağı `DailyLog`'un kendisi.
2. Manager metotları `(MentorReview, DailyLog)` tuple döndürüyor, tek bir "sonuç nesnesi" sarmalayıcı sınıf yazmadım — iki aggregate'in birlikte döndüğünü ifade etmenin en basit yolu, gereksiz bir DTO/sonuç sınıfı eklemeden.
3. Mentor kimliği (`mentorUserId`) hiçbir DTO'da yer almıyor, her zaman `CurrentUser.GetId()`'den okunuyor — Gün 15'teki "stajyer kendi InternProfileId'sini gönderemez" kararıyla aynı ilke: bir kullanıcı, kendi kimliği dışında birini "ben mentörüm" diyerek işaretleyemesin.

## Yapay zekâ kullanımı

`ApproveAsync_WhenCommentEmpty`/`RequestRevisionAsync_WhenCommentWhitespace` testlerini ilk yazdığımda hangi exception türünün geleceğini varsaymadım; testi çalıştırıp gerçek hatayı (`AbpValidationException`, beklediğim `BusinessException` değil) okuyup kök nedenini (ABP'nin kendi doğrulama katmanının domain koduna hiç ulaşmadan araya girmesi) araştırdıktan sonra testi düzelttim ve bunu günlükte açıkladım.

## Kabul kriteri kontrolü

- [x] `MentorReview` yalnızca `Submitted` günlüğü inceleyebiliyor (DailyLog'un kendi kontrolü üzerinden)
- [x] Düzeltme talebinde yorum zorunlu
- [x] Mentor yalnızca kendisine bağlı stajyerin günlüğünü inceleyebiliyor
- [x] Mentor günlük içeriğini değiştiremiyor
- [x] İnceleme işlemi DailyLog durumunu da güncelliyor
- [x] Migration, tasarım dokümanlarıyla (table-catalog.md/constraints.md) birebir eşleşiyor
- [x] Domain.Tests 41/41, EntityFrameworkCore.Tests 52/52 geçiyor

## Yarın yapacaklarım

- Gün 19: Yetkilendirme/permission yapısı (Stajyer/Mentor rolleri, ABP Permission tanımları) ve mentor tarafı Razor Pages (bekleyen incelemeler listesi, onayla/düzeltme iste ekranı).
- Gün 20: Final test geçişi, README/Obsidian tamamlama, 4. Hafta değerlendirmesi, demo hazırlığı, teslim.
