# Gün 19

Tarih: 26 Ağustos 2026, Çarşamba

## Bugün tamamladığım işler

Bugünün odak noktası, yöneticinin daha önceki geri bildirimindeki ikinci maddeydi: *"permission/sahiplik kontrollerinin ilgili tüm metotlarda tutarlı şekilde uygulanması"*. Bunu üç parçada ele aldım:

### 1. Gerçek bir güvenlik açığını kapattım

Gün 16'da bilerek ertelediğim sahiplik kontrolü eksikliğini araştırırken, **daha ciddi bir mimari tutarsızlık** buldum: `DailyLogAppService.ApproveAsync`/`RequestRevisionAsync` (Gün 16) hâlâ duruyordu ve **hiçbir yetki kontrolü yapmadan** herhangi bir kullanıcının herhangi bir günlüğü onaylamasına/reddetmesine izin veriyordu — Gün 18'de yazdığım `MentorReviewAppService.ApproveAsync`/`RequestRevisionAsync` (mentor eşleşmesini kontrol eden, `MentorReview` kaydı oluşturan, doğru yol) tamamen atlanabiliyordu. İki paralel yol, biri güvenli biri değil.

**Düzeltme:** `IDailyLogAppService`'ten `ApproveAsync`/`RequestRevisionAsync`'i tamamen kaldırdım. Artık onay/düzeltme yalnızca `IMentorReviewAppService` üzerinden, mentor-stajyer eşleşmesi doğrulanarak ve denetim kaydı (`MentorReview`) oluşturularak yapılabiliyor.

### 2. Sahiplik kontrolü (tüm ilgili metotlara tutarlı şekilde)

`DailyLogAppService`'teki her mutasyon metoduna (`UpdateSummary`, Add/Update/Remove Item/Skill/Problem, `Submit`, `ReturnToDraft`) `EnsureOwnerAsync` kontrolü eklendi — çağıran kullanıcı, günlüğün `InternProfile.UserId`'si değilse `BusinessException(DailyLogNotOwnedByCurrentUser)` fırlatılıyor. `GetAsync` için daha gevşek bir kontrol (`EnsureCanViewAsync`): sahibi VEYA atanmış mentor görebilir — mentorun inceleme yapabilmesi için içeriği görmesi gerekiyor.

### 3. Permission yapısı (ABP idiomu)

Master dokümanın "22. Permission yapısı" bölümündeki tam hiyerarşiyi (`Workplaces`/`.Create`/`.Edit`, `DailyLogs`/`.Create`/`.Edit`/`.Submit`, `Reviews`/`.Approve`/`.RequestRevision`, `Reports`, `Administration`) `InternshipJournalPermissions`+`InternshipJournalPermissionDefinitionProvider`'da tanımladım (ikisi zaten Gün 10'dan beri boş stub olarak duruyordu). `WorkplaceAppService`, `DailyLogAppService`, `MentorReviewAppService`'e sınıf/metot seviyesinde `[Authorize(...)]` ekledim.

**"Stajyer" ve "Mentor" rollerini** `InternshipJournalDataSeedContributor`'a ekledim (Gün 11'den beri var olan seed akışına bir adım daha) — Stajyer rolüne `DailyLogs.*`, Mentor rolüne `DailyLogs.Default` + `Reviews.*` izinleri otomatik veriliyor.

### 4. Mentor tarafı ekranları

