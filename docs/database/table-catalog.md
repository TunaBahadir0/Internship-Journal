# Tablo Kataloğu

Uygulamaya özel **11 tablo**. Tüm birincil anahtarlar `Guid` (uuid) tipindedir (ABP `FullAuditedAggregateRoot<Guid>` / `Entity<Guid>`). ABP'nin eklediği audit alanları (CreationTime, CreatorId, LastModificationTime, IsDeleted vb.) her tabloda otomatik bulunur ve burada tekrar yazılmamıştır.

Null sütunu: E = zorunlu (NOT NULL), H = opsiyonel (NULL).

---

## 1. AppCountries
**Amaç:** Ülke referans verisi.
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| Code | varchar(3) | E | Ülke kodu (TR, DE...), benzersiz |
| Name | varchar(100) | E | Ülke adı |
| IsActive | boolean | E | Aktif/pasif |

**Unique:** Code
**Silme davranışı:** Kullanılan ülke silinmez (restrict); pasifleştirilir.

---

## 2. AppProvinces
**Amaç:** İl referans verisi (ülkeye bağlı).
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| CountryId | uuid | E | FK → AppCountries |
| Code | varchar(10) | H | Plaka/kod |
| Name | varchar(100) | E | İl adı |
| IsActive | boolean | E | Aktif/pasif |

**FK:** CountryId → AppCountries.Id
**Unique:** (CountryId, Name)
**Silme davranışı:** CountryId için restrict.

---

## 3. AppDistricts
**Amaç:** İlçe referans verisi (ile bağlı). Adresin çapası.
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| ProvinceId | uuid | E | FK → AppProvinces |
| Code | varchar(10) | H | Kod |
| Name | varchar(100) | E | İlçe adı |
| IsActive | boolean | E | Aktif/pasif |

**FK:** ProvinceId → AppProvinces.Id
**Unique:** (ProvinceId, Name)
**Silme davranışı:** ProvinceId için restrict.

---

## 4. AppWorkplaces
**Amaç:** Çalışma yeri ve adresi.
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| Name | varchar(200) | E | Çalışma yeri adı |
| TaxNumber | varchar(20) | H | Vergi no |
| Phone | varchar(30) | H | Telefon |
| Email | varchar(256) | H | E-posta (girilirse geçerli format) |
| Website | varchar(256) | H | Web sitesi |
| DistrictId | uuid | E | FK → AppDistricts (adres) |
| AddressLine | varchar(500) | E | Açık adres |
| PostalCode | varchar(10) | H | Posta kodu |
| Latitude | decimal(9,6) | H | Enlem |
| Longitude | decimal(9,6) | H | Boylam |
| IsActive | boolean | E | Aktif/pasif |

**FK:** DistrictId → AppDistricts.Id
**Silme davranışı:** DistrictId için restrict. Not: İl/ülke ayrıca saklanmaz, ilişkiden türetilir.

---

## 5. AppInternProfiles
**Amaç:** Stajyer profili.
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| UserId | uuid | E | ABP kullanıcı kimliği (stajyer) |
| MentorUserId | uuid | E | ABP kullanıcı kimliği (mentor) |
| WorkplaceId | uuid | E | FK → AppWorkplaces |
| University | varchar(200) | H | Üniversite |
| SchoolDepartment | varchar(200) | H | Bölüm |
| StudentNumber | varchar(50) | H | Öğrenci no |
| InternshipStartDate | date | E | Staj başlangıcı (DateRange) |
| InternshipEndDate | date | E | Staj bitişi (DateRange) |
| RequiredWorkDays | int | H | Gerekli çalışma günü |
| Status | int | E | Profil durumu (enum) |

**FK:** WorkplaceId → AppWorkplaces.Id
**Unique/İş kuralı:** Bir UserId için yalnızca bir **aktif** profil (kısmi/filtreli unique veya servis kontrolü).
**Not:** Ad, soyad, e-posta, şifre burada tutulmaz; UserId üzerinden ABP kullanıcı tablosundan gelir.
**Silme davranışı:** WorkplaceId için restrict.

---

## 6. AppSkills
**Amaç:** Yetkinlik kataloğu.
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| Name | varchar(100) | E | Yetkinlik adı |
| Category | varchar(100) | H | Kategori (Backend, Frontend...) |
| Description | varchar(500) | H | Açıklama |
| IsActive | boolean | E | Aktif/pasif |

