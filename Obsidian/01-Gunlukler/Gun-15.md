# Gün 15

Tarih: 24 Ağustos 2026, Pazartesi

## Bugün tamamladığım işler

- `DailyLog`, `DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry` için EF Core mapping'i `InternshipJournalDbContext.OnModelCreating` içine yazdım: `AppDailyLogs`, `AppDailyLogItems`, `AppDailyLogSkills`, `AppProblemSolvingEntries` tabloları; dört DbSet eklendi.
- Child koleksiyonları (`Items`/`Skills`/`Problems`) `HasMany(...).WithOne().HasForeignKey("DailyLogId")` ile **gölge (shadow) FK** üzerinden, `UsePropertyAccessMode(PropertyAccessMode.Field)` ile de private backing field'lar (`_items`/`_skills`/`_problems`) üzerinden maplendi — child entity'lerin genel (public) bir `DailyLogId` alanı yok, bu tamamen EF Core'un iç muhasebesi.
- Silme davranışını `docs/database/constraints.md`'deki karara birebir uydurdum: `AppDailyLogs.InternProfileId` → Restrict (farklı aggregate), üç child tablonun `DailyLogId`'si → Cascade (aynı aggregate), `AppDailyLogSkills.SkillId` → Restrict (Skill kendi başına yaşayan bir referans veri, günlük silinince silinmemeli).
- `(InternProfileId, LogDate)` ve `(DailyLogId, SkillId)` unique index'lerini, `Status` ve `SkillId` üzerinde de sorgu-odaklı indeksleri ekledim.
- `dotnet ef migrations add Added_DailyLog_Module` ile migration'ı ürettim (EntityFrameworkCore projesini hem `--project` hem `--startup-project` olarak kullandım; `DbMigrator`'da `Microsoft.EntityFrameworkCore.Design` paketi yok). Üretilen dosyayı satır satır okuyup `docs/database/table-catalog.md` ve `constraints.md`'de Gün 8'de önceden tasarlanmış planla karşılaştırdım — FK'ler, cascade/restrict davranışları, unique constraint'ler ve indeksler beklenen planla birebir eşleşti.
- `IDailyLogRepository`'yi implemente ettim (`DailyLogRepository`): `ExistsForDateAsync`, `GetByInternAndDateAsync`, `GetWithDetailsAsync` (Items/Skills/Problems Include'lu — tekil detay için), `GetListWithDetailsAsync` (Include'suz — liste ekranı için; imzasına `startDate`/`endDate`/`keyword` parametrelerini de ekledim, Gün 14'te henüz bilinmeyen liste filtreleme ihtiyacı netleşince).
- Contracts: `DailyLogDto` (liste — özet alanlar), `DailyLogDetailDto` (detay — `InternProfileId` + üç child koleksiyon), `DailyLogItemDto`, `DailyLogSkillDto`, `ProblemSolvingEntryDto`, `CreateDailyLogDto`, `UpdateDailyLogSummaryDto`, `GetDailyLogListInput`, ve gelecek günler için hazırlanan `Add/UpdateDailyLogItemInput`, `Add/UpdateDailyLogSkillInput`, `Add/UpdateProblemSolvingEntryInput`, `IDailyLogAppService`.
- `DailyLogAppService`'i yazdım — yalnızca müfredatın istediği 4 başlangıç metodu: `GetAsync`, `GetListAsync`, `CreateAsync`, `UpdateSummaryAsync`. Veri sahipliği: `GetListAsync`/`CreateAsync` her zaman `CurrentUser.Id → GetActiveByUserIdAsync → InternProfileId` zincirini kendi içinde çözüyor; `GetDailyLogListInput`'ta `InternProfileId` alanı yok, stajyer başkasının günlüğünü ID vererek listeleyemiyor.
- `InternshipJournalApplicationMappers`'a Mapperly partial metotları ekledim (`MapToDailyLogDto`, `MapToDailyLogDetailDto`, üç child DTO için `Map` overload'ları).
- Testler: `DailyLogAppServiceTests` (8 test, müfredattaki isimlerle birebir) + `EfCoreDailyLogAppServiceTests` (SQLite üzerinden çalıştıran somut sınıf). Tüm EntityFrameworkCore.Tests paketi: 31/31 geçiyor. Tam çözüm derlemesi 0 hata.
- `docs/database/indexes.md`'ye "Gün 15 doğrulaması" bölümünü ekledim: hangi indekslerin migration'da birebir oluştuğunu, hangi ikisinin (WorkType, AI boolean) bilinçli olarak ertelendiğini yazdım.

## Öğrendiğim / pekiştirdiğim konular

