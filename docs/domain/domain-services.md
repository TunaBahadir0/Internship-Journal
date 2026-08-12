# Domain Service'ler

**Domain Service**, tek bir entity'ye doğal olarak yerleşmeyen ama iş alanına ait olan davranışları taşır. Genellikle **birden çok aggregate'i** veya **dış bir kontrolü** (başka kayıt var mı, aktif mi) gerektiren kurallar buraya gelir.

Bir davranış Domain Service'e mi ait, aggregate'e mi? Basit test: Kural yalnızca aggregate'in kendi verisiyle korunabiliyorsa → aggregate (invariant). Başka aggregate'lere bakmak gerekiyorsa → Domain Service.

ABP'de bu servisler genellikle `...Manager` adıyla ve `DomainService` temel sınıfıyla yazılır.

---

## WorkplaceManager

**Neden Domain Service:** Çalışma yeri oluştururken/adres değiştirirken ilçenin aktifliğini ve bağlı olduğu il/ülkeyi ve ad tekrarını kontrol etmek gerekir; bu bilgiler Workplace aggregate'inin dışındadır (District aggregate'ine ve diğer Workplace kayıtlarına bakılır).

**Sorumlulukları:**
- İlçenin aktif olduğunu doğrulamak.
- İlçenin bağlı olduğu il ve ülkeyi kontrol etmek (hiyerarşi tutarlılığı).
- Aynı çalışma yeri adının tekrarını kontrol etmek.

**Metotlar:**
```text
CreateAsync(...)
ChangeAddressAsync(...)
```

**İlgili kurallar:** BR-6, BR-13, BR-29.

---

## InternProfileManager

**Neden Domain Service:** "Kullanıcının başka aktif profili var mı?" kuralı (BR-8) tek bir InternProfile içinde görülemez; tüm profillere bakmayı gerektirir. Ayrıca çalışma yerinin ve mentorun geçerliliği dış aggregate'lere bağlıdır.

**Sorumlulukları:**
- Çalışma yerinin aktif olduğunu doğrulamak (BR-9).
- Mentor kullanıcısının geçerli olduğunu doğrulamak (BR-10).
- Kullanıcı için başka aktif profil olup olmadığını kontrol etmek (BR-8).

**Metotlar:**
```text
CreateAsync(...)
ChangeWorkplaceAsync(...)
ChangeMentorAsync(...)
```

**İlgili kurallar:** BR-8, BR-9, BR-10.

---

## DailyLogManager

**Neden Domain Service:** Günlük oluştururken "aynı tarih için günlük var mı?" (BR-1) kontrolü, oluşturulacak günlüğün kendisinde bilinemez; mevcut günlüklere bakmak gerekir. Tarihin dönem içinde ve gelecekte olmaması da profile bağlıdır.

**Sorumlulukları:**
- Aynı tarih için o stajyerin başka günlüğü olup olmadığını kontrol etmek (BR-1).
- Tarihin staj dönemi içinde olduğunu doğrulamak (BR-3).
- Gelecek tarihli günlük oluşturulmasını engellemek (BR-2).

**Metotlar:**
```text
CreateAsync(Guid internProfileId, DateTime logDate)
SubmitAsync(DailyLog dailyLog)
```

> Not: Günlüğün içeriğiyle ilgili kurallar (toplam süre, madde/yetkinlik ekleme, boş gönderilemez) DailyLog aggregate'inin **kendi** invariant'larıdır. DailyLogManager yalnızca **oluşturma ve gönderme öncesi dış kontrolleri** yapar.

**İlgili kurallar:** BR-1, BR-2, BR-3, BR-21.

---

## MentorReviewManager

**Neden Domain Service:** Bir inceleme iki aggregate'i etkiler: yeni bir `MentorReview` oluşur **ve** ilgili `DailyLog`'un durumu değişir. Bu koordinasyon tek bir aggregate'e ait değildir. Ayrıca mentor–stajyer eşleşmesi kontrolü dış veriyi gerektirir.

**Sorumlulukları:**
- Mentor ile stajyer eşleşmesini doğrulamak (BR-24).
- Günlüğün "Submitted" durumunda olduğunu kontrol etmek (BR-23).
- İnceleme sonucuna göre DailyLog durumunu değiştirmek (onay → Approved, düzeltme → RevisionRequested).

**Metotlar:**
```text
ApproveAsync(...)
RequestRevisionAsync(...)
```

**İlgili kurallar:** BR-23, BR-24, BR-25, BR-26.

---

## Özet

| Domain Service | Temel görevi | Dokunduğu aggregate'ler |
|---|---|---|
| WorkplaceManager | İlçe/hiyerarşi/ad kontrolü | Workplace, District |
| InternProfileManager | Tek aktif profil, geçerli mentor/çalışma yeri | InternProfile, Workplace |
| DailyLogManager | Aynı tarih/dönem/gelecek kontrolü | DailyLog, InternProfile |
| MentorReviewManager | İnceleme + durum koordinasyonu | MentorReview, DailyLog |

**Genel ilke:** Aggregate kendi bildiğini korur (invariant); "başka kayda bakmak" veya "iki aggregate'i birlikte değiştirmek" gerektiğinde Domain Service devreye girer. Application Service ise bu servisleri ve repository'leri çağırarak kullanım senaryosunu koordine eder (bu katman 10. günde ABP ile ele alınacak).
