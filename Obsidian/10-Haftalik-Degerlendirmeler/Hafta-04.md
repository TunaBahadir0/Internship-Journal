# 4. Hafta Değerlendirmesi

Tarih: 26 Ağustos 2026, Çarşamba (Gün 16 - Gün 20, teslim haftası)

> Not: 4. Hafta için (1-3. haftaların aksine) ayrı, gün gün detaylı bir müfredat dosyası yoktu — yalnızca `00-Stajyer-Program-Rehberi.md`'deki tek satırlık özet ("Razor Pages, mentor akışı, test ve teslim") vardı. Bu yüzden bu değerlendirme, o dosyanın genel haftalık değerlendirme şablonunu (bölüm 13) kullanıyor.

## Bu hafta öğrendiğim en önemli üç konu

1. **Application Service'in "ince" katman olması pratikte ne demek:** Gün 16'da `DailyLogAppService`'e 13 metot eklerken hiçbiri yeni bir iş kuralı içermedi — hepsi zaten Gün 14'te aggregate'in içindeydi. Application katmanı yalnızca domain davranışını güvenli şekilde açığa çıkarıyor, kural üretmiyor.
2. **Bir framework API'sini varsayımla değil doğrulayarak kullanmak:** Gün 19'da `IPermissionManager`'ın gerçek namespace'ini ve rol sağlayıcı adını bulmak için reflection ve IL byte analizi yapmak zorunda kaldım — resmi dokümantasyonun her zaman yeterli olmadığı, gerçek assembly'yi incelemenin bazen tek güvenilir yol olduğu bir deneyimdi.
3. **Güvenlik açıklarının "yeni özellik eklerken" değil "eski özelliği unutup yeni bir yol açarken" oluşabileceği:** Gün 16'da yazdığım `DailyLogAppService.Approve/RequestRevision`, Gün 18'de doğru/yetkili yolu (`MentorReviewAppService`) yazana kadar zararsızdı — ama iki yol bir arada var olduğunda biri sessizce güvenlik açığına dönüştü. Bunu ancak "sahiplik kontrollerini tüm metotlara tutarlı uygula" görevini yaparken fark ettim.

## En zorlandığım konu

`abp-script`/Static Web Assets uyumsuzluğu (Gün 17/19 arası, yönetici geri bildirimiyle ortaya çıkan Workplace dropdown hatası). İlk düzeltme denemem (EmbeddedResource ekleme) yanlıştı ve işe yaramadı; gerçek kök nedeni bulmak için ABP'nin build çıktısındaki manifest dosyasını (`staticwebassets.build.json`) incelemem, ve bunun ABP'nin bundling alt sistemiyle .NET 10'un yeni statik varlık modelinin tam entegre olmadığını göstermesi gerekti. Yanlış ilk denemeyi sessizce bırakmak yerine geri alıp doğru çözümü bulmak, bu haftanın en çok zaman alan tekil sorunuydu.

## Çözdüğüm en önemli problem

Gün 19'da bulduğum, `DailyLogAppService`'teki yetkisiz `Approve`/`RequestRevision` çiftinin `MentorReviewAppService`'in mentor-doğrulamalı yolunu tamamen atlaması. Bu, görevi yaparken (sahiplik kontrolü eklerken) tesadüfen bulunan ama teslimden önce mutlaka kapatılması gereken gerçek bir güvenlik açığıydı.

## Daha iyi yapabileceğim konu

Canlı tarayıcı doğrulamasını daha erken ve daha sık yapabilirdim — bu hafta boyunca (Gün 17, 19) kullanıcının kendi ortamında test etmesine bağımlı kaldım çünkü bu oturumda doğrudan bir Postgres/tarayıcı erişimim yoktu. Bazı hatalar (dropdown boşluğu, ham exception metni) yalnızca gerçek kullanım sırasında ortaya çıktı; bunları öngörmek için daha fazla "bu ekranı gerçekten kullanan biri ne görür" sorusu sorabilirdim.

## Gelecek hafta hedefim

(Program 4 hafta ile bitiyor — "gelecek hafta" yerine, teslim sonrası olası bir devam senaryosu için:) Rol ataması için ayrı bir yönetim ekranı, menü öğelerinin izin bazlı gizlenmesi, ve `docs/security-checklist.md`'de "bilinen sınırlama" olarak işaretlenen Workplace/InternProfile sahiplik kontrollerinin netleştirilmesi.

## Teknik öz değerlendirme

- **C#:** Aggregate/Manager/Repository/AppService kalıbını 20 gün boyunca tekrar tekrar uygulayarak içselleştirdim; factory metodu vs. instance mutator ayrımını (Gün 18) ilk kez bilinçli bir tasarım kararı olarak verdim.
- **ASP.NET Core / ABP:** Permission sistemi, rol seed'i, çoklu form validasyonu (Gün 17), Static Web Assets/bundling ayrımı (Gün 19) bu hafta netleşti.
- **Veritabanı:** `MentorReview`'ın Restrict, child entity'lerin Cascade FK davranışının nedenini (ayrı aggregate vs. aynı aggregate'in parçası) artık örnekle açıklayabiliyorum.
- **Git:** Her gün ayrı branch+PR akışı, bir merge edilmiş branch'e sonradan commit atma hatasını (Gün 16 sonu) fark edip yeni bir branch'e cherry-pick ile taşıyarak düzeltme deneyimi kazandım.
- **Test:** Üç katmanlı test stratejisini (saf/mock/tam DI) bilinçli olarak seçip belgeleyebiliyorum (`docs/test-strategy.md`).
- **Dokümantasyon:** Her günün "ne yaptım" değil "neden bu şekilde yaptım, hangi alternatifi neden elemedim" sorularına cevap verecek şekilde günlük tutmak.
- **Problem çözme:** Yanlış bir ilk çözümü (Gün 19'daki EmbeddedResource denemesi) fark edip geri alma disiplinini uyguladım.

## Haftalık kabul kriterleri

- [x] DailyLog uçtan uca ekrana taşındı (madde ekleme, Submit, Approve/RequestRevision)
- [x] MentorReview akışı (Domain'den ekrana kadar) çalışıyor
- [x] Permission/sahiplik kontrolleri tüm ilgili metotlarda tutarlı
- [x] Tam çözüm 0 hata, 100/100 test geçiyor (Domain 41, EFCore 56, Web 3)
- [x] Final teslimatları (test stratejisi, güvenlik kontrol listesi, AI kullanım raporu, final demo senaryosu, docker-compose) hazır
- [x] README ve Obsidian güncel
