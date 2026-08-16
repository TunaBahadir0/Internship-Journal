# İstek Akışı

## Genel akış

```text
Razor Page
   ↓
Application Service
   ↓
Domain davranışı
   ↓
Repository
   ↓
DbContext
   ↓
PostgreSQL
```

## Örnek senaryo: Günlük gönderme (DailyLog Submit)

Stajyer, taslak (`Draft`) durumundaki günlüğünü mentöre gönderir.

1. **Razor Page** — `Pages/DailyLogs/Index.cshtml` üzerindeki "Gönder" butonu, `SubmitAsync` handler'ını tetikler.
2. **PageModel** — `IndexModel.OnPostSubmitAsync(Guid id)`, doğrudan `DailyLogAppService.SubmitAsync(id)` çağırır. İş kuralı içermez.
3. **Application Service** — `DailyLogAppService.SubmitAsync`:
   - Yetki kontrolü yapar (`[Authorize(InternshipJournalPermissions.DailyLogs.Submit)]`).
   - `IDailyLogRepository` üzerinden aggregate'i (`DailyLog` + child'ları) yükler.
   - `dailyLog.Submit()` domain metodunu çağırır.
   - Repository ile günceller, sonucu `DailyLogDto`'ya çevirip döner.
4. **Domain davranışı** — `DailyLog.Submit()`:
   - I-4 (boş günlük gönderilemez) ve I-5 (geçerli durum geçişi: `Draft`/`RevisionRequested` → `Submitted`) invariant'larını kontrol eder.
   - `Status = Submitted`, `SubmittedAt = Clock.Now` set eder.
   - Kural ihlali varsa `BusinessException` fırlatır; Application Service bunu API/UI katmanına taşır.
5. **Repository** — `EfCoreDailyLogRepository`, aggregate'i (root + child koleksiyonları) tek transaction içinde günceller.
6. **DbContext** — `InternshipJournalDbContext`, `AppDailyLogs` ve ilişkili child tablolar için `UPDATE`/`INSERT` komutlarını üretir.
7. **PostgreSQL** — Değişiklik kalıcı hale gelir. `AppDailyLogs(InternProfileId, LogDate)` unique kısıtı ve FK'lar veritabanı seviyesinde korunur.

## Not

Bu akışta domain kuralı (Submit için geçerli durum kontrolü) **yalnızca Domain katmanında** uygulanır; Application Service ve PageModel kuralın kendisini tekrar etmez, sadece çağırır. Bu, `docs/decisions/design-review.md`'de savunulan "iş kuralı tek yerde yaşar" ilkesiyle uyumludur.
