# DbMigrator

## Ne işe yarar

`InternshipJournal.DbMigrator`, ayrı çalıştırılabilir bir konsol projesidir. İki işi vardır:

1. Bekleyen EF Core migration'larını veritabanına uygular (`dbContext.Database.MigrateAsync()`).
2. Seed veriyi yükler: referans veriler (`Country`/`Province`/`District`), varsayılan roller ve izinler (`PermissionDataSeeder`), ilk admin kullanıcı.

## Ne zaman çalıştırılır

- Veritabanı ilk kurulurken (boş veritabanına şema + seed veri için).
- Her yeni migration eklendiğinde, `Web`/`HttpApi` projesi başlatılmadan önce.
- Ortam değiştiğinde (yeni geliştirici ortamı, CI, staging) veritabanını güncel şemaya getirmek için.

`Web` projesi migration uygulamaz; sadece var olan şemayı kullanır. Bu ayrım, uygulamanın her başlangıcında istemeden şema değişikliği yapılmasını engeller.

## Çalıştırma adımları

1. PostgreSQL container'ının çalıştığından emin ol.
2. `DbMigrator/appsettings.json` içindeki connection string'in `Web/appsettings.json` ile aynı veritabanını gösterdiğini doğrula.
3. `dotnet run` ile `InternshipJournal.DbMigrator` projesini çalıştır.
4. Konsol çıktısında migration'ların uygulandığını ve seed adımlarının tamamlandığını doğrula.
5. DBeaver ile veritabanına bağlanıp `AppCountries`, `AppProvinces`, `AppDistricts` gibi tabloların seed veriyle dolduğunu, ABP'nin kendi tablolarının (`AbpUsers`, `AbpRoles`, `AbpPermissionGrants`) oluştuğunu kontrol et.
6. Ardından `InternshipJournal.Web` projesini başlat ve admin hesabıyla giriş yap.

## Seed veri ile `table-catalog.md` ilişkisi

`AppCountries`, `AppProvinces`, `AppDistricts` referans tabloları uygulama açılışında boş olamaz — akış (`Workplace` oluşturma, `InternProfile` kayıt) bu referans verilere bağımlıdır. Bu yüzden seed adımı, migration'dan hemen sonra, uygulama ilk kez kullanılmadan önce mutlaka çalıştırılmalıdır.