- **Gölge FK (shadow foreign key):** Child entity'nin (`DailyLogItem` vb.) kendi domain modelinde `DailyLogId` alanı yok — bu saf bir domain kararı (child, kendi aggregate'ine "geri referans" tutmuyor). EF Core'a `.HasForeignKey("DailyLogId")` ile string verince, EF bu kolonu veritabanında oluşturuyor ama hiçbir C# sınıfında karşılığı yok. Domain modeli ile veritabanı modelinin illa birebir aynı alanlara sahip olması gerekmediğini somut gördüm.
- **`PropertyAccessMode.Field` gerekliliği:** `Items` property'si `IReadOnlyCollection<DailyLogItem>` (yalnızca get), gerçek liste `_items` private field'ında. EF Core varsayılan olarak property üzerinden (get/set) yazmaya çalışır; `Items`'in set'i olmadığı için `UsePropertyAccessMode(PropertyAccessMode.Field)` demeden migration'ı uygulasam bile runtime'da materyalize etme hatası alırdım. Bunu ilk denemede unuttum, `LocationSeedTests`'teki `IncludeDetails` deseniyle karşılaştırırken fark edip ekledim.
- **Testte `GetAsync` ile `GetWithDetailsAsync` farkının cezası:** `SubmitAndApproveAsync` test yardımcı metodumda ilk yazımda düz `_dailyLogRepository.GetAsync(id)` kullandım; `Items` Include edilmediği için `Submit()` "en az bir madde olmalı" hatasını attı — halbuki madde az önce eklenmişti. Kök nedeni (ABP'nin varsayılan `GetAsync`'i, ben `WithDetails()`'i override etmediğim için collection'ları otomatik yüklemiyor) stack trace'teki gerçek satırı (211: `DailyLogMustHaveAtLeastOneItem`, Status kontrolü değil) okuyarak buldum, varsayımla geçmedim.
- **Restrict/Cascade kararının aggregate sınırıyla ilişkisi:** `DailyLogSkill.SkillId` neden Restrict de `DailyLogId` neden Cascade sorusunun cevabı aynı ilke: cascade yalnızca "bu obje aggregate'in dışında anlamsız" ilişkilerde (child→parent) kullanılır; `Skill` günlükten bağımsız yaşayan bir referans veri olduğu için oraya cascade **asla** uygulanmaz, aksi halde bir günlük silindiğinde yetkinlik kataloğundan kayıt gitmiş olurdu.
- **`dotnet ef migrations add` için doğru başlangıç projesi:** `DbMigrator`'ı startup-project vermek `Microsoft.EntityFrameworkCore.Design` paketi eksik olduğu için başarısız oldu; `EntityFrameworkCore` projesinin kendisi bu paketi zaten taşıdığı (design-time factory için) için hem `--project` hem `--startup-project` olarak onu vermek işe yaradı.

## Alınan kararlar

1. `GetListWithDetailsAsync`'in imzasını Gün 14'teki haliyle bırakmadım; `startDate`/`endDate`/`keyword` parametrelerini ekledim. Gün 14'te bu metot yalnızca arayüz olarak tanımlanmıştı ve gerçek liste filtreleme ihtiyacı (Gün 15'in `GetDailyLogListInput` alanları) o an netleşmemişti — Gün 13'te `InternProfileRepository.GetListWithDetailsAsync`'e sonradan `filter` parametresi eklenmesiyle aynı emsal.
2. Liste sorgusunda (`GetListWithDetailsAsync`) child collection'lar bilerek Include edilmiyor — metodun adı "WithDetails" olsa da, Gün 15 metninin "Liste sorgusunda child collection'ların tamamı yüklenmez" kuralı burada isme değil, gerçek kurala uyuldu; koda açıklayıcı bir yorum bıraktım.
3. `DailyLogItem(WorkType)` ve `ProblemSolvingEntry(UsedArtificialIntelligence)` indekslerini eklemedim — ikisi de düşük seçicilikte enum/boolean kolonlar ve gerçek bir sorgu ihtiyacına karşılık gelmiyor; `docs/database/indexes.md`'nin kendi "çok fazla indeksin maliyeti" ilkesiyle çelişirdi. Gün 15 müfredat metni bunları listeliyor olsa da, daha önceki (Gün 8) tasarım incelemesinin gerekçeli kararını (`indexes.md`'de zaten "(opsiyonel)" işaretliydi) daha güçlü kanıt kabul ettim.
4. `DailyLogAppService.CreateAsync`/`GetListAsync`, `GetDailyLogListInput`'a bilerek `InternProfileId` alanı eklemedi; CurrentUser'dan içeride çözülüyor. Bu, veri sahipliği kuralını DTO seviyesinde de garanti altına alıyor — istemci taraf yanlışlıkla ya da kasıtlı olarak başka bir `InternProfileId` gönderemez.

## Yapay zekâ kullanımı

Migration dosyasını üretmeden önce hangi ilişkilerin Cascade, hangilerinin Restrict olacağını varsayımla yazmadım; `docs/database/table-catalog.md` ve `constraints.md`'yi (Gün 8'de yazılmış, henüz kod yokken hazırlanmış tasarım dokümanları) açıp satır satır okuyarak doğruladım — özellikle `DailyLogSkill.SkillId → Restrict` kararını ilk planımda atlamıştım, doküman karşılaştırması sırasında yakaladım ve mapping'e ekledim.

## Gün 15 hedefleri — durum

- [x] Aggregate ve child entity mapping yazıldı
- [x] Unique constraint ve indeks oluşturuldu
- [x] Cascade davranışı incelendi ve `docs/database/constraints.md`'ye uygun uygulandı
- [x] Migration dosyası okundu, FK/cascade/unique/indeks satırları doğrulandı
- [x] Liste ve detay DTO'ları ayrıldı (`DailyLogDto` / `DailyLogDetailDto`)
- [x] Temel Application Service metotları (`GetAsync`, `GetListAsync`, `CreateAsync`, `UpdateSummaryAsync`) geliştirildi
- [x] Veri sahipliği filtresi hazırlandı (CurrentUser → aktif InternProfile)
- [x] Application Service testleri yazıldı (8/8, isimler müfredatla birebir)

## Yarın yapacaklarım

- 3. Hafta tamamlandı — haftalık değerlendirme `Obsidian/10-Haftalik-Degerlendirmeler/Hafta-03.md` dosyasına yazıldı.
- 4. Hafta: `AddItem`/`UpdateItem`/`RemoveItem`, `AddSkill`/`AddProblem` gibi kalan `DailyLogAppService` metotları, durum geçişi uçtan uca akışı (`Submit`/`RequestRevision`/`Approve` App Service seviyesinde), ayrı bir aggregate olarak planlanan `MentorReview`.