- `Pages/MentorReviews/Index` — mentorun kendisine bağlı stajyerlerin **bekleyen (Submitted) günlükleri** listesi, `IDailyLogAppService.GetListForReviewAsync()` (yeni: `DailyLog + InternProfile + IdentityUser` join'i, `DailyLogRepository.GetListForMentorAsync`) üzerinden.
- `DailyLogs/Detail` sayfasına Onayla/Düzeltme İste formlarını (artık `IMentorReviewAppService` çağırıyor, Düzeltme'de yorum zorunlu) ve bir **inceleme geçmişi** bölümü ekledim (`Stajyer: mentor yorumlarını görüntüler` rol gereksinimini karşılıyor — daha önce hiç ekranda gösterilmiyordu).
- Menüye "İncelemelerim" girişi eklendi.

## Öğrendiğim / pekiştirdiğim konular

- **ABP'de `[Authorize]` ile ilgili yanlış varsayımım:** Önce ayrı bir `Volo.Abp.Authorization.AuthorizeAttribute` olduğunu varsaydım; gerçekte ABP, AppService'lerde **standart `Microsoft.AspNetCore.Authorization.AuthorizeAttribute`'u** kullanıyor — dinamik proxy interceptor'ı bu attribute'u okuyup ilk pozisyonel string argümanı izin adı olarak yorumluyor. Reflection ile assembly'yi tarayıp gerçek tipi bulmadan varsaymadım.
- **Permission sabitlerinin katman sınırı hatası:** İlk yazımda `InternshipJournalPermissions`'ı `Application.Contracts`'a koydum — ama seed işlemi (rol+izin atama) `Domain` katmanında yaşıyor ve Domain, Application.Contracts'a bağımlı OLAMAZ (ters yön). Sabitleri `Domain.Shared`'a taşıyarak düzelttim; bu, ABP'nin kendi projelerinde de permission sabitlerinin genelde en alt katmanda durmasının nedenini somut olarak gösterdi.
- **`IPermissionManager.SetForRoleAsync` diye bir şey yok:** API yalnızca genel `SetAsync(permissionName, providerName, providerKey, isGranted)`; rol sağlayıcısının adı ("R") hiçbir yerde dokümante bir sabit olarak karşıma çıkmadı, IL'den (`ldstr` opcode) okuyarak doğruladım — varsayımla "RoleName" gibi bir şey yazmadım.
- **Test host'ta izin kontrolünün bilerek kapalı olduğu:** `InternshipJournalEntityFrameworkCoreTestModule`'de `PermissionManagementOptions.IsDynamicPermissionStoreEnabled = false` (Gün 10'dan beri) — bu, `[Authorize(Permission)]` eklediğimde MEVCUT hiçbir testin kırılmayacağı, ama gerçek çalışan uygulamada (DbMigrator+Web) korumanın tam olarak uygulanacağı anlamına geliyor. Bunu varsaymadım, gerçek test host modülünün kodunu okuyup doğruladım.

## Alınan kararlar

1. `DailyLogAppService.ApproveAsync`/`RequestRevisionAsync` **kaldırıldı**, düzeltilmedi/yetkilendirilmedi — çünkü `MentorReviewAppService` zaten doğru, denetim-kayıtlı yolu sağlıyor; iki paralel yol tutmak ilk günden itibaren yeni bir tutarsızlık kaynağı olurdu.
2. Onayla/Düzeltme İste butonları, mevcut kullanıcının gerçekten mentor olup olmadığına bakılmaksızın `Status == Submitted` iken gösteriliyor — gerçek yetki kontrolü sunucu tarafında (`MentorReviewNotAuthorized`) yapılıyor, arayüz yalnızca "yanlış kullanıcıya buton gösterme" inceliğini atlıyor. Bunu `DailyLogDetailDto`'ya mentor kimliği eklemeden basit tutmayı tercih ettim; gerçek güvenlik sınırı zaten sunucuda.
3. Test paketine (56 test) rol/izin atama eklemedim — test host'un `IsDynamicPermissionStoreEnabled = false` ayarı zaten tüm izin kontrollerini no-op yaptığı için (yukarıda doğrulandı), bu ek karmaşıklık hiçbir gerçek koruma test etmeyecekti. Sahiplik kontrolü (ownership) testleri ise gerçek iş kuralı olduğu için eklendi ve gerçekten çalışıyor.

## Yapay zekâ kullanımı

`IPermissionManager`'ın gerçek namespace'ini ve rol sağlayıcı adını ("R") hiçbir resmi dokümanda hızlıca bulamayınca varsayımla ilerlemedim — küçük bir C# konsol projesiyle ilgili NuGet paketini indirip reflection'la (assembly tarama, IL byte'larından `ldstr` okuma) gerçek API'yi ve sabiti doğrudan doğruladım, sonra koda geçtim.

## Kabul kriteri kontrolü

- [x] `DailyLogAppService`'teki tüm mutasyon metotları sahiplik kontrolü yapıyor
- [x] Onay/düzeltme yalnızca yetkili mentor tarafından, denetim kaydıyla yapılabiliyor (eski, yetkisiz yol kaldırıldı)
- [x] Permission yapısı master dokümanla birebir eşleşiyor, AppService'lere uygulandı
- [x] Stajyer/Mentor rolleri seed ediliyor, doğru izinlerle
- [x] Mentor "bekleyen incelemeler" listesini ve inceleme geçmişini görebiliyor
- [x] Domain.Tests 41/41, EntityFrameworkCore.Tests 56/56 geçiyor

## Bilinen sınırlama

Kullanıcının yerel `dotnet run` süreci gün boyunca açık kaldığı için `InternshipJournal.Web` projesinin bin çıktısını kilitledi — bu yüzden `Web.Tests`'i (yeni `MentorReviews` sayfalarını ve host açılışını doğrulayan) bugün çalıştıramadım. Kod tarafında derleme hatası (CS/RZ) sıfır olduğunu doğruladım, ama canlı doğrulama kullanıcıya kaldı.

## Yarın yapacaklarım (Gün 20 — teslim)

- Kullanıcıdan `Web.Tests` dahil tam test paketinin geçtiğini teyit almak.
- README/Obsidian son hâli, 4. Hafta değerlendirmesi.
- Final demo senaryosu ve teslim kontrol listesi.
