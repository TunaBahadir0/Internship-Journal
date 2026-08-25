# Test Stratejisi

## Amaç

Bu proje boyunca kullanılan test yaklaşımını ve her katmanın hangi türde test aldığını, neden o türü aldığını açıklar.

## Üç katmanlı yaklaşım

### 1. Saf domain testleri (`test/InternshipJournal.Domain.Tests`, DI'sız)

Value object'ler ve aggregate davranışları, herhangi bir DI konteyneri veya veritabanı olmadan doğrudan `new` ile kurulup test edilir (ör. `DateRangeTests`, `DailyLogTests`, `MentorReviewTests`). Aggregate'lerin `internal` constructor'larına test projesinden erişim, `InternalsVisibleTo` (`AssemblyInfo.cs`) ile sağlanır.

**Neden:** Bir aggregate'in iş kuralı, bir veritabanına veya HTTP isteğine ihtiyaç duymamalı — saf C# olarak doğrulanabilmeli. Bu testler milisaniyeler içinde çalışır ve iş kuralını değişiklik yapan herkese anında geri bildirim verir.

### 2. NSubstitute ile mock'lanmış Domain Service testleri (aynı proje)

Bir Domain Service (Manager) başka bir aggregate'in repository'sine ihtiyaç duyduğunda (ör. `DailyLogManager`'ın `IInternProfileRepository`'ye, `MentorReviewManager`'ın hem `IDailyLogRepository` hem `IInternProfileRepository`'ye ihtiyacı), gerçek bir veritabanı kurmak yerine bu bağımlılıklar NSubstitute ile mock'lanır (`DailyLogManagerTests`, `MentorReviewManagerTests`).

**Neden:** Cross-aggregate kuralın kendisini (ör. "mentor yalnızca kendisine bağlı stajyerin günlüğünü inceleyebilir") test etmek istiyoruz — repository'nin SQL sorgusunun doğruluğunu değil (o, ayrı bir endişe). Mock, "Manager doğru girdi/çıktı ile doğru kararı veriyor mu" sorusuna gerçek bir veritabanından çok daha hızlı ve kararlı (deterministic) cevap verir.

### 3. Tam DI, SQLite bellek-içi veritabanı testleri (`test/InternshipJournal.EntityFrameworkCore.Tests`)

Application Service'ler (`DailyLogAppServiceTests`, `MentorReviewAppServiceTests`, `InternProfileAppServiceTests`, `WorkplaceAppServiceTests`, `LocationAppServiceTests`, `SkillAppServiceTests`) gerçek bir ABP modülü (`InternshipJournalEntityFrameworkCoreTestModule`) içinde, gerçek EF Core mapping'i kullanan bir SQLite bellek-içi veritabanına karşı çalışır. Test sınıfları `TStartupModule` generic parametresiyle yazılır (`test/InternshipJournal.Application.Tests`'te tanımlı, soyut) ve somut hâli EF Core test projesinde (`EfCore*Tests`) `[Collection]` ile aynı veritabanını paylaşacak şekilde örneklenir.

**Neden:** Application Service, birden fazla aggregate'i (Manager'lar, repository'ler) koordine eder ve gerçek EF Core mapping'inin (ör. shadow FK, owned type, unique index) doğru çalıştığını da dolaylı olarak doğrular. `CurrentUser`, `ICurrentPrincipalAccessor.Change(...)` ile taklit edilir (`LoginAs` yardımcı metodu) — böylece sahiplik/yetki testleri (ör. "başka birinin günlüğünü güncelleyemezsin") gerçek bir kullanıcı bağlamında çalışır.

### 4. Uygulama boot testi (`test/InternshipJournal.Web.Tests`)

Tam ASP.NET Core host'u (`AbpWebApplicationFactoryIntegratedTest`) ayağa kaldırılıp bir HTTP isteği atılır (`Index_Tests.Welcome_Page`). Ayrıca `ErrorCodeLocalizationTests`, reflection ile `InternshipJournalDomainErrorCodes`'daki her hata kodunun hem `tr` hem `en` kültüründe bir çevirisi olduğunu doğrular.

**Neden:** Yeni bir menü girişi, sayfa veya DI kaydı eklendiğinde host'un gerçekten ayağa kalktığını (eksik bir bağımlılık, yanlış bir modül referansı olmadığını) doğrulayan tek test. `ErrorCodeLocalizationTests`, Gün 17'de yaşanan "kullanıcıya ham exception metni gösterme" hatasının bir daha sessizce geri gelmemesini garanti eder.

## Permission testleri hakkında not

`InternshipJournalEntityFrameworkCoreTestModule`, `PermissionManagementOptions.IsDynamicPermissionStoreEnabled = false` ile yapılandırılmıştır (Gün 10). Bu, test host'unda `[Authorize(Permission)]` kontrollerinin **etkin olmadığı** anlamına gelir — testler iş mantığına odaklanır, izin/rol altyapısına değil. Gerçek yetkilendirme, sahiplik kontrolleriyle (ör. `EnsureOwnerAsync`, `MentorReviewManager`'daki mentor eşleşmesi) test edilir; bunlar saf iş kuralı olduğu için mock'lanmış/DI'lı testlerde gerçekten çalışır ve doğrulanır.

## Sayılar (Gün 20 itibarıyla)

| Proje | Test sayısı |
|---|---:|
| `InternshipJournal.Domain.Tests` | 41 |
| `InternshipJournal.EntityFrameworkCore.Tests` | 56 |
| `InternshipJournal.Web.Tests` | 3 |
| **Toplam** | **100** |

Tam çözüm derlemesi: 0 hata.
