# Domain Modeli

Bu doküman, Staj Günlüğü uygulamasının domain (iş alanı) modelini tanımlar. Amaç, tabloları değil; iş kurallarını doğru sınırlar içinde koruyan nesneleri ve ilişkilerini belirlemektir. **Domain modeli ile veritabanı modeli aynı şey değildir**; burada nesne davranışına ve sınırlarına odaklanıyoruz.

---

## 1. Entity ve Value Object farkı

- **Entity (varlık):** Kimliği (Id) olan, zaman içinde değişebilen nesne. İki entity aynı alanlara sahip olsa bile kimlikleri farklıysa farklı nesnedir. Örn: iki farklı `DailyLog` aynı özete sahip olabilir ama ayrı günlüklerdir.
- **Value Object (değer nesnesi):** Kimliği olmayan, yalnızca değeriyle tanımlanan nesne. Aynı değerlere sahip iki value object birbirinin aynısıdır ve değişmez (immutable). Örn: `DateRange` (başlangıç–bitiş).

---

## 2. Kavramların sınıflandırması

| Kavram | Entity | Aggregate Root | Child Entity | Value Object | Gerekçe |
|---|:---:|:---:|:---:|:---:|---|
| Country | ✓ | ✓ | | | Bağımsız yaşam döngüsü olan referans verisi; kendi kimliğiyle erişilir. |
| Province | ✓ | ✓ | | | Ülkeye Id ile bağlı, bağımsız referans aggregate'i. |
| District | ✓ | ✓ | | | İle Id ile bağlı, bağımsız referans aggregate'i; adresin çapası. |
| Workplace | ✓ | ✓ | | | Kendi kuralları (adres, aktiflik) olan bağımsız aggregate. |
| InternProfile | ✓ | ✓ | | | Staj bilgisi ve kuralları taşıyan bağımsız aggregate. |
| Skill | ✓ | ✓ | | | Katalog/referans verisi; bağımsız yönetilir. |
| DailyLog | ✓ | ✓ | | | Ana aggregate; child'larının tutarlılığını korur. |
| DailyLogItem | ✓ | | ✓ | | Yalnızca bir DailyLog içinde anlamlı; DailyLog üzerinden yönetilir. |
| DailyLogSkill | ✓ | | ✓ | | DailyLog'a bağlı; tek başına yaşamaz. |
| ProblemSolvingEntry | ✓ | | ✓ | | DailyLog'a bağlı; tek başına yaşamaz. |
| MentorReview | ✓ | ✓ | | | Ayrı yaşam döngüsü ve kuralları olan bağımsız aggregate. |
| DateRange | | | | ✓ | Kimliği yok; başlangıç–bitiş değeriyle tanımlı, değişmez. |

**Önemli ilke:** Her tablo Aggregate Root değildir. `DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry` birer tablo olacak ama Aggregate Root **değil**, DailyLog'un child'larıdır.

---

## 3. Aggregate sınırları

```text
[DailyLog]  ── Aggregate Root
   ├── DailyLogItem        (child)
   ├── DailyLogSkill       (child)
   └── ProblemSolvingEntry (child)

[Country]        [Province]        [District]       (referans aggregate'ler)
[Workplace]      [InternProfile]   [Skill]          (bağımsız aggregate'ler)
[MentorReview]                                       (bağımsız aggregate)
```

**Kural:** Bir aggregate başka bir aggregate'e yalnızca **Id ile** referans verir (nesne referansıyla değil). Örneğin `Workplace`, `District` nesnesini değil `DistrictId`'yi tutar; `DailyLog`, `InternProfileId`'yi tutar.

---

## 4. DateRange (Value Object)

```text
DateRange
  StartDate
  EndDate
```

Davranışları (kimlik yok, değer odaklı):
- Kurulurken **EndDate < StartDate olamaz** (kendi içinde doğrular).
- `Contains(date)` — bir tarihin aralıkta olup olmadığını söyler.
- `Overlaps(other)` — iki aralığın çakışıp çakışmadığını söyler.

Kullanımı: `InternProfile`'ın staj dönemi bir `DateRange`'dir. Günlük tarihinin dönem içinde olup olmadığı bu value object ile kontrol edilir.

---

## 5. Child entity'lere erişim yöntemi

Child entity'ler (madde, yetkinlik, problem) **doğrudan** değil, **her zaman DailyLog (Aggregate Root) üzerinden** değiştirilir:

```text
dailyLog.AddItem(...)         // doğru
dailyLog.RemoveSkill(...)     // doğru
item.ChangeDuration(...)      // YANLIŞ (root'u atlıyor)
```

Nedeni: Root, invariant'ları korur. Örneğin bir madde eklenince **toplam süre** yeniden hesaplanmalı; bu ancak root üzerinden geçilirse garanti edilir. Ayrıca "onaylı günlüğe madde eklenemez" kuralı da yalnızca root'ta güvenle uygulanır.

---

## 6. Davranış yerleşimi (hangi davranış nerede?)

| Davranış | Sahip model | Not |
|---|---|---|
| Günlük oluşturma | DailyLogManager (Domain Service) | Aynı tarih/dönem kontrolü birden çok nesneyi ilgilendirir |
| Günlük özetini değiştirme | DailyLog | Kendi alanı |
| Çalışma maddesi ekleme/çıkarma | DailyLog | Child; toplam süreyi de günceller |
| Toplam süreyi hesaplama | DailyLog | Invariant; maddelerden türetilir |
| Yetkinlik ekleme | DailyLog | Aynı yetkinlik tekrarını engeller |
| Problem kaydı ekleme | DailyLog | Child |
| Günlüğü gönderme | DailyLog (+ DailyLogManager kontrolü) | Boş günlük gönderilemez |
| Günlüğü onaylama | MentorReview + MentorReviewManager | DailyLog durumunu da günceller |
| Düzeltme talep etme | MentorReview + MentorReviewManager | Yorum zorunlu |
| Çalışma yeri adresini değiştirme | Workplace (+ WorkplaceManager) | İlçe geçerliliği kontrolü |
| Staj dönemini değiştirme | InternProfile | DateRange ile doğrulanır |

---

## 7. Domain modeli ile veritabanı modeli farkı

- Domain modelinde **DailyLog + child'ları tek bir bütün** (aggregate) olarak düşünülür; veritabanında ise bunlar **ayrı tablolar** (`AppDailyLogs`, `AppDailyLogItems`...) olur.
- Domain modelinde nesneler birbirine davranışlarıyla bağlıyken, veritabanında ilişkiler **foreign key** ile kurulur.
- Value Object olan `DateRange`'in domainde ayrı kimliği yoktur; veritabanında `InternProfile` tablosunda iki kolon (`InternshipStartDate`, `InternshipEndDate`) olarak saklanır.

Bu fark, 8. günde (veritabanı tasarımı) daha net ele alınacaktır.
