# Güvenlik Kontrol Listesi

Gün 19'da yöneticinin geri bildirimiyle (permission/sahiplik kontrollerinin tutarlı uygulanması) başlayan çalışmanın özeti ve projenin genelindeki güvenlik/erişim kontrolü durumu.

## Yetkilendirme ve sahiplik

- [x] **Permission yapısı tanımlı ve uygulanıyor** — `InternshipJournalPermissions` (master doküman bölüm 22 ile birebir), `WorkplaceAppService`/`DailyLogAppService`/`MentorReviewAppService`'e `[Authorize(...)]` ile uygulandı.
- [x] **Stajyer/Mentor rolleri seed ediliyor**, doğru izin gruplarıyla (`InternshipJournalDataSeedContributor.SeedRolesAsync`).
- [x] **DailyLog sahiplik kontrolü** — bir stajyer yalnızca kendi `InternProfile.UserId`'sine ait günlüğü değiştirebilir (`DailyLogAppService.EnsureOwnerAsync`); tüm mutasyon metotlarında (Add/Update/Remove Item/Skill/Problem, Submit, ReturnToDraft, UpdateSummary) tutarlı uygulanıyor.
- [x] **Görüntüleme, mutasyondan daha gevşek ama sınırsız değil** — `EnsureCanViewAsync`: yalnızca günlüğün sahibi VEYA atanmış mentor görebilir, ilgisiz bir kullanıcı göremez.
- [x] **Mentor yetkisi çift kontrollü** — hem `MentorReviewManager` (mentor gerçekten o stajyerin mentoru mu) hem de tek, yetkili bir giriş noktası (`MentorReviewAppService.ApproveAsync`/`RequestRevisionAsync`) üzerinden; Gün 16'dan kalan, hiçbir yetki kontrolü yapmayan paralel bir `DailyLogAppService.ApproveAsync`/`RequestRevisionAsync` çifti Gün 19'da **kaldırıldı** (bulunup kapatılan gerçek bir güvenlik açığı).
- [x] **Kullanıcı kimliği asla client input olarak kabul edilmiyor** — mentor/stajyer kimliği her zaman `CurrentUser.GetId()`'den okunuyor (`DailyLogAppService.CreateAsync`, `MentorReviewAppService.ApproveAsync` vb.); hiçbir DTO'da "ben şuyum" diyen bir alan yok.

## Girdi doğrulama

- [x] Tüm DTO'larda `[Required]`/`[StringLength]`/`[Range]` gibi DataAnnotations ile ABP'nin otomatik `AbpValidationException` doğrulaması aktif.
- [x] Domain katmanı, Application katmanının (DTO doğrulaması) yakalayamayacağı durumları (ör. yalnızca boşluktan oluşan bir yorum, `IsNullOrWhiteSpace`) ikinci bir savunma hattı olarak ayrıca kontrol ediyor (`MentorReview.RequestRevision`, `ProblemSolvingEntry.SetAiInformation`).

## Hata mesajları

- [x] Kullanıcıya gösterilen hiçbir mesaj ham CLR/exception metni değil — `InternshipJournalPageModel.GetErrorMessage` her `BusinessException`'ı `ex.Code` üzerinden lokalize ediyor (Gün 17 düzeltmesi).
- [x] `ErrorCodeLocalizationTests`, yeni bir hata kodu eklenip çevirisi unutulursa build'i kırar (regresyon koruması).

## Gizli bilgi yönetimi

- [x] `appsettings.json`'daki bağlantı dizesi/şifreleme anahtarı ABP şablonunun **varsayılan yerel geliştirme değerleri** — gerçek bir üretim sırrı değil (README'de açıkça belirtiliyor).
- [x] Gerçek üretim sırları (varsa) `appsettings.json`'a değil, ortam değişkenlerine veya bir secret store'a taşınmalı — bu proje kapsamında (öğrenim amaçlı, tek geliştirici) uygulanmadı, **teslim sonrası gerçek bir dağıtım öncesi yapılması gereken bir adım** olarak not ediliyor.
- [x] `openiddict.pfx` gibi sertifika dosyaları `.gitignore` ile hariç tutuluyor (`.csproj`'daki `Exists('./openiddict.pfx')` koşulu, dosyanın repoya commit edilmediğini varsayıyor).

## SQL enjeksiyonu ve veri erişimi

- [x] Tüm veritabanı erişimi EF Core LINQ sorguları üzerinden — hiçbir yerde ham/interpolate edilmiş SQL string'i yok, parametrik sorgular EF Core tarafından otomatik sağlanıyor.

## Bilinen sınırlamalar (dürüstçe belirtilmesi gereken)

- **Workplace/InternProfile için sahiplik kontrolü yok** — bunlar Admin'in yönettiği varlıklar olarak tasarlandı (master doküman bölüm 2), bu yüzden bilerek eklenmedi. Ama şu an herhangi bir kimliği doğrulanmış kullanıcı `[Authorize(Permission)]` izniyle bunları değiştirebilir; gerçek "yalnızca Admin" kısıtlaması, rol ataması (kimin Admin rolünde olduğu) dışında ayrıca uygulanmadı.
- **Menü öğeleri izin bazlı gizlenmiyor** — "İncelemelerim" menü girişi her authenticated kullanıcıya görünüyor, yalnızca gerçek mentor olmayan biri sayfaya girdiğinde boş liste görür (veri sızıntısı yok, ama UX olarak ideal değil).
- **Rol ataması (kullanıcıya Stajyer/Mentor rolü verme) için ayrı bir yönetim ekranı yok** — ABP'nin kendi hazır Identity/Role Management ekranları (Administration menüsü altında) üzerinden yapılması bekleniyor; bu proje özelinde ayrı bir ekran yazılmadı.
