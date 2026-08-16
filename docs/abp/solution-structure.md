# ABP Çözüm Yapısı

ABP Framework şablonundan üretilen çözüm, DDD katmanlarına birebir karşılık gelen ayrı projelerden oluşur. Aşağıdaki liste, projenin ana çözümünde (`InternshipJournal.sln`) yer alacak projeleri gösterir.

```text
InternshipJournal.Domain.Shared
InternshipJournal.Domain
InternshipJournal.Application.Contracts
InternshipJournal.Application
InternshipJournal.EntityFrameworkCore
InternshipJournal.HttpApi
InternshipJournal.HttpApi.Client
InternshipJournal.Web
InternshipJournal.DbMigrator
InternshipJournal.Domain.Tests
InternshipJournal.Application.Tests
InternshipJournal.EntityFrameworkCore.Tests
InternshipJournal.Web.Tests
InternshipJournal.TestBase
```

## Proje bağımlılık yönü

```text
Domain.Shared
   ↑
Domain
   ↑
Application.Contracts ← (DTO sözleşmeleri Domain'den bağımsız)
   ↑
Application
   ↑
EntityFrameworkCore   HttpApi   Web
```

Bağımlılıklar tek yönlüdür: alt katman üst katmanı bilmez. `Domain`, `Web`'i; `Application.Contracts`, `Application`'ı tanımaz. Bu sıra korunmazsa katmanlar arası döngüsel bağımlılık oluşur ve testler yalıtılamaz.

## `docs/domain/domain-model-v2.md` ile eşleşme

v2 domain modelindeki aggregate'ler bu projelere şöyle dağılır:

| Aggregate / kavram | Bulunduğu proje |
|---|---|
| `DailyLog`, `DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry` | Domain |
| `MentorReview` | Domain |
| `InternProfile`, `Workplace`, `Skill`, `Country`, `Province`, `District` | Domain |
| `DateRange` (value object) | Domain |
| `WorkType`, `DailyLogStatus`, `LearningLevel`, `MentorReviewDecision` enum'ları | Domain.Shared |
| `DailyLogDto`, `CreateDailyLogDto`, `IDailyLogAppService` | Application.Contracts |
| `DailyLogAppService` | Application |
| `InternshipJournalDbContext`, EF Core mapping, migration'lar | EntityFrameworkCore |
| Razor Pages (`Pages/DailyLogs/...`) | Web |
| Seed veri (Country/Province/District, admin kullanıcı, roller) | DbMigrator |

## Not

`docs/database/table-catalog.md` içindeki 11 tablo, EntityFrameworkCore katmanındaki `DbContext` üzerinden Domain katmanındaki aggregate'lere mapping ile bağlanacak. Bu doküman aşamasında henüz kod yazılmadı; amaç, mevcut onaylı tasarımın (v2) hangi projeye nasıl yerleşeceğini netleştirmek.
