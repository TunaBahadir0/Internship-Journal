# Normalizasyon

Bu doküman, veritabanını neden ve nasıl tablolara böldüğümüzü **öğretici** biçimde anlatır. Amaç: tekrar (aynı verinin birçok yerde tutulması) ve tutarsızlık (aynı bilginin farklı yerlerde çelişmesi) olmayan bir tasarıma ulaşmak.

---

## 0. Kötü başlangıç: Her şey tek tabloda

Diyelim ki tüm veriyi tek bir tabloda tutuyoruz:

```text
InternshipJournal
-----------------
InternName, InternEmail, MentorName, WorkplaceName,
CountryName, ProvinceName, DistrictName, AddressLine,
LogDate, WorkItem1, WorkItem2, Skill1, Skill2, Problem1, MentorComment
```

Bu tasarım birçok sorun içerir. Onları normalizasyon kurallarıyla (Normal Form) tek tek çözeceğiz.

---

## 1. Birinci Normal Form (1NF) — Atomiklik ve tekrarlayan gruplar

**Kural:** Her hücre tek (atomik) bir değer içermeli; `WorkItem1, WorkItem2` gibi tekrarlayan kolon grupları olmamalı.

**Sorun:**
- `WorkItem1`, `WorkItem2`: Bir günde 3 iş yapılırsa nereye yazacağız? Tablo genişleyemez.
- `Skill1`, `Skill2`: Aynı sorun; ölçeklenmez.
- Tek hücrede birden fazla yetkinlik tutmak (`"Docker, PostgreSQL"`) sorgulamayı imkânsızlaştırır ("Docker çalışılan günlükleri getir" yazılamaz).

**Çözüm:** Tekrarlayan grupları **ayrı satırlara** taşı. Yani çalışma maddeleri ve yetkinlikler kendi tablolarına gider; her iş/yetkinlik ayrı bir satır olur:

```text
AppDailyLogItems:  her satır = bir çalışma maddesi (DailyLogId ile günlüğe bağlı)
AppDailyLogSkills: her satır = bir yetkinlik (DailyLogId + SkillId ile bağlı)
```

Artık bir güne istediğin kadar madde/yetkinlik eklenebilir ve sorgulanabilir.

---

## 2. İkinci Normal Form (2NF) — Bileşik anahtara tam bağımlılık

**Kural:** Tablo 1NF olmalı ve anahtar olmayan her alan, birincil anahtarın **tamamına** bağlı olmalı (bileşik anahtarın yalnızca bir parçasına değil).

**Nerede karşımıza çıkar:** Bağlantı (junction) tablolarında. Örneğin bir günlük ile yetkinlik arasındaki ilişkiyi tutan `AppDailyLogSkills`:

- Bu tabloda ilişkiye ait alanlar (öğrenme seviyesi, not) bulunmalı.
- Yetkinliğin adı/kategorisi burada **tutulmamalı** — çünkü bunlar yalnızca `SkillId`'ye bağlıdır, ilişkinin tamamına değil. Yetkinlik bilgisi `AppSkills` tablosunda durur; burada sadece `SkillId` referansı olur.

Böylece yetkinlik adı değişirse tek yerde (AppSkills) değişir, her ilişkide tekrar güncellenmez.

---

## 3. Üçüncü Normal Form (3NF) — Geçişli bağımlılık yok

**Kural:** Tablo 2NF olmalı ve anahtar olmayan bir alan, başka bir anahtar olmayan alana bağlı olmamalı (geçişli bağımlılık olmamalı).

**Adres örneği — en önemli kısım:** İlçe, il ve ülke arasında şu bağımlılık vardır:

```text
District → Province → Country
```

Yani ilçeyi bilirsen ilini, ilini bilirsen ülkesini bilirsin. Eğer `AppWorkplaces` tablosunda `CountryName`, `ProvinceName`, `DistrictName` alanlarını birlikte tutarsak:

- Aynı bilgi birçok çalışma yerinde tekrarlanır.
- Daha kötüsü, **çelişki** oluşabilir: "Ülke: Türkiye, İl: Ankara, İlçe: Kadıköy" gibi hatalı bir kombinasyon girilebilir (Kadıköy aslında İstanbul'a bağlıdır).

**Çözüm:** Konumu üç ayrı tabloya böl ve çalışma yerinde **yalnızca `DistrictId`** tut:

```text
AppWorkplaces.DistrictId → AppDistricts.Id
AppDistricts.ProvinceId  → AppProvinces.Id
AppProvinces.CountryId   → AppCountries.Id
```

Artık il ve ülke, ilişkiden **türetilir**; ayrıca saklanmadığı için çelişemez. Bu, adres hiyerarşisinin tutarlılığını veritabanı seviyesinde garanti eder.

---

## 4. Çok-çoğa ilişkiyi çözmek

Bir günlükte birden çok yetkinlik olabilir; bir yetkinlik birçok günlükte kullanılabilir. Bu bir **çok-çoğa (many-to-many)** ilişkidir ve doğrudan tek tabloyla tutulamaz.

**Çözüm:** Araya bir **bağlantı tablosu** koyarız: `AppDailyLogSkills`. Bu tablo, hangi günlükte hangi yetkinliğin çalışıldığını (ve seviyesini/notunu) satır satır tutar:

```text
DailyLog *───1 AppDailyLogSkills 1───* Skill
```

Aynı yetkinliğin bir günlüğe iki kez eklenmemesi için `(DailyLogId, SkillId)` benzersiz yapılır (bkz. constraints.md).

---

## 5. Sonuç: 11 uygulama tablosu

Normalizasyon sonrası tablolar:

**Konum (referans):**
`AppCountries`, `AppProvinces`, `AppDistricts`

**Ana akış:**
`AppWorkplaces`, `AppInternProfiles`, `AppSkills`,
`AppDailyLogs`, `AppDailyLogItems`, `AppDailyLogSkills`,
`AppProblemSolvingEntries`, `AppMentorReviews`

Ayrıntılı kolonlar `table-catalog.md`, kurallar `constraints.md`, hız için indeksler `indexes.md`, görsel ilişki `erd-v1.png` dosyalarındadır.

---

## 6. Domain modeli ile ER modeli neden birebir aynı değil?

- Domain modelinde `DailyLog` + child'ları **tek bir bütün** (aggregate) gibi düşünülür; veritabanında bunlar **ayrı tablolar** olur (`AppDailyLogs`, `AppDailyLogItems`...).
- Value Object olan `DateRange`'in domainde kimliği yok; veritabanında `AppInternProfiles` içinde iki kolon (`InternshipStartDate`, `InternshipEndDate`) olarak durur.
- Domain davranışlarla (metotlarla) bağlanırken, veritabanı **foreign key** ve **constraint** ile bağlanır.

Yani domain modeli "davranış ve kural" odaklı; ER modeli "veri saklama ve bütünlük" odaklıdır. İkisi ilişkilidir ama aynı değildir.
