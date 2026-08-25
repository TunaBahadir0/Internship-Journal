# Final Demo Senaryosu

`00-Stajyer-Program-Rehberi.md` bölüm 9'daki ("Haftalık demo düzeni") formatı izler: çalışan uygulama, en az bir hata/validasyon senaryosu, veritabanı incelemesi, bir test çalıştırması, bir teknik karar açıklaması, bir problem+çözüm anlatımı, AI kullanımının nasıl doğrulandığı.

## Demo öncesi kontrol

```text
[ ] docker compose up -d (PostgreSQL ayakta)
[ ] InternshipJournal.DbMigrator çalıştırıldı (migration + seed tamamlandı)
[ ] InternshipJournal.Web çalışıyor (https://localhost:44399)
[ ] dotnet test tüm projelerde geçiyor (100/100)
[ ] Bilinen açık PR: fix/workplace-country-dropdown-bundling (merge edilmemişse önce onu birleştirin)
```

## 1. Uçtan uca akış (stajyer tarafı)

1. `admin` / `1q2w3E*` ile giriş yapın.
2. **Çalışma Yerleri → Yeni Çalışma Yeri**: bir çalışma yeri oluşturun (Ülke → İl → İlçe nested seçimi — Gün 12).
3. **Staj Profilleri → Yeni Staj Profili**: bir stajyer kullanıcı, mentor kullanıcı ve az önceki çalışma yeriyle bir profil oluşturun, **Başlat** ile Active durumuna geçirin.
4. Stajyer kullanıcı olarak giriş yapın (veya `admin` ile devam edip stajyerin `UserId`'sini kullanarak API üzerinden test edin).
5. **Günlüklerim → Yeni Günlük**: bugünün tarihiyle bir günlük oluşturun.
6. Günlük detayında bir **çalışma maddesi** ekleyin (Toplam Süre'nin otomatik hesaplandığını gösterin).
7. Bir **yetkinlik** ve bir **problem kaydı** ekleyin (AI kullanıldıysa araç adı zorunluluğunu gösterin — bkz. adım 2, hata senaryosu).
8. **Gönder** butonuyla günlüğü Submitted durumuna geçirin.

## 2. Hata / validasyon senaryosu

- Bir çalışma maddesi eklerken **süreyi 0 veya negatif** girin → `Çalışma maddesi süresi pozitif olmalıdır.` (lokalize, ham exception metni değil — Gün 17 düzeltmesi).
- Bir problem kaydında **"Yapay Zekâ Kullanıldı mı"** kutusunu işaretleyip AI aracı adını boş bırakın → `Yapay zekâ kullanıldıysa araç adı ve kullanım özeti girilmelidir.`
- **Sahiplik kontrolü**: başka bir kullanıcıyla giriş yapıp az önceki günlüğü URL üzerinden açmaya/düzenlemeye çalışın → `Bu günlük üzerinde işlem yapma yetkiniz yok.` (Gün 19).

## 3. Mentor tarafı

1. Mentor kullanıcı olarak giriş yapın.
2. **İncelemelerim** menüsünde bekleyen (Submitted) günlüğü görün.
3. Günlük detayına girip **Düzeltme İste** ile bir yorum yazıp gönderin → günlük `RevisionRequested` durumuna geçer, stajyer tarafında **Taslağa Döndür** butonu görünür.
4. Stajyer tarafında taslağa döndürüp tekrar gönderin, mentor tarafında bu kez **Onayla**.
5. Günlük detayındaki **inceleme geçmişi** bölümünde her iki kararın da (yorumlarıyla) göründüğünü gösterin.
6. **Yetkisiz mentor senaryosu**: ilgisiz bir mentor kullanıcısıyla aynı günlüğü onaylamaya çalışın → `Yalnızca kendisine bağlı stajyerin günlüğünü inceleyebilirsiniz.`

## 4. Veritabanı incelemesi (DBeaver)

- `AppDailyLogs`, `AppMentorReviews` tablolarını açıp yeni kayıtları gösterin.
- `AppDailyLogs(InternProfileId, LogDate)` üzerindeki **unique index**'i gösterin, aynı gün için ikinci bir günlük eklemeyi deneyip veritabanı seviyesinde de reddedildiğini (Domain katmanının yanı sıra) gösterin.
- `AppMentorReviews.DailyLogId` FK'sinin **Restrict** olduğunu, `AppDailyLogItems.DailyLogId` FK'sinin **Cascade** olduğunu açıklayın (Gün 18/15 kararı: ayrı aggregate vs. aynı aggregate'in child'ı).

## 5. Test çalıştırması

```bash
dotnet test test/InternshipJournal.Domain.Tests/InternshipJournal.Domain.Tests.csproj
dotnet test test/InternshipJournal.EntityFrameworkCore.Tests/InternshipJournal.EntityFrameworkCore.Tests.csproj
dotnet test test/InternshipJournal.Web.Tests/InternshipJournal.Web.Tests.csproj
```

100/100 test geçmeli. `ErrorCodeLocalizationTests`'in (Web.Tests) her hata kodunun tr/en çevirisi olduğunu otomatik doğruladığını vurgulayın.

## 6. Bir teknik karar açıklaması (örnek)

*"MentorReview neden bir durum makinesi değil, factory metoduyla oluşturulan değişmez bir kayıt?"* — Çünkü bir inceleme, gerçekleştikten sonra değişmeyen bir olgu (fact); `DailyLog` ise zaman içinde durumdan duruma geçen bir varlık. İkisini aynı kalıpla modellemek yanlış soyutlama olurdu (bkz. `Gun-18.md`).

## 7. Bir problem + çözüm anlatımı (örnek)

*"Workplace Create ekranındaki Ülke dropdown'ı neden boştu?"* — ABP'nin `abp-script` bundling sistemi, .NET 10'un yeni Static Web Assets modeliyle tam entegre değildi; dosya statik varlık olarak doğru kayıtlıydı ama ABP'nin ayrı bundling alt sistemi onu bulamıyordu. Düz `<script>` etiketine geçerek Static Web Assets'in kendi (çalışan) yoluna yönlendirildi (bkz. `Gun-19.md` ve ilgili PR).

## 8. AI kullanımının doğrulanması

`docs/ai-usage-report.md`'deki yönteme (testi çalıştır, gerçek sonucu oku, varsayma) atıfta bulunun; somut bir örnek olarak Gün 19'daki `IPermissionManager` reflection araştırmasını gösterin.
