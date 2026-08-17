# Gün 11

Tarih: 17 Ağustos 2026, Pazartesi

## Bugün tamamladığım işler

- ABP çözümünü (`InternshipJournal`) `abp new` ile oluşturdum: Domain.Shared, Domain, Application.Contracts, Application, EntityFrameworkCore, HttpApi, Web, DbMigrator, test projeleri.
- Domain.Shared'a enum'ları (`DailyLogStatus`, `WorkType`, `LearningLevel`, `MentorReviewDecision`, `InternshipStatus`), const sınıflarını (10 adet) ve hata kodlarını ekledim.
- `Country`, `Province`, `District`, `Skill` entity'lerini davranışlarıyla (Rename, Activate, Deactivate, Skill için ayrıca ChangeCategory/ChangeDescription) yazdım.
- EF Core mapping'lerini (`AppCountries`, `AppProvinces`, `AppDistricts`, `AppSkills`) unique constraint ve foreign key'lerle birlikte `InternshipJournalDbContext`'e ekledim.
- Tekrar çalıştırılabilir (idempotent) `InternshipJournalDataSeedContributor` ve sabit GUID'li `InternshipJournalSeedIds` yazdım; 3 ülke, 5 il, 11 ilçe, 14 yetkinlik seed edildi.
- `CountryLookupDto`, `ProvinceLookupDto`, `DistrictLookupDto` ve `ILocationAppService`/`LocationAppService`'i (yalnız aktif, ada göre sıralı, üst konuma göre filtreli) geliştirdim.
- Migration oluşturdum ve `dotnet build` ile derleme hatasız geçti.
- Domain ve Application Service testlerini yazdım: `Seed_WhenExecutedTwice_ShouldNotCreateDuplicates`, `GetCountries_ShouldReturnOnlyActiveCountries`, `GetProvinces_ShouldReturnOnlySelectedCountry`, `GetDistricts_ShouldReturnOnlySelectedProvince`, `InactiveLocation_ShouldNotBeReturned`.

## Öğrendiğim / pekiştirdiğim konular

- **Domain.Shared'ın rolü:** Enum ve const'lar burada olunca hem Domain hem Web katmanı aynı tanıma bağımlı kalıyor, tekrar oluşmuyor.
- **Idempotent seed:** Seed contributor'ın ikinci çalıştırmada aynı kaydı tekrar oluşturmaması için sabit GUID + var olma kontrolü şart.
- **Lookup DTO'nun küçüklüğü bilinçli bir tercih:** Dropdown yalnızca Id/Name/Code'a ihtiyaç duyar; entity'nin tamamını (audit alanları dahil) dışarı sızdırmamak için ayrı DTO kullanılır.
- **internal constructor + InternalsVisibleTo:** Entity'lerin sadece Domain katmanında (ve izin verilen test projelerinde) inşa edilebilmesi, iş kuralının dışarıdan atlanmasını engelliyor.

## Alınan kararlar

1. Province/District/Skill için de Activate/Deactivate davranışı eklendi (dokümanda sadece Country ve Skill için önerilmişti); pasif konum testinin gerçekçi kurulabilmesi için tutarlılık amacıyla genişletildi.
2. Seed idempotency kontrolü, kod/isim eşleşmesine göre yapıldı (ör. `Country.Code == "TR"`), sabit GUID sadece tekrar üretilebilirlik için kullanıldı.

## Yapay zekâ kullanımı

Entity alan/uzunluk tanımlarını `docs/database/table-catalog.md` ile, enum değerlerini `docs/abp/layer-responsibilities.md` ve önceki günlerin domain dokümanlarıyla çapraz kontrol ederek tutarlılığı doğruladım. Kod, `dotnet build` ve ilgili testlerle doğrulandı.

## Kabul kriteri kontrolü

- [x] Referans tablolar oluştu
- [x] Seed verileri oluştu
- [x] Seed ikinci çalıştırmada duplicate üretmedi
- [x] Foreign key ilişkileri doğru
- [x] Unique constraint'ler oluştu
- [x] Lookup servisleri doğru filtreli sonuç döndürüyor
- [x] Entity doğrudan UI'a dönmüyor (DTO üzerinden)
- [x] Testler geçiyor

## Yarın yapacaklarım

- Gün 12: Workplace aggregate, nested adres seçimi (ülke → il → ilçe), WorkplaceManager ve Razor Pages ekranları.
