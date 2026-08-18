# Gün 13

Tarih: 18 Ağustos 2026, Salı

## Bugün tamamladığım işler

- `DateRange` Value Object'ini yazdım: `StartDate`/`EndDate` (get-only), `Contains(date)`, `Overlaps(other)`, `DurationInDays()`; bitiş başlangıçtan önceyse `BusinessException(InvalidDateRange)` fırlatıyor. EF Core'da `OwnsOne` ile `InternshipPeriod_StartDate`/`InternshipPeriod_EndDate` kolonlarına gömülü (owned type) olarak mapleniyor.
- `InternProfile` aggregate'ini yazdım: UserId, MentorUserId, WorkplaceId, University, SchoolDepartment, StudentNumber, InternshipPeriod (DateRange), RequiredWorkDays, Status; ChangeMentor/ChangeWorkplace/ChangeEducationInformation/ChangeInternshipPeriod/ChangeRequiredWorkDays/Start/Complete/Cancel davranışları.
- Identity verileri (ad, e-posta vb.) tekrar tutulmuyor — profil yalnızca `UserId` ile Identity kullanıcısına bağlanıyor.
- `InternProfileManager` domain servisini yazdım: Workplace var/aktif mi, mentor kullanıcısı var mı, kullanıcının zaten aktif profili var mı kontrolleri; mentor ile stajyerin aynı kullanıcı olamayacağı kontrolü entity içinde.
- `IInternProfileRepository`/`InternProfileRepository`'yi yazdım: `FindByUserIdAsync`, `GetActiveByUserIdAsync`, `HasActiveProfileAsync`, `GetWithWorkplaceAsync` (Workplace+adres+mentor join, stajyerin kendi kimliği hariç), `GetWithMentorAndWorkplaceAsync` (tam join, admin detay ekranı için).
- `AppInternProfiles` tablosunu ekledim: `WorkplaceId` foreign key (Restrict), kullanıcı başına **kısmi (filtered) unique index** (`UserId` üzerinde, yalnız `Status = 1` yani Active iken) — Gün 9'da alınan karar uygulamaya geçti.
- Contracts: `InternProfileDto`, `InternProfileDetailDto`, `CreateInternProfileDto`, `UpdateInternProfileDto`, `GetInternProfileListInput`, `IInternProfileAppService`.
- `InternProfileAppService`'i yazdım: GetMyProfileAsync (CurrentUser.Id üzerinden), GetAsync, GetListAsync, CreateAsync, UpdateAsync, StartAsync, CompleteAsync, CancelAsync.
- Razor Pages: Stajyer tarafı `Pages/Profile/Index` (salt okunur) ve `Pages/Profile/Edit` (yalnızca eğitim bilgileri düzenlenebilir; Mentor/Workplace/tarihler hidden alanlarla korunuyor); Admin tarafı `Pages/InternProfiles/Index` (Başlat/Tamamla/İptal butonlarıyla), `Create`, `Edit`, `Detail`.
- Menüye "Staj Profilleri" (admin) ve "Profilim" (stajyer) girişlerini ekledim; her iki taraf da `[Authorize]` ile korunuyor.
- Testler: `DateRangeTests` (7 test, saf unit test — DI gerekmiyor) + `InternProfileAppServiceTests` (8 test). Tam paket: Domain.Tests 7, Application+EFCore.Tests 23, Web.Tests 1 — hepsi geçiyor.

## Öğrendiğim / pekiştirdiğim konular

