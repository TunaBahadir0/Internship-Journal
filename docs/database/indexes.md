# İndeksler

**İndeks**, bir kitabın arka sayfasındaki dizin gibidir: belirli bir değeri tüm tabloyu taramadan hızlıca bulmayı sağlar. Ama bedeli vardır: her yazma (insert/update) işleminde indeksin de güncellenmesi gerekir. Bu yüzden indeksi **gerçek sorgu ihtiyacına** göre seçeriz — gereksiz indeks yazmayı yavaşlatır.

Her indeks için: desteklediği sorgu, kolon(lar) ve gerekçe.

---

## Foreign key indeksleri

İlişkiler üzerinden yapılan "üste bağlı olanları getir" sorguları için FK kolonları indekslenir.

### IX_AppProvinces_CountryId
- **Sorgu:** Bir ülkenin illerini getir (nested dropdown).
- **Kolon:** CountryId
- **Gerekçe:** Ülke seçilince iller hızlı yüklenmeli.

### IX_AppDistricts_ProvinceId
- **Sorgu:** Bir ilin ilçelerini getir (nested dropdown).
- **Kolon:** ProvinceId
- **Gerekçe:** İl seçilince ilçeler hızlı yüklenmeli.

### IX_AppWorkplaces_DistrictId
- **Sorgu:** Bir ilçedeki çalışma yerleri.
- **Kolon:** DistrictId

### IX_AppInternProfiles_UserId
- **Sorgu:** Kullanıcının (aktif) staj profili.
- **Kolon:** UserId
- **Gerekçe:** Giriş yapan stajyerin profilini bulmak çok sık yapılır.

### IX_AppInternProfiles_MentorUserId
- **Sorgu:** Bir mentora atanmış stajyerler.
- **Kolon:** MentorUserId
- **Gerekçe:** Mentor dashboard'u ve inceleme listesi için.

### IX_AppDailyLogItems_DailyLogId
- **Sorgu:** Bir günlüğün maddeleri.
- **Kolon:** DailyLogId

### IX_AppDailyLogSkills_DailyLogId / IX_AppDailyLogSkills_SkillId
- **Sorgu:** Bir günlüğün yetkinlikleri; belirli yetkinliğin kullanıldığı günlükler.
- **Kolonlar:** DailyLogId ve SkillId (ayrı indeksler)

### IX_AppProblemSolvingEntries_DailyLogId
- **Sorgu:** Bir günlüğün problemleri.
- **Kolon:** DailyLogId

### IX_AppMentorReviews_DailyLogId / IX_AppMentorReviews_MentorUserId
- **Sorgu:** Bir günlüğün incelemeleri; mentorun incelemeleri.
- **Kolonlar:** DailyLogId ve MentorUserId

---

## Sorgu odaklı bileşik indeksler

### IX_AppDailyLogs_InternProfileId_LogDate
- **Sorgu:** Stajyerin tarih aralığındaki günlükleri; belirli tarihteki günlüğü.
- **Kolon sırası:** (InternProfileId, LogDate) — önce kime ait, sonra tarih.
- **Yazma maliyeti:** Orta; günlük ekleme sık ama katlanılır.
- **Gerekçe:** Günlük listesi ve "aynı gün var mı?" kontrolü (BR-1) bu indeksten faydalanır. Zaten `(InternProfileId, LogDate)` unique olduğu için bu indeks unique constraint tarafından da sağlanır.

### IX_AppDailyLogs_Status
- **Sorgu:** Onay bekleyen günlükler (Status = Submitted).
- **Kolon:** Status
- **Gerekçe:** Mentor "inceleme bekleyenler" listesini durumla filtreler. (Alternatif: (MentorUserId, Status) birleşik — mentor bazlı bekleyenler; profil→mentor ilişkisiyle birlikte değerlendirilir.)

### IX_AppProblemSolvingEntries_UsedArtificialIntelligence (opsiyonel)
- **Sorgu:** Yapay zekâ kullanılan problem kayıtları (raporlama).
- **Kolon:** UsedArtificialIntelligence
- **Gerekçe:** Haftalık raporda "AI kullanılan problem sayısı" gösterilir. Ancak boolean seçiciliği düşük olduğundan (yalnızca iki değer) fayda sınırlı olabilir; veri büyüyene kadar ertelenebilir.

---

## İndeks çalışması özeti

| İndeks | Desteklediği sorgu | Kolon sırası | Yazma maliyeti | Gerekçe |
|---|---|---|---|---|
| Provinces(CountryId) | İl listeleme | CountryId | Düşük | Dropdown |
| Districts(ProvinceId) | İlçe listeleme | ProvinceId | Düşük | Dropdown |
| DailyLogs(InternProfileId, LogDate) | Günlük listesi / aynı gün kontrolü | (InternProfileId, LogDate) | Orta | Unique + sorgu |
| DailyLogs(Status) | Bekleyen günlükler | Status | Düşük | Mentor listesi |
| DailyLogSkills(SkillId) | Yetkinliğe göre günlükler | SkillId | Düşük | Rapor |
| InternProfiles(MentorUserId) | Mentorun stajyerleri | MentorUserId | Düşük | Mentor dashboard |

---

## Gün 15 doğrulaması

Migration'da (`Added_DailyLog_Module`) yukarıdaki bileşik indeksler ve FK indeksleri (`IX_AppDailyLogItems_DailyLogId`, `IX_AppDailyLogSkills_SkillId`, `IX_AppProblemSolvingEntries_DailyLogId` dahil) birebir oluştu. İki kalem bilinçli olarak **eklenmedi**:

- **DailyLogItem(WorkType):** 15. Gün müfredat metni bunu listeliyor, ama gerçek bir "WorkType'a göre filtrele" sorgusu henüz yok ve enum yalnızca 8 değer alıyor (düşük seçicilik) — yukarıdaki "gerçek sorgu ihtiyacına göre seç" ilkesiyle çelişirdi. İhtiyaç doğarsa (örn. haftalık "en çok zaman harcanan WorkType" raporu) eklenir.
- **ProblemSolvingEntry(UsedArtificialIntelligence):** Bu dosyada zaten "(opsiyonel)" olarak işaretliydi; karar korundu.

`AppMentorReviews` henüz oluşturulmadı (ayrı bir aggregate olarak ilerleyen bir günde planlanıyor); o tabloya ait indeksler kod yazıldığında eklenecek.

---

## Çok fazla indeksin maliyeti

Her indeks:
- **Yazmayı yavaşlatır** (her insert/update indeksleri de günceller).
- **Disk yer kaplar.**
- Nadiren kullanılıyorsa **fayda sağlamaz**.

Bu yüzden yalnızca gerçek sorgu ihtiyaçlarına (dropdown, listeleme, kontrol, rapor) karşılık gelen indeksleri ekledik; "her ihtimale karşı" indeks eklemekten kaçındık.
