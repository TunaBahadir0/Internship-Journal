# Gün 12

Tarih: 18 Ağustos 2026, Salı

## Bugün tamamladığım işler

- `Workplace` aggregate'ini yazdım: Id, Name, TaxNumber, Phone, Email, Website, DistrictId, AddressLine, PostalCode, Latitude, Longitude, IsActive alanları; Rename, ChangeContactInformation, ChangeAddress, ChangeCoordinates, Activate, Deactivate davranışları.
- Entity içinde e-posta formatını ve enlem/boylam aralığını (`-90..90`, `-180..180`) doğrulayan private `SetEmail`/`SetCoordinates` metotları yazdım; hatalı değerlerde `BusinessException` fırlatılıyor.
- `WorkplaceManager` domain servisini yazdım: District'in var/aktif olduğunu, Province ve Country'nin aktif olduğunu ve çalışma yeri adının benzersiz olduğunu doğruluyor (cross-aggregate kontroller).
- `IWorkplaceRepository` ve `WorkplaceRepository`'yi yazdım: `FindByNameAsync`, `IsNameInUseAsync`, `GetWithLocationAsync`, `GetListWithLocationAsync`. Son ikisi Workplace + District + Province + Country'yi tek sorguda join'leyip `WorkplaceWithLocation` projeksiyonuna dönüştürüyor.
- `AppWorkplaces` tablosunu (`DistrictId` foreign key + Restrict, `Name` unique index) EF Core mapping'e ve migration'a ekledim.
- Contracts: `WorkplaceDto`, `WorkplaceDetailDto`, `CreateWorkplaceDto`, `UpdateWorkplaceDto`, `GetWorkplaceListInput`, `IWorkplaceAppService`.
- `WorkplaceAppService`'i yazdım: GetAsync, GetListAsync, CreateAsync, UpdateAsync, ActivateAsync, DeactivateAsync.
- Razor Pages: `Pages/Workplaces/Index`, `Create`, `Edit`, `Detail`; nested ülke → il → ilçe seçimi için `wwwroot/js/workplace-location-selector.js` (ABP dinamik JS proxy'si üzerinden `LocationAppService` çağrılıyor).
- Menüye "Çalışma Yerleri" girişini ekledim.
- Application Service testlerini yazdım (7 test) ve tam paket (Domain.Tests + Application.Tests + EntityFrameworkCore.Tests = 15 test, Web.Tests = 1 test) hatasız geçti.
- Postman/curl ile API'yi uçtan uca test ettim: oluşturma, listeleme, detay, güncelleme (ilçe değişikliği), pasifleştirme ve hata senaryoları (mükerrer ad → 403, olmayan ilçe → 403, geçersiz enlem → 400) doğru çalışıyor.

## Öğrendiğim / pekiştirdiğim konular

- **Yalnız DistrictId saklamak:** Workplace, Province/Country kimliklerini kendi içinde tutmuyor; tam adres her zaman `DistrictId → District.ProvinceId → Province.CountryId` ilişkisinden türetiliyor. Bu sayede "Türkiye / Ankara / Kadıköy" gibi tutarsız bir kombinasyon veritabanı seviyesinde hiç oluşamaz — çünkü Kadıköy'ün `ProvinceId`'si zaten sabit olarak İstanbul'u gösteriyor, Ankara'yı değil. Workplace bu ilişkiyi kendi başına asla çelişkili kuramaz.
- **Repository'nin custom sorguları neden generic'ten farklı:** `GetWithLocationAsync`/`GetListWithLocationAsync`, üç ayrı aggregate'i (Workplace, District, Province, Country) tek SQL sorgusunda join'liyor. Generic `IRepository<Workplace,Guid>` bunu yapamaz çünkü aggregate sınırları arasında navigasyon property'si kasıtlı olarak yok (DDD kuralı); bu yüzden join mantığı özel repository'de, `DbContext.Set<T>()` üzerinden elle yazıldı.
- **WorkplaceManager neden Application Service'te değil:** İlçe/il/ülke aktiflik kontrolü ve ad benzersizliği, birden fazla aggregate'i ilgilendiren iş kuralları; bunları Domain Service'e taşımak Application Service'i ince (thin) tutuyor ve aynı kural başka bir senaryodan (ör. ileride toplu içe aktarma) da tekrar kullanılabiliyor.
- **InsertAsync/UpdateAsync autoSave:** ABP'nin repository metotları varsayılan olarak `autoSave: false` çalışıyor (değişiklik, Unit of Work tamamlanınca yazılıyor). `CreateAsync` içinde entity eklendikten hemen sonra aynı metotta `GetWithLocationAsync` ile tekrar okumak istediğim için `autoSave: true` vermem gerekti; aksi halde henüz veritabanına yazılmamış satırı sorgulayıp "bulunamadı" hatası alıyordum. Bunu gerçek bir API çağrısıyla (Postman/curl) test ederken fark ettim.

## Alınan kararlar

1. `WorkplaceWithLocation` adında entity olmayan, düz bir okuma modeli (read model) tanımladım; repository join sorgusunun sonucunu doğrudan bu sınıfa projekte ediyor, Mapperly de bu sınıftan `WorkplaceDto`/`WorkplaceDetailDto`'ya map ediyor. Böylece entity hiçbir zaman UI/DTO katmanına sızmıyor.
2. `GetWorkplaceListInput`'ı `PagedResultRequestDto`'dan türettim (`Sorting` içermeyen versiyon), çünkü şu an tek sıralama kriteri (ada göre) var; dinamik sıralama için ek bir kütüphane (System.Linq.Dynamic.Core) eklemek bu aşamada gereksiz karmaşıklık olurdu.
3. Nested dropdown JS'i, sayfa özelinde değil `wwwroot/js/workplace-location-selector.js` altında paylaşımlı yazdım; Create ve Edit ekranları aynı mantığı tekrar etmeden kullanıyor. JS içinde hiçbir iş kuralı yok — yalnızca `change` event'i dinleme, API çağrısı ve dropdown doldurma; ilçe aktifliği ve ilişki doğruluğu her zaman sunucuda (`WorkplaceManager`) tekrar kontrol ediliyor.
4. Vergi no/telefon/e-posta/web sitesi gibi iletişim alanlarını `ChangeContactInformation` tek metodunda topladım; adres alanlarını `ChangeAddress` içinde ayrı tuttum, çünkü adres değişikliği `WorkplaceManager` üzerinden District doğrulaması gerektiriyor, iletişim bilgisi değişikliği gerektirmiyor.

## Yapay zekâ kullanımı

Repository join sorgusunun EF Core'a doğru çevrildiğini ve ABP'nin dinamik JS proxy'sinin (`internshipJournal.workplaces.workplace.*`, `internshipJournal.locations.location.*`) ürettiği gerçek metot/route isimlerini, uygulamayı bilgisayarımda çalıştırıp `/Abp/ServiceProxyScript` çıktısını inceleyerek doğruladım; tahmine dayanmadım. `EfCoreRepository<,,>` sınıfının bu ABP sürümünde `Volo.Abp.Domain.Repositories.EntityFrameworkCore` ad alanında olduğunu da derleme hatası üzerinden bulup düzelttim.

## Kabul kriteri kontrolü

- [x] Workplace oluşturuluyor
- [x] Nested seçim çalışıyor
- [x] Yalnız DistrictId saklanıyor
- [x] Detayda ülke, il ve ilçe gösteriliyor
- [x] Pasif konum seçilemiyor
- [x] Düzenleme ekranı mevcut adresi yüklüyor
- [x] API üzerinden geçersiz seçim engelleniyor
- [x] Testler geçiyor

## Yarın yapacaklarım

- Gün 13: `InternProfile` aggregate'i, `DateRange` Value Object, staj dönemi ve mentor doğrulaması, kullanıcı başına tek aktif profil kuralı.