- **Value Object'i EF Core'da owned type olarak maplemek:** `DateRange`'in get-only property'leri ve tek constructor'ı, EF Core'un constructor binding özelliği sayesinde parametresiz constructor'a ihtiyaç duymadan çalıştı — entity'lerde kullandığımız `protected boş ctor + private setter` deseninden farklı, gerçek anlamda immutable bir model kurulabildi.
- **Kısmi unique index + Domain Service birlikte çalışıyor:** Veritabanı seviyesinde `Status = 1` filtreli unique index, `InternProfileManager.ValidateNoActiveProfileAsync` kontrolünü atlayan bir yarış durumunu (race condition) da yakalar; ikisi birbirinin yedeği, biri diğerinin yerini tutmuyor.
- **GetMyProfileAsync ile GetAsync'in farklı repository sorguları kullanma nedeni:** Stajyer kendi profiline bakarken kendi ad/e-posta bilgisini zaten `CurrentUser`'dan biliyor, bu yüzden `GetWithWorkplaceAsync` stajyerin kendi Identity join'ini yapmıyor (daha hafif sorgu); admin ise profilin sahibini tanımadığı için `GetWithMentorAndWorkplaceAsync` her iki kullanıcının da kimliğini join'liyor.
- **ABP modülleri arası FK kurmamak:** `UserId`/`MentorUserId` alanları `AbpUsers` tablosuna foreign key ile bağlanmadı — ABP konvansiyonu, modüller (Identity burada ayrı bir modül) arasında gerçek FK kurmamak, yalnızca Id ile referans vermek yönünde; bu modüllerin bağımsız değiştirilebilirliğini korur.
- **[Authorize] eksikliği önce 500 olarak ortaya çıktı:** Sayfaları ilk yazdığımda `CurrentUser.GetId()` ve Identity'nin kendi izin kontrolleri, girişsiz istekte "Nullable object must have a value" / yetkilendirme istisnası olarak 500 döndürüyordu. `[Authorize]` eklemek bunu düzgün bir login yönlendirmesine çevirdi — gerçek bir tarayıcı/oturum testinde bu, kullanıcıya normal bir "giriş yap" akışı olarak görünür.

## Alınan kararlar

1. `UpdateInternProfileDto` tek bir DTO; hem admin hem stajyer Edit ekranı aynı `UpdateAsync` metodunu çağırıyor. Stajyer formunda yalnızca eğitim alanları (`University`/`SchoolDepartment`/`StudentNumber`) görünür/düzenlenebilir; Mentor/Workplace/tarih/gerekli-gün alanları hidden input olarak mevcut değerleriyle formda taşınıyor, böylece stajyer bunları değiştiremiyor ama ayrı bir DTO/metot çoğaltmaya gerek kalmıyor. Bu basitleştirme bilinçli bir tercih — daha sağlam bir çözüm (sunucu tarafında alan bazlı yetki) ileride eklenebilir.
2. "Kullanıcı için yalnız bir aktif profil" kuralı, literal olarak `Status == Active` üzerinden kontrol ediliyor (Draft durumundaki birden fazla profil şu an engellenmiyor) — repository metodunun adı (`HasActiveProfileAsync`) ve dokümandaki ifade bu yorumu destekliyor.
3. `GetWorkplaceListInput` (Gün 12) gibi, `GetInternProfileListInput` da `PagedResultRequestDto`'dan türetildi (Sorting olmadan); liste her zaman staj başlangıç tarihine göre azalan sıralanıyor.

## Yapay zekâ kullanımı

`DateRange` value object'in EF Core owned type mapping'inin gerçekten çalışıp çalışmadığını (constructor binding, kolon adları) migration'ı üreterek ve içeriğini inceleyerek doğruladım; varsayımla bırakmadım. `InternProfileAppServiceTests`'teki test kullanıcılarını gerçek `IdentityUserManager.CreateAsync` ile oluşturdum (mock değil), böylece mentor/stajyer doğrulama kuralları gerçek Identity altyapısına karşı test edildi.

## Kabul kriteri kontrolü

- [x] Profil oluşturuluyor
- [x] Çalışma yeri bağlantısı doğru
- [x] Mentor bağlantısı doğru
- [x] Staj dönemi doğrulanıyor
- [x] Aynı kullanıcı için ikinci aktif profil engelleniyor
- [x] Profil detayında tam adres gösteriliyor
- [x] Durum geçişleri kontrollü
- [x] Testler geçiyor

## Veri sahipliği hazırlığı

İlerleyen günlerde stajyerin yalnız kendi günlüklerine erişebilmesi için kurulacak ilişki:

```text
CurrentUser.Id
   ↓
InternProfile.UserId
   ↓
DailyLog.InternProfileId
```

`GetMyProfileAsync` bugün bu ilişkinin ilk halkasını (`CurrentUser.Id → InternProfile.UserId`) zaten kullanıyor; Gün 14'te `DailyLog.InternProfileId` eklenince zincir tamamlanacak.

## Yarın yapacaklarım

- Gün 14: `DailyLog` aggregate'i ve child entity'leri (`DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry`), durum geçişleri (Draft → Submitted → Approved/RevisionRequested), toplam süre hesaplama, `DailyLogManager`.
