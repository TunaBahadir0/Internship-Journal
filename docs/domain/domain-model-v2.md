# Domain Modeli — v2 (Kontrol Kapısından Geçmiş)

Bu, tasarım sunumundaki geri bildirimlerle güncellenmiş domain modelidir. v1'e göre değişen noktalar **[v2]** ile işaretlenmiştir. v1'in temel yapısı (DailyLog ana aggregate, MentorReview ayrı aggregate, konum referans aggregate'leri) korunmuştur.

Ayrıntılı v1 anlatımı için `domain-model.md`; bu doküman v1'in üstüne farkları koyar.

---

## 1. Sınıflandırma (değişmedi)

| Kavram | Rol |
|---|---|
| Country, Province, District | Aggregate Root (referans) |
| Workplace, InternProfile, Skill | Aggregate Root (bağımsız) |
| DailyLog | Aggregate Root (ana) |
| DailyLogItem, DailyLogSkill, ProblemSolvingEntry | Child Entity (DailyLog'a bağlı) |
| MentorReview | Aggregate Root (ayrı) |
| DateRange | Value Object |

---

## 2. v2 değişiklikleri

### [v2-1] InternProfile — tek aktif profil garantisi
- **Değişiklik:** "Bir kullanıcı için tek aktif profil" kuralı artık yalnızca Domain Service'te değil, veritabanında da korunuyor: `AppInternProfiles(UserId)` üzerinde **kısmi unique index** (`Status = Active` için).
- **Etki:** InternProfileManager kontrolü + DB kısmi unique = çift savunma.
- **Kural:** BR-8.

### [v2-2] MentorReview — 1..* kardinalite
- **Değişiklik:** DailyLog ↔ MentorReview ilişkisi **bire-çok** olarak netleştirildi. Bir günlük, düzeltme döngüsü boyunca birden çok kez incelenebilir; her inceleme ayrı `MentorReview` kaydıdır.
- **Etki:** En güncel karar `ReviewedAt` en büyük olan incelemedir. İnceleme geçmişi denetlenebilir.
- **Kural:** BR-23..BR-26 korunur.

### [v2-3] Soft-delete stratejisi
- **Değişiklik:** ABP varsayılanı olan **soft-delete** (IsDeleted) benimsendi. "Kullanılan konum/çalışma yeri/yetkinlik silinmez" kuralı soft-delete + restrict FK ile desteklenir.
- **Etki:** Fiziksel silme yerine pasifleştirme/soft-delete; child'lar aggregate ile birlikte ele alınır.
- **Kural:** BR-6.

### [v2-4] WorkType enum hizalama
- **Değişiklik:** DailyLogItem.WorkType, ana proje enum'una göre **8 değerli**: Setup, Training, Development, Research, Testing, Documentation, Meeting, ProblemSolving.
- **Etki:** Rapor ve ortak dil tutarlılığı.

---

## 3. Aggregate'ler ve dış referanslar (v2 net hali)

```text
[DailyLog] (root)
   ├── DailyLogItem   (child, cascade)
   ├── DailyLogSkill  (child, cascade)  ── SkillId ile Skill'e Id referansı
   └── ProblemSolvingEntry (child, cascade)
   └── InternProfileId ile InternProfile'a Id referansı

[MentorReview] (root, ayrı)
   └── DailyLogId, MentorUserId ile Id referansları
   └── Bir DailyLog için 1..* MentorReview (v2)

[InternProfile] (root)
   └── UserId (aktif tekil - v2 kısmi unique), MentorUserId, WorkplaceId
   └── DateRange (staj dönemi, value object)

[Workplace] (root) ── DistrictId
[Country] 1..* [Province] 1..* [District]  (referans zinciri)
[Skill] (root, referans)
```

---

## 4. Korunan invariant'lar (özet)

- Toplam süre = maddelerin toplamı (I-1).
- Aynı yetkinlik bir günlükte tek kez (I-2).
- Yalnızca Draft/RevisionRequested düzenlenebilir; Approved değişmez (I-3).
- Boş günlük gönderilemez (I-4).
- Geçerli durum geçişleri (I-5).
- Madde başlığı dolu, süre > 0 (I-6).
- Staj dönemi geçerli aralık (I-7), profil çalışma yeri+mentora bağlı (I-8).

---

## 5. Domain ↔ veritabanı farkı (hatırlatma)

- Domain'de DailyLog + child'ları tek bütün; veritabanında ayrı tablolar (`AppDailyLogs`, `AppDailyLogItems`...).
- DateRange domainde value object; veritabanında iki kolon.
- Domain davranışla, veritabanı FK/constraint ile bağlanır.

Bu v2 modeli **kod geliştirmeye geçiş için onaylanmıştır** (bkz. `docs/decisions/design-review.md`).
