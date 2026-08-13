# Constraint'ler (Kısıtlar)

Bu doküman, veri bütünlüğünü **veritabanı seviyesinde** koruyan kuralları listeler. Bir kuralı yalnızca uygulama kodunda kontrol etmek yetmez; iki istek aynı anda gelirse (race condition) uygulama kontrolü kaçabilir. Veritabanı constraint'i son ve kesin savunmadır.

---

## 1. Primary Key'ler

Her tabloda `Id` (uuid) birincil anahtardır. Her satırı benzersiz ve tekil olarak tanımlar.

---

## 2. Unique Constraint'ler

| Tablo | Unique alan(lar) | Gerekçe | İlgili kural |
|---|---|---|---|
| AppCountries | Code | Aynı ülke kodu iki kez olmamalı | BR-28 |
| AppProvinces | (CountryId, Name) | Aynı ülkede aynı il adı tekrar etmesin | BR-28 |
| AppDistricts | (ProvinceId, Name) | Aynı ilde aynı ilçe adı tekrar etmesin | BR-28 |
| AppSkills | Name | Katalogda yetkinlik tekrarı olmasın | — |
| AppDailyLogs | (InternProfileId, LogDate) | Aynı stajyer aynı gün için tek günlük | BR-1 |
| AppDailyLogSkills | (DailyLogId, SkillId) | Aynı yetkinlik bir günlüğe tek kez | BR-5 |

**Özel durum — tek aktif profil (BR-8):** "Bir kullanıcı için tek aktif profil" kuralı, klasik unique ile tam ifade edilemez (pasif profiller birden çok olabilir). İki yol var:
- **Kısmi (filtered) unique index:** `UserId` üzerinde yalnızca `Status = Active` satırları için benzersizlik.
- **Domain Service kontrolü:** InternProfileManager oluşturma anında kontrol eder.
Öneri: İkisi birlikte (servis kontrolü + mümkünse kısmi unique).

---

## 3. Foreign Key'ler

| Tablo.Kolon | Referans | Silme davranışı |
|---|---|---|
| AppProvinces.CountryId | AppCountries.Id | Restrict |
| AppDistricts.ProvinceId | AppProvinces.Id | Restrict |
| AppWorkplaces.DistrictId | AppDistricts.Id | Restrict |
| AppInternProfiles.WorkplaceId | AppWorkplaces.Id | Restrict |
| AppDailyLogs.InternProfileId | AppInternProfiles.Id | Restrict |
| AppDailyLogItems.DailyLogId | AppDailyLogs.Id | Cascade |
| AppDailyLogSkills.DailyLogId | AppDailyLogs.Id | Cascade |
| AppDailyLogSkills.SkillId | AppSkills.Id | Restrict |
| AppProblemSolvingEntries.DailyLogId | AppDailyLogs.Id | Cascade |
| AppMentorReviews.DailyLogId | AppDailyLogs.Id | Restrict |

---

## 4. Silme davranışı gerekçeleri (Cascade vs Restrict)

- **Restrict (referans korunur):** Kullanılan referans veri silinemez. Örneğin bir ülkeye bağlı iller varken ülke silinemez; bir çalışma yerine bağlı stajyer varken çalışma yeri silinemez. Bu, geçmiş kayıtların bozulmasını önler. Konum/çalışma yeri gibi veriler silmek yerine **pasifleştirilir** (IsActive = false).
- **Cascade (birlikte silinir):** Bir aggregate root silindiğinde child'ları da silinir. `AppDailyLogs` silinirse maddeleri, yetkinlikleri ve problemleri de silinir — çünkü bunlar günlük olmadan anlamsızdır (aynı aggregate).
- **MentorReview neden Restrict?** Ayrı bir aggregate olduğu için günlükle otomatik yok edilmez; incelemeler denetim kaydı niteliğindedir. (Uygulamada günlük silme zaten çoğunlukla soft-delete ile yapılır — ABP `IsDeleted`.)

> Not: ABP varsayılan olarak **soft delete** kullanır (kayıt fiziksel silinmez, `IsDeleted = true` olur). Bu, "kullanılan konum silinmemeli" (BR-6) kuralını da destekler.

---

## 5. Check / Domain kuralları (veritabanı + uygulama)

Bazı kurallar tek bir kolon kısıtıyla ifade edilebilir, bazıları uygulama/domain tarafında korunur:

| Kural | Nasıl korunur |
|---|---|
| DurationMinutes > 0 | Check constraint veya domain (BR-16) |
| InternshipStartDate ≤ InternshipEndDate | Domain (DateRange value object, BR-7) |
| Email formatı | Domain/validasyon (BR-14) |
| TotalMinutes = maddeler toplamı | Domain invariant (I-1); DB'de türetilmiş değer |
| Onaylı günlük değişmez | Domain (durum kontrolü, BR-22) |

Genel ilke: **Yapısal/tekillik kuralları** veritabanında (unique/FK), **davranışsal/durum kuralları** domain'de korunur.
