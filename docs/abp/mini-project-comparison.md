# Mini Proje ↔ ABP Karşılaştırması

Daha önce yazılan küçük (mini) `WorkEntry` projesi ile ABP Framework şablonunun karşılaştırması:

| Mini proje | ABP karşılığı |
|---|---|
| `WorkEntry` modeli | Entity veya Aggregate (burada: `DailyLog`) |
| `IWorkEntryService` | Application Service sözleşmesi (`IDailyLogAppService`, Application.Contracts) |
| `EfCoreWorkEntryService` | Repository + Application Service implementasyonu (EntityFrameworkCore + Application) |
| Input modeli | DTO (`CreateUpdateDailyLogDto`) |
| `DbContext` | EntityFrameworkCore katmanı (`InternshipJournalDbContext`) |
| Migration komutu (elle) | Migration + `DbMigrator` projesi |
| Manuel `CreatedAt` alanı | Audited entity (`FullAuditedAggregateRoot`, `CreationTime` otomatik) |
| Manuel yetki kontrolü (`if (user.Role != "Mentor")`) | Permission altyapısı (`[Authorize(Permissions...)]`, `PermissionDefinitionProvider`) |
| Manuel soft-delete (`IsDeleted` alanı elle kontrol) | `ISoftDelete` / `FullAuditedAggregateRoot` otomatik filtreleme |
| Manuel çoklu tenant desteği yok | Çok kiracılı altyapı hazır (bu projede kullanılmıyor ama mevcut) |

## Temel fark

Mini projede her şey (CRUD, yetki, audit alanları, soft-delete) elle yazılıyordu. ABP'de bu altyapı hazır geliyor; geliştirici sadece **iş kuralına özgü kısmı** (Domain katmanındaki entity davranışları, invariant'lar) yazar. Katman ayrımı ise mini projede de kavramsal olarak vardı (model / servis / DbContext) — ABP bu ayrımı ayrı projelere ve net sözleşmelere (Contracts) taşıyor.
