# Aggregate Kararları

Bu doküman, her aggregate için sınırını, sorumluluğunu ve gerekçesini belgeler. Her tablo Aggregate Root değildir; burada hangi nesnenin neden bir aggregate kökü olduğunu (veya bir root'un child'ı olduğunu) açıklıyoruz.

---

## DailyLog (Ana Aggregate)

- **Sorumluluğu:** Bir günün çalışma kaydını, içindeki maddeleri, yetkinlikleri ve problemleri tutarlı biçimde yönetmek.
- **Koruduğu iş kuralları:** BR-1, BR-2, BR-3, BR-4, BR-5, BR-11, BR-21, BR-22, BR-27.
- **İçerdiği nesneler:** DailyLogItem, DailyLogSkill, ProblemSolvingEntry (child entity'ler).
- **Dış aggregate referansları:** InternProfileId (Id ile), SkillId (DailyLogSkill içinde Id ile).
- **Transaction sınırı:** DailyLog ve tüm child'ları **tek transaction** içinde kaydedilir. Bir madde eklenip toplam süre güncellendiğinde ikisi birlikte kalıcı olur.
- **Repository ihtiyacı:** Evet — `IDailyLogRepository` (child'larıyla birlikte yükleme, tarihe göre sorgu).
- **Kararın gerekçesi:** Maddeler/yetkinlikler/problemler tek başına anlamlı değildir; hep bir günlüğe aittir. Toplam süre ve "onaylı değiştirilemez" gibi invariant'lar ancak root üzerinden korunur.

---

## MentorReview (Ayrı Aggregate)

- **Sorumluluğu:** Bir günlüğe mentorun verdiği kararı (onay/düzeltme) ve yorumu kaydetmek.
- **Koruduğu iş kuralları:** BR-23, BR-24, BR-25, BR-26.
- **İçerdiği nesneler:** Yok (basit aggregate).
- **Dış aggregate referansları:** DailyLogId, MentorUserId (Id ile).
- **Transaction sınırı:** İnceleme kaydı oluşturulurken ilgili DailyLog'un durumu da güncellenir. Bu iki aggregate'i **bir Domain Service (MentorReviewManager)** koordine eder.
- **Repository ihtiyacı:** Evet — `IMentorReviewRepository`.
- **Kararın gerekçesi:** İnceleme, günlüğün içeriğinden bağımsız bir yaşam döngüsüne sahiptir ve mentor tarafından üretilir. DailyLog aggregate'ini şişirmemek ve "mentor içeriği değiştiremez" ayrımını net tutmak için ayrı aggregate seçildi. İki aggregate arasındaki tutarlılık (inceleme → durum değişimi) Domain Service ile sağlanır.

---

## Workplace (Bağımsız Aggregate)

- **Sorumluluğu:** Çalışma yerinin kimlik, iletişim ve adres bilgilerini ve aktiflik durumunu yönetmek.
- **Koruduğu iş kuralları:** BR-6, BR-12, BR-13, BR-14, BR-29.
- **İçerdiği nesneler:** Yok (adres, ayrı child değil; DistrictId + alanlar olarak tutulur).
- **Dış aggregate referansları:** DistrictId (Id ile).
- **Transaction sınırı:** Yalnızca Workplace.
- **Repository ihtiyacı:** Evet — `IWorkplaceRepository`.
- **Kararın gerekçesi:** Çalışma yeri, stajyerden ve günlükten bağımsız yaşar (önce admin tanımlar). Adres yalnızca DistrictId ile saklanarak ülke/il/ilçe çelişkisi baştan engellenir.

---

## InternProfile (Bağımsız Aggregate)

- **Sorumluluğu:** Bir kullanıcının staj bilgilerini (çalışma yeri, mentor, dönem, okul, durum) yönetmek.
- **Koruduğu iş kuralları:** BR-7, BR-8, BR-9, BR-10.
- **İçerdiği nesneler:** DateRange (staj dönemi — value object).
- **Dış aggregate referansları:** UserId, MentorUserId, WorkplaceId (Id ile).
- **Transaction sınırı:** Yalnızca InternProfile.
- **Repository ihtiyacı:** Evet — `IInternProfileRepository` (kullanıcının aktif profili sorgusu).
- **Kararın gerekçesi:** Staj profili bağımsız bir yaşam döngüsüne (Start/Complete/Cancel) sahiptir ve günlüklerin bağlandığı çapadır.

---

## Skill (Referans/Katalog Aggregate)

- **Sorumluluğu:** Yetkinlik kataloğunu (ad, kategori, aktiflik) yönetmek.
- **Koruduğu iş kuralları:** BR-17 (pasif yetkinlik seçilemez — kullanım anında kontrol edilir).
- **İçerdiği nesneler:** Yok.
- **Dış aggregate referansları:** Yok.
- **Transaction sınırı:** Yalnızca Skill.
- **Repository ihtiyacı:** Genellikle generic repository yeterli.
- **Kararın gerekçesi:** Yetkinlik, tüm günlükler tarafından paylaşılan bir referans veridir; günlüğe Id ile bağlanır (kopyalanmaz).

---

## Country / Province / District (Referans Aggregate'ler)

- **Sorumluluğu:** Konum hiyerarşisini (ülke → il → ilçe) ve aktifliklerini yönetmek.
- **Koruduğu iş kuralları:** BR-6, BR-28.
- **İçerdiği nesneler:** Yok (her biri ayrı aggregate; birbirine Id ile bağlı).
- **Dış aggregate referansları:** Province → CountryId, District → ProvinceId.
- **Transaction sınırı:** Her biri kendi başına.
- **Repository ihtiyacı:** Generic repository + il/ilçe listeleme sorguları.
- **Kararın gerekçesi:** Bunlar nadiren değişen, paylaşılan referans verilerdir. Tek bir "Location" aggregate'i yapmak yerine ayrı ayrı aggregate tutmak, ilçe listesini il bazında sorgulamayı ve seed etmeyi kolaylaştırır. Çoğunlukla seed ile gelir.

---

## Özet: Neden her tablo Aggregate Root değil?

`DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry` veritabanında ayrı tablolar olacak ama **DailyLog'un child'larıdır**. Eğer her birini ayrı aggregate yapsaydık:
- Toplam süre gibi invariant'ları korumak zorlaşırdı (madde ayrı kaydedilir, root haberi olmaz).
- "Onaylı günlüğe madde eklenemez" kuralı kolayca atlanabilirdi.
- Bir günlüğü child'larıyla tutarlı kaydetmek için ekstra koordinasyon gerekirdi.

Bu yüzden bu üç nesne, yalnızca DailyLog üzerinden değiştirilir.