**Unique:** Name (öneri; katalog tekrarı olmasın)
**Silme davranışı:** Kullanılan yetkinlik silinmez; pasifleştirilir.

---

## 7. AppDailyLogs
**Amaç:** Günlük (ana aggregate kökü).
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| InternProfileId | uuid | E | FK → AppInternProfiles |
| LogDate | date | E | Günlük tarihi |
| Summary | varchar(1000) | H | Özet |
| TotalMinutes | int | E | Maddelerden hesaplanır (türetilmiş) |
| Status | int | E | DailyLogStatus (Draft/Submitted/RevisionRequested/Approved) |
| SubmittedAt | timestamptz | H | Gönderim zamanı |
| ReviewedAt | timestamptz | H | İnceleme zamanı |
| ApprovedAt | timestamptz | H | Onay zamanı |

**FK:** InternProfileId → AppInternProfiles.Id
**Unique:** (InternProfileId, LogDate) — aynı gün tek günlük
**Silme davranışı:** InternProfileId için restrict. Child'lar (item/skill/problem) cascade.

---

## 8. AppDailyLogItems
**Amaç:** Günlük çalışma maddesi (child).
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| DailyLogId | uuid | E | FK → AppDailyLogs |
| Title | varchar(200) | E | Başlık |
| Description | varchar(1000) | H | Açıklama |
| WorkType | int | E | WorkType enum |
| DurationMinutes | int | E | Süre (>0) |
| IsCompleted | boolean | E | Tamamlandı mı |

**FK:** DailyLogId → AppDailyLogs.Id
**Silme davranışı:** DailyLogId için **cascade** (günlük silinince maddeler de gider).

---

## 9. AppDailyLogSkills
**Amaç:** Günlük–yetkinlik bağlantı tablosu (child + junction).
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| DailyLogId | uuid | E | FK → AppDailyLogs |
| SkillId | uuid | E | FK → AppSkills |
| LearningLevel | int | E | LearningLevel enum |
| Note | varchar(500) | H | Not |

**FK:** DailyLogId → AppDailyLogs.Id; SkillId → AppSkills.Id
**Unique:** (DailyLogId, SkillId) — aynı yetkinlik bir günlükte tek kez
**Silme davranışı:** DailyLogId için cascade; SkillId için restrict.

---

## 10. AppProblemSolvingEntries
**Amaç:** Problem çözme ve yapay zekâ kaydı (child).
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| DailyLogId | uuid | E | FK → AppDailyLogs |
| Title | varchar(200) | E | Başlık |
| ProblemDescription | varchar(2000) | E | Problem açıklaması |
| ErrorMessage | varchar(1000) | H | Hata mesajı |
| AttemptedSolutions | varchar(2000) | H | Denenen çözümler |
| RootCause | varchar(1000) | H | Kök neden |
| FinalSolution | varchar(2000) | H | Nihai çözüm |
| UsedArtificialIntelligence | boolean | E | AI kullanıldı mı |
| AiToolName | varchar(100) | H | AI aracı |
| AiPromptSummary | varchar(1000) | H | İstem özeti |
| AiSuggestion | varchar(2000) | H | AI önerisi |
| AiSuggestionAccepted | boolean | H | Öneri kabul edildi mi |
| AiRejectionReason | varchar(1000) | H | Ret gerekçesi |

**FK:** DailyLogId → AppDailyLogs.Id
**Silme davranışı:** DailyLogId için cascade.

---

## 11. AppMentorReviews
**Amaç:** Mentor incelemesi (ayrı aggregate).
**Primary key:** Id

| Kolon | Tip | Null | Açıklama |
|---|---|:--:|---|
| Id | uuid | E | PK |
| DailyLogId | uuid | E | FK → AppDailyLogs |
| MentorUserId | uuid | E | ABP kullanıcı kimliği (mentor) |
| Decision | int | E | MentorReviewDecision (Approved/RevisionRequested) |
| Comment | varchar(1000) | H | Yorum (düzeltmede zorunlu) |
| ReviewedAt | timestamptz | E | İnceleme zamanı |

**FK:** DailyLogId → AppDailyLogs.Id
**Silme davranışı:** DailyLogId için restrict (ayrı aggregate; günlükle otomatik silinmez).
