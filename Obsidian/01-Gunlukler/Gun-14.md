# Gün 14

Tarih: 24 Ağustos 2026, Pazartesi

## Bugün tamamladığım işler

- `DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry` child entity'lerini (`Entity<Guid>`) yazdım: hepsi `internal` constructor + `internal Update` metoduyla yalnızca `DailyLog` aggregate'i (aynı assembly) tarafından oluşturulup değiştirilebiliyor; dışarıdan (Application katmanından) doğrudan `new` veya `Update` çağrısı yapılamıyor.
- `DailyLog` aggregate root'unu (`FullAuditedAggregateRoot<Guid>`) yazdım: InternProfileId, LogDate, Summary, TotalMinutes, Status, SubmittedAt, ReviewedAt, ApprovedAt alanları; `Items`/`Skills`/`Problems` koleksiyonları `IReadOnlyCollection` olarak dışarı açılıyor, backing field'lar (`_items`/`_skills`/`_problems`) private.
- Davranışlar: `ChangeSummary`, `AddItem`/`UpdateItem`/`RemoveItem`, `AddSkill`/`UpdateSkill`/`RemoveSkill`, `AddProblem`/`UpdateProblem`/`RemoveProblem`, `Submit`, `Approve`, `RequestRevision`, `ReturnToDraft`.
- Durum geçişini `Draft → Submit → Submitted → (Approve → Approved | RequestRevision → RevisionRequested → ReturnToDraft → Draft)` şeklinde uyguladım; tekrarlanan "düzenlenebilir mi" kontrolünü `EnsureEditable` private metoduna aldım (yalnızca Submitted/Approved düzenlemeyi engelliyor, RevisionRequested tekrar düzenlenebilir durumda).
- `TotalMinutes`'i kullanıcı doğrudan değiştiremiyor; `AddItem`/`UpdateItem`/`RemoveItem` sonrasında `RecalculateTotalMinutes` ile `_items.Sum(x => x.DurationMinutes)` üzerinden otomatik yeniden hesaplanıyor.
- `ProblemSolvingEntry` içinde AI kurallarını uyguladım: `UsedArtificialIntelligence = true` iken `AiToolName`/`AiPromptSummary` boş olamaz; `AiSuggestionAccepted = false` (öneri reddedildi) iken `AiRejectionReason` boş olamaz.
- `IDailyLogRepository` arayüzünü yazdım (bu gün yalnızca arayüz; EF Core implementasyonu ve migration Gün 15'te): `ExistsForDateAsync`, `GetByInternAndDateAsync`, `GetWithDetailsAsync`, `GetListWithDetailsAsync`.
- `DailyLogManager` domain servisini yazdım — cross-aggregate kontroller: staj profili var mı, profil Active mi, günlük tarihi gelecekte mi (`IClock` ile), tarih staj dönemi içinde mi (`InternshipPeriod.Contains`), aynı tarihli günlük zaten var mı (`ExistsForDateAsync`); ayrıca `AddSkillAsync` ile eklenmek istenen `Skill`'in var/aktif olduğunu kontrol ediyor.
- `InternshipJournalDomainErrorCodes`'a 16 yeni hata kodu ekledim (`DailyLogDateCannotBeInFuture`, `DailyLogInternProfileNotActive`, `DailyLogCannotBeApproved`, `ProblemSolvingAiToolAndSummaryRequired` vb.); Gün 13'ten kalan 5 kod (`DuplicateDailyLog`, `DuplicateDailyLogSkill`, `DailyLogCannotBeEdited`, `DailyLogCannotBeSubmitted`, `MentorIsNotAssigned`) doğrudan kullanıldı, tekrar tanımlanmadı.
- `DailyLogSkillConsts.MaxNoteLength` sabitini ekledim (diğer üç Consts dosyası zaten iskelette hazırdı).
- Testler: `DailyLogTests` (17 saf unit test — DI gerekmiyor, `internal` constructor'a `InternalsVisibleTo` ile doğrudan erişiliyor) + `DailyLogManagerTests` (7 test — `IDailyLogRepository`/`IInternProfileRepository`/`IRepository<Skill,Guid>`/`IClock` NSubstitute ile mock'landı). Toplam 24 yeni test; Domain.Tests paketi tamamı: 31/31 geçiyor.

## Öğrendiğim / pekiştirdiğim konular

- **Child entity'lerin `internal` kurucu/mutator ile korunması:** `DailyLogItem` gibi child entity'lerin constructor'ı ve `Update` metodu `internal` olunca, Application katmanı (ayrı assembly) bunları doğrudan çağıramıyor — tek yol aggregate root'un `AddItem`/`UpdateItem` gibi kontrollü metotları. Aynı assembly içinden teknik olarak yine erişilebilir, ama gerçek sınır katman (assembly) sınırında kuruluyor.
- **`TotalMinutes`'in neden public setter'ı yok:** Eğer dışarıdan set edilebilseydi, `_items`'in toplamıyla tutarsız bir değer yazılabilirdi (örn. madde silinmeden toplam değişmeden kalması). Tek yazma noktasını (`RecalculateTotalMinutes`) private tutup yalnızca ilgili üç metottan tetiklemek, hesaplanan alanın her zaman gerçek kaynaktan (`_items`) türediğini garanti ediyor.
- **Aynı tarih kontrolünün neden aggregate içinde yapılamayacağı:** `DailyLog`'un kendisi, aynı `InternProfileId` için başka hangi günlüklerin var olduğunu bilemez (aggregate'ler birbirinin verisine doğrudan erişemez) — bu yalnızca repository'ye sorgu atabilen `DailyLogManager` seviyesinde, veritabanına bakarak yapılabilir.
- **`DomainService.GuidGenerator`'ın DI olmadan `NullReferenceException` vermesi:** `DailyLogManager`'ı testte `new DailyLogManager(...)` ile doğrudan kurunca, ABP'nin `GuidGenerator`/`Clock` gibi property'leri `LazyServiceProvider` üzerinden lazy çözüldüğü için (DI container yokken) null referans hatası verdi. Çözüm: aggregate id'leri için zaten kullandığımız `Guid.NewGuid()` deseniyle tutarlı kalıp, Manager'da da `GuidGenerator.Create()` yerine `Guid.NewGuid()` kullandım; `Clock` ise constructor'a açıkça enjekte edilen bir parametre olduğu için testte NSubstitute ile sorunsuz mock'landı.
- **Repository mock'lamanın hangi kuralları ölçtüğü:** `AddSkill_WhenSkillInactive_ShouldFail` gibi testler, Skill'in gerçekten var olup olmadığını değil, "Manager pasif bir Skill döndüğünde doğru hata kodunu fırlatıyor mu" kuralını ölçüyor — yani iş kuralının kendisini, veritabanı sorgusunun doğruluğunu değil (o, Gün 15'te EF Core implementasyonu geldiğinde entegrasyon testiyle ölçülecek).

## Alınan kararlar

1. Child entity id'leri (`DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry`) aggregate içinde `Guid.NewGuid()` ile üretiliyor; aggregate'lerin kendi içinde DI servisi (`IGuidGenerator`) olmadığından bu en basit ve tutarlı çözüm — Gün 13'teki `InternProfile`'ın aksine (o `Manager` üzerinden `GuidGenerator.Create()` kullanıyordu) çünkü child entity oluşturma noktası doğrudan aggregate metotları, Manager değil.
2. `EnsureEditable`, müfredatın verdiği örnek koda birebir uyularak yalnızca `Submitted`/`Approved` durumlarını engelliyor — `RevisionRequested` durumundaki bir günlük tekrar düzenlenebilir kabul edildi (metnin "düzeltme istenen günlük tekrar taslağa alınarak düzenlenir" ifadesiyle tutarlı; taslağa dönmeden de düzenlemeye izin vermek, mentor geri bildirimini hemen işlemeye başlamayı kolaylaştırıyor).
3. `Submit` yalnızca `Draft` durumundan yapılabiliyor (durum diyagramındaki tek ok buradan çıkıyor); `RevisionRequested`'ten tekrar gönderim için önce `ReturnToDraft` sonra `Submit` çağrılması gerekiyor — bu iki adımlı akış, "düzeltme istendiğinde günlük gerçekten taslağa dönüyor" davranışını açıkça görünür kılıyor.
4. `AiSuggestionAccepted` `bool?` (nullable) olarak modellendi: `null` = AI'dan öneri istenmedi/yok, `true` = kabul edildi, `false` = reddedildi. Yalnızca `false` durumunda ret gerekçesi zorunlu; bu üç durumu ayırt etmek için `bool` yetmezdi.

## Yapay zekâ kullanımı

`DomainService.GuidGenerator` özelliğinin DI container olmadan neden `NullReferenceException` verdiğini varsayımla geçmedim — testi önce olduğu gibi çalıştırıp gerçek stack trace'i (`get_GuidGenerator()` içinde null referans) inceleyerek kök nedeni doğruladım, sonra `Guid.NewGuid()`'e geçip testi tekrar çalıştırarak düzeldiğini teyit ettim.

## Kabul kriteri kontrolü

- [x] Child entity'ler dışarıdan değiştirilemiyor
- [x] Toplam süre otomatik hesaplanıyor
- [x] Durum geçişleri kontrollü
- [x] Duplicate skill engelleniyor
- [x] Gönderim kuralları uygulanıyor
- [x] AI kullanım kuralları uygulanıyor
- [x] En az 15 domain testi geçiyor (24 yeni test yazıldı, toplam paket 31/31 geçiyor)

## Yarın yapacaklarım

- Gün 15: EF Core Mapping, Migration ve Application Contracts — `DailyLog`/`DailyLogItem`/`DailyLogSkill`/`ProblemSolvingEntry` için EF Core konfigürasyonu (child entity'ler owned/ilişkili tablo olarak), migration, `IDailyLogRepository`'nin gerçek implementasyonu ve DTO/AppService sözleşmeleri.
