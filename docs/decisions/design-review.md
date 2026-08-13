# Tasarım Sunumu ve Kontrol Kapısı (Design Review)

Bu doküman, kod geliştirmeye geçmeden önce analiz + domain + veritabanı tasarımının savunulmasını, kritik soruların cevaplarını, alınan geri bildirimleri ve son onayı kaydeder. Kontrol kapısından geçen tasarım "v2" olarak işaretlenir.

---

## 1. Sunum akışı

Sunum şu sırayla yapıldı:

1. Projenin amacı (Staj Günlüğü ve Gelişim Takip Uygulaması)
2. Aktörler (Stajyer, Mentor, Admin)
3. Temel kullanım senaryoları (11 senaryo)
4. İş kuralları (27 kural)
5. Ubiquitous Language (ortak dil)
6. Domain modeli (Entity/VO/Aggregate)
7. Aggregate kararları
8. ER diyagramı
9. Normalizasyon kararları (1NF/2NF/3NF)
10. Constraint'ler (unique/FK)
11. İndeksler
12. Adres hiyerarşisi (ülke → il → ilçe)
13. Belirsiz noktalar (mentora sorular)

---

## 2. Kritik sorulara cevaplar

### Domain

**DailyLog neden Aggregate Root olabilir?**
Çünkü çalışma maddeleri, yetkinlikler ve problemler tek başına anlamlı değildir; hep bir güne aittir. Toplam süre ve "onaylı günlük değiştirilemez" gibi invariant'lar ancak tek bir kapıdan (root) geçilirse korunur. Bu yüzden DailyLog bu bütünün köküdür.

**Günlük maddesi (DailyLogItem) bağımsız repository gerektirir mi?**
Hayır. Madde yalnızca DailyLog üzerinden değiştirilir; DailyLog kendi repository'siyle child'larıyla birlikte yüklenip kaydedilir. Maddeye ayrı repository vermek, root'u atlayıp invariant'ı bozma riski doğurur.

**Toplam süre hangi model tarafından hesaplanmalı?**
DailyLog tarafından, çalışma maddelerinin toplamından. Kullanıcıdan alınmaz (BR-11, I-1). Böylece toplam ile maddeler asla çelişmez.

**MentorReview ayrı aggregate olursa avantajı nedir?**
İncelemenin kendi yaşam döngüsü (mentor tarafından üretilir) DailyLog'u şişirmez; "mentor içeriği değiştiremez" ayrımı net kalır; düzeltme döngüsünde birden çok inceleme tutulabilir. İki aggregate arasındaki tutarlılık (inceleme → durum değişimi) MentorReviewManager ile sağlanır.

**Aynı tarih kontrolü neden Domain Service gerektirebilir?**
Çünkü oluşturulacak günlüğün kendisi "başka bir günlük var mı?" sorusuna cevap veremez; mevcut günlüklere bakmak gerekir. Bu, tek aggregate'in dışına çıkan bir kontroldür → DailyLogManager.

### Veritabanı

**Aynı tarihli ikinci günlük nasıl engellenir?**
`AppDailyLogs(InternProfileId, LogDate)` üzerinde unique constraint ile (BR-1). Ek olarak DailyLogManager oluşturma öncesi kontrol eder. Veritabanı, yarış durumunda son güvence.

**Aynı yetkinlik iki kez nasıl engellenir?**
`AppDailyLogSkills(DailyLogId, SkillId)` üzerinde unique constraint ile (BR-5).

**Workplace içinde neden ülke, il ve ilçe kimlikleri birlikte saklanmamalı?**
Çünkü `District → Province → Country` geçişli bağımlılığı vardır. Üçünü birlikte saklamak tekrar ve çelişki (ör. "Ankara/Kadıköy") üretir. Yalnızca `DistrictId` saklanır; il/ülke ilişkiden türetilir (3NF).

