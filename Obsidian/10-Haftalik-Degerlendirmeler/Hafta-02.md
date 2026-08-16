# 2. Hafta Değerlendirmesi

Tarih: 14 Ağustos 2026, Cuma (Gün 6 - Gün 10)

## Gereksinim analizinde öğrendiklerim

Aktörleri, senaryoları ve iş kurallarını netleştirmeden domain modeline geçmenin yanıltıcı olduğunu gördüm. `docs/analysis/business-rules.md`'deki kurallar, sonraki tüm günlerin (domain, veritabanı, tasarım sunumu) referans noktası oldu.

## Aggregate kararı verirken kullandığım ölçütler

Bir varlığın ayrı aggregate mi yoksa child entity mi olacağına, kendi başına bir yaşam döngüsü/tutarlılık sınırı olup olmadığına bakarak karar verdim. `MentorReview`'ı `DailyLog`'dan ayrı aggregate yaptım çünkü kendi onay/durum bilgisine ve bağımsız yaşam döngüsüne sahip; `DailyLogItem` gibi child'lar ise `DailyLog` olmadan anlamsız.

## Domain modeli ve veritabanı modeli farkı

Domain modelinde `DailyLog` ve child'ları tek bir bütün (aggregate) olarak davranır; veritabanında ise ayrı tablolar ve FK ilişkileriyle temsil edilir. `DateRange` domainde tek bir value object iken veritabanında iki ayrı kolon (`InternshipStartDate`, `InternshipEndDate`). Domain davranışla korunur, veritabanı constraint/FK ile korunur — ikisi birbirini tamamlar.

## En önemli normalizasyon kararım

`AppInternProfiles(UserId)` üzerindeki kısmi unique index (yalnızca `Status = Active` için) kararı. Uygulama kontrolü tek başına yarış durumuna karşı yetersiz kalıyordu; veritabanı seviyesinde garanti ekleyerek çift savunma sağladım.

## ABP katmanlarından anladığım

Katmanların tek yönlü bağımlılığı (Domain.Shared → Domain → Application.Contracts → Application → EntityFrameworkCore/Web) iş kuralını tek bir yerde (Domain) tutmayı zorunlu kılıyor. Application Service koordinasyon yapar ama kural içermez; PageModel ise sadece Application Service çağırır.

## Gelecek hafta uygulayacağım tasarım kararları

- Domain katmanında `DailyLog`, `MentorReview`, `InternProfile` aggregate'lerini, v2'de netleşen invariant ve kısıtlarla (kısmi unique, 1..* MentorReview, soft-delete) implemente edeceğim.
- İlk migration'ı `docs/database/table-catalog.md` ve `constraints.md`'ye birebir uyumlu şekilde oluşturacağım.
