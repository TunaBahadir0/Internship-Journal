# Katman Sorumlulukları

## Domain.Shared

- Enum: `WorkType`, `DailyLogStatus`, `LearningLevel`, `MentorReviewDecision`
- Sabitler (örn. alan uzunlukları: `DailyLogConsts.MaxSummaryLength`)
- Hata kodu tanımları (`InternshipJournalDomainErrorCodes`)
- Diğer tüm katmanların bağımlı olabileceği en temel, framework'ten bağımsız tipler

## Domain

- Entity: `DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry`
- Aggregate Root: `DailyLog`, `MentorReview`, `InternProfile`, `Workplace`, `Skill`, `Country`, `Province`, `District`
- Value Object: `DateRange`
- Domain Service: `InternProfileManager` (tek aktif profil kuralını, DB kısıtına ek olarak burada da doğrular — BR-8, v2-1)
- Repository arayüzleri: `IDailyLogRepository`, `IInternProfileRepository`, ...
- Domain Event: örn. `DailyLogSubmittedEto`, `MentorReviewCompletedEto`
- Invariant'lar (`docs/domain/invariants.md`) burada, entity metotları içinde uygulanır — dışarıdan set edilebilir public property olarak değil.

## Application.Contracts

- DTO: `DailyLogDto`, `CreateUpdateDailyLogDto`, `MentorReviewDto`
- Application Service arayüzleri: `IDailyLogAppService`, `IMentorReviewAppService`, `IInternProfileAppService`
- Kullanım senaryosu sözleşmeleri (girdi/çıktı şekli), herhangi bir iş kuralı veya EF Core referansı içermez

## Application

- Application Service implementasyonları: `DailyLogAppService`, `MentorReviewAppService`
- Yetki kontrolü (`[Authorize(InternshipJournalPermissions.DailyLogs.Submit)]`)
- Entity ↔ DTO dönüşümü (AutoMapper profil sınıfları)
- Kullanım senaryosu koordinasyonu: repository çağırma, domain service tetikleme, transaction sınırı

## EntityFrameworkCore

- `InternshipJournalDbContext`
- Entity mapping (`ConfigureInternshipJournal` içinde `builder.Entity<DailyLog>()...`)
- Migration'lar (`Migrations/` klasörü)
- Repository implementasyonları (`EfCoreDailyLogRepository` veya ABP generic repository)
- `docs/database/table-catalog.md`, `constraints.md`, `indexes.md` burada koda döner (FK, unique index, kısmi unique index — v2-1)

## Web

- Razor Pages: `Pages/DailyLogs/Index.cshtml`, `Pages/DailyLogs/CreateModal.cshtml`
- PageModel: sayfa arkası kod, sadece Application Service çağırır — iş kuralı içermez
- Menü, layout, kullanıcı arayüzü etkileşimi

## DbMigrator

- Migration'ları veritabanına uygulama (`dbcontext.Database.Migrate()`)
- Seed veri çalıştırma (`ICountrySeedContributor`, `IPermissionDataSeeder`, admin kullanıcı/rol)
- Uygulama başlamadan önce, ayrı bir konsol projesi olarak elle çalıştırılır

---

## ABP not çalışması — sorular ve cevaplar

**Hangi sınıf hangi projede bulunmalı?**
Sınıfın rolüne göre: framework'ten bağımsız sabit/enum → Domain.Shared; iş kuralı taşıyan sınıf → Domain; dış sözleşme (DTO/arayüz) → Application.Contracts; sözleşmenin implementasyonu → Application; veritabanı erişimi → EntityFrameworkCore; kullanıcı arayüzü → Web.

**Domain katmanı neden Web katmanına bağımlı olmamalı?**
Domain, iş kurallarının kaynağıdır ve birden fazla arayüzden (Web, HttpApi, gelecekte mobil) kullanılabilmelidir. Web'e bağımlı olursa iş kuralları belirli bir sunum teknolojisine kilitlenir ve test edilebilirliği kaybolur.

**DTO neden Application.Contracts içinde bulunur?**
DTO, dış dünyaya açılan sözleşmenin parçasıdır; Application katmanının implementasyon detaylarından (repository, domain service çağrıları) ayrı tutulmalıdır ki HttpApi.Client gibi projeler Application'a değil sadece Contracts'a bağımlı olsun.

**DbMigrator ne zaman çalıştırılır?**
Uygulama ilk kurulurken ve her migration eklendiğinde, Web/HttpApi başlatılmadan önce elle çalıştırılır. Migration'ları veritabanına uygular ve seed veriyi (referans veriler, admin kullanıcı, roller, izinler) yükler.

**Generic repository hangi işlemleri sağlar?**
`IRepository<TEntity, TKey>` üzerinden temel CRUD (`GetAsync`, `InsertAsync`, `UpdateAsync`, `DeleteAsync`), `GetListAsync`/`GetQueryableAsync` ile sorgulama ve ABP'nin otomatik uyguladığı soft-delete/audit filtreleme.

**Application Service'in sorumluluğu nedir?**
Bir kullanım senaryosunu uçtan uca koordine etmek: yetki kontrolü, DTO → domain çağrısı dönüşümü, repository/domain service kullanımı, sonucu DTO olarak döndürme. İş kuralının kendisini içermez, kuralı domain katmanına devreder.

**PageModel içine hangi kodlar yazılmalıdır?**
Sadece sayfa akışına ait kod: Application Service çağırma, sayfa modelini doldurma, yönlendirme/mesaj gösterme. İş kuralı, veritabanı sorgusu veya doğrudan repository erişimi PageModel içine yazılmaz.

**Permission ile veri sahipliği kontrolü arasındaki fark nedir?**
Permission ("DailyLogs.Submit" gibi), kullanıcının bir *işlemi genel olarak* yapmaya yetkili olup olmadığını kontrol eder. Veri sahipliği kontrolü ise o kullanıcının *o belirli kaydın* sahibi/mentoru olup olmadığını kontrol eder (örn. bir mentor sadece kendi stajyerinin günlüğünü inceleyebilir). İkisi birlikte çalışır; permission yetkiyi, sahiplik kontrolü kapsamı sınırlar.
