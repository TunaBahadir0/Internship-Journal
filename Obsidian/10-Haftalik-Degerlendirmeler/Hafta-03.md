# 3. Hafta Değerlendirmesi

Tarih: 24 Ağustos 2026, Pazartesi (Gün 11 - Gün 15)

## Seed yazarken dikkat ettiğim noktalar

Seed verilerinin (ülke/il/ilçe, yetkinlik kataloğu) sabit `Guid`'lerle tanımlanması (`InternshipJournalSeedIds`), testlerin ve gerçek çalıştırmaların aynı kimliklere güvenebilmesini sağladı. Bu sayede `EfCoreDailyLogAppServiceTests` gibi bu hafta yazdığım testler, `InternshipJournalSeedIds.Districts.Kadikoy` gibi sabit referanslarla çalışma yeri oluşturabildi — seed her test çalıştırmasında yeniden uygulanabilir (idempotent) olmasa test verisi kırılgan olurdu.

## Nested adres yapısında öğrendiklerim

`Country → Province → District → Workplace` zincirinin her katmanının kendi `IsActive`/pasifleştirme kuralına sahip olması (silme değil), üst seviyedeki bir referans veri değiştiğinde alt seviyedeki gerçek kayıtların (ör. bir çalışma yerine bağlı stajyer) bozulmamasını sağlıyor. `DailyLog` haftasında da aynı ilkeyi `Skill` için tekrar uyguladım (`AppDailyLogSkills.SkillId → Restrict`).

## Value Object kullanma gerekçem

`DateRange`'i Gün 13'te yazarken, aynı kuralın (bitiş başlangıçtan önce olamaz) hem `InternProfile.InternshipPeriod` hem de ileride başka bir tarih aralığı ihtiyacında tekrar tekrar yazılmasını istemedim. Value Object, kuralı tek bir yerde (kendi constructor'ında) korurken, iki `DateTime` alanını anlamlı tek bir kavram olarak taşımayı da sağladı — `Contains`/`Overlaps`/`DurationInDays` gibi davranışlar da doğal olarak oraya ait.

## DailyLog aggregate'ının koruduğu kurallar

- Aynı stajyer aynı gün için yalnızca bir günlük oluşturabilir (aggregate dışında, `DailyLogManager` + veritabanı unique index — çift savunma).
- `TotalMinutes` hiçbir zaman doğrudan set edilemez; yalnızca `_items` toplamından türetilir.
- Durum makinesi (`Draft → Submitted → Approved/RevisionRequested → Draft`) yalnızca tanımlı geçişlere izin verir; `EnsureEditable` tekrarlayan kontrolü tek yerde topluyor.
- Aynı yetkinlik bir günlüğe iki kez eklenemez; AI kullanıldıysa araç/özet, öneri reddedildiyse gerekçe zorunlu.

Bu kuralların hepsi saf C# ile (`DailyLogTests`, DI'sız) doğrulandı; veritabanı yalnızca bunları ikinci bir savunma hattı olarak (unique constraint, FK) tekrarlıyor.

## EF Core mapping'de zorlandığım bölüm

Child entity'lerin `IReadOnlyCollection<T>` tipindeki genel koleksiyon property'lerini (`Items`/`Skills`/`Problems`) EF Core'a maplemek — `HasMany(x => x.Items)` derlendi ama `PropertyAccessMode.Field` demeden runtime'da materyalize edemedi, çünkü property'nin set'i yok. Ayrıca gölge FK (`HasForeignKey("DailyLogId")`) kavramı ilk kez bu hafta karşıma çıktı: domain modelinde var olmayan bir kolonun veritabanında var olabilmesi.

## Testlerin yakaladığı önemli hata

`DailyLogManagerTests` içinde `DomainService.GuidGenerator`'ın DI container olmadan (`new DailyLogManager(...)` ile doğrudan kurulan testte) `NullReferenceException` verdiğini yakaladım — çünkü bu property `LazyServiceProvider` üzerinden lazy çözülüyor. Ayrıca `DailyLogAppServiceTests.UpdateSummary_WhenLogApproved_ShouldFail` testinde, yardımcı metodumun `GetAsync` yerine `GetWithDetailsAsync` kullanması gerektiğini (Include eksikliği yüzünden `Submit()`'in "en az bir madde" hatasını yanlış nedenle attığını) testin kendisi gösterdi — production kodunda değil, test yardımcı kodunda bir hataydı, ama testin doğru hatayı (yanlış satırda) yakalaması sayesinde fark edildi.

## Gelecek hafta hedefim

4. Haftada `DailyLogAppService`'i tamamlamak (`AddItem`/`AddSkill`/`AddProblem`/`Submit`/`Approve` gibi kalan uçtan uca akışlar), Razor Pages tarafında günlük giriş ekranını inşa etmek, ve ayrı bir aggregate olarak tasarlanan `MentorReview`'ı (`docs/database/table-catalog.md`'de zaten planlanmış) uygulamak.

## Haftalık kabul kriterleri

- [x] Referans seed verileri tekrar çalıştırılabilir
- [x] Workplace nested adresle çalışıyor
- [x] InternProfile geçerli staj dönemiyle oluşturuluyor
- [x] DailyLog aggregate iş kurallarını koruyor
- [x] Mapping ve migration doğru
- [x] Constraint'ler veritabanında mevcut
- [x] DTO ve entity ayrımı uygulanıyor
- [x] Domain ve Application testleri geçiyor (Domain.Tests 31/31, EntityFrameworkCore.Tests 31/31, Web.Tests dahil tam çözüm 0 hata)