**Hangi kolonlar indekslenmeli?**
FK kolonları (dropdown/ilişki sorguları), `AppDailyLogs(InternProfileId, LogDate)` (liste + aynı gün kontrolü), `AppDailyLogs(Status)` (bekleyen günlükler), `AppInternProfiles(UserId)` ve `(MentorUserId)`. Boolean gibi düşük seçicilikli kolonlar (UsedAI) ertelendi.

**Child tabloların silme davranışı nasıl olmalı?**
DailyLog child'ları (item/skill/problem) aggregate ile birlikte gider (cascade). Referans veriler (konum, çalışma yeri, yetkinlik) restrict — silinmez, pasifleştirilir. MentorReview ayrı aggregate olduğu için restrict. Not: ABP varsayılan soft-delete kullanır.

---

## 3. Geri bildirim kayıtları

### Geri bildirim 1 — Tek aktif profil garantisi
- **İlk tasarım:** "Bir kullanıcı için tek aktif profil" yalnızca InternProfileManager'da (uygulama kontrolü) tutuluyordu.
- **Tespit edilen sorun:** İki eşzamanlı istek yarış durumunda iki aktif profil oluşturabilir.
- **Yeni karar:** `AppInternProfiles(UserId)` üzerinde **kısmi (filtered) unique index** — yalnızca `Status = Active` satırları için. Servis kontrolü de korunur (çift savunma).
- **Teknik gerekçe:** Yapısal tekillik veritabanında garanti edilmeli; uygulama kontrolü tek başına yeterli değil.

### Geri bildirim 2 — MentorReview kardinalitesi
- **İlk tasarım:** DailyLog ↔ MentorReview bire bir gibi düşünülüyordu.
- **Tespit edilen sorun:** Düzeltme döngüsünde günlük birden çok kez incelenebilir; tek inceleme geçmişi kaybeder.
- **Yeni karar:** İlişki **1..*** (bir günlüğün birden çok incelemesi olabilir); en güncel karar `ReviewedAt` ile bulunur.
- **Teknik gerekçe:** İnceleme geçmişi denetlenebilir olmalı; her düzeltme/onay ayrı kayıt.

### Geri bildirim 3 — Silme stratejisi netleştirme
- **İlk tasarım:** Fiziksel cascade/restrict üzerinden konuşuluyordu.
- **Tespit edilen sorun:** ABP zaten soft-delete kullanıyor; fiziksel silme çoğu senaryoda olmayacak.
- **Yeni karar:** Varsayılan **soft-delete** (IsDeleted); FK davranışları yine de tanımlı kalır (bütünlük için). "Kullanılan konum silinmez" kuralı soft-delete + restrict ile desteklenir.
- **Teknik gerekçe:** Geçmiş kayıtların adres/ilişki bütünlüğü korunmalı.

### Geri bildirim 4 — WorkType enum hizalama
- **İlk tasarım:** Mini projedeki WorkType (6 değer) akla geliyordu.
- **Tespit edilen sorun:** Ana projede WorkType 8 değerli (Setup, Training, Development, Research, Testing, Documentation, Meeting, ProblemSolving).
- **Yeni karar:** Ana proje enum'u (8 değer) kullanılacak; DailyLogItem.WorkType buna göre.
- **Teknik gerekçe:** Ortak dil ve rapor tutarlılığı.

---

## 4. Onay

- [x] Kritik sorular cevaplandı.
- [x] Tasarım geri bildirimlere göre güncellendi (bkz. domain-model-v2.md, erd-v2.png).
- [x] Domain modeli ile veritabanı modeli farkı açıklanabiliyor.
- [x] Ana iş kuralları (aynı gün tek günlük, onaylı değişmez, toplam süre hesaplanır, aynı yetkinlik tek kez) korunuyor.
- [x] **Kod geliştirmeye geçiş için tasarım onaylandı.**

Sonraki adım (Gün 10): ABP çözüm yapısını tanımak ve bu onaylı tasarımı katmanlara yerleştirmek.
