# Yapay Zekâ Kullanım Raporu

Program boyunca (`00-Stajyer-Program-Rehberi.md` bölüm 7'deki kurala göre: önce problem kendi cümleleriyle yazılır, hata mesajı/bağlam incelenir, sınırlı bir istek yapılır, öneri doğrulanır, sonuç açıklanır) yapay zekâ kullanımının izi her günün kendi günlüğündeki **"Yapay zekâ kullanımı"** bölümünde tutuldu (`Obsidian/01-Gunlukler/Gun-XX.md`). Bu rapor onların yerine geçmez — hangi günde ne olduğunun ayrıntısı için ilgili günlüğe bakılmalıdır. Burada, doğrulanabilir ve bu oturumda doğrudan gözlemlenen (Gün 14-20) somut örnekler üzerinden **yöntemin kendisi** özetleniyor.

## Yöntem: doğrula, varsayma

Bu programda tekrar eden ilke, bir yapay zekâ önerisini veya kendi ilk tahminimi **çalıştırıp gerçek sonucu görmeden** doğru kabul etmemekti. Somut örnekler:

- **Gün 14** — `DomainService.GuidGenerator`'ın DI container olmadan neden `NullReferenceException` verdiğini varsaymadım; testi olduğu gibi çalıştırıp gerçek stack trace'i (`get_GuidGenerator()` içinde null referans) okuyarak kök nedeni buldum, sonra düzelttim.
- **Gün 17** — Kullanıcının yöneticisinin bildirdiği "ham exception metni görünüyor" hatasının kök nedenini (`BusinessException.Message`'ın ABP'nin HTTP pipeline'ı dışında otomatik lokalize olmadığı) ilk denemede yanlış teşhis ettim (`EmbeddedResource` eksikliği sandım) — bu düzeltme işe yaramayınca geri aldım, log dosyasını (`obj/.../staticwebassets.build.json`) inceleyip gerçek kök nedeni (ABP'nin bundling alt sisteminin Static Web Assets ile tam entegre olmaması) bulup doğru düzeltmeyi (`abp-script` yerine düz `<script>` etiketi) uyguladım.
- **Gün 18** — `MentorReview`'in `Approve`/`RequestRevision`'ını instance metodu mu yoksa factory metodu mu yapacağıma karar verirken, test çalıştırıp gerçek exception türünü (`AbpValidationException`, beklediğim `BusinessException` değil) gördükten sonra hem kodu hem test beklentimi düzelttim.
- **Gün 19** — `IPermissionManager`'ın gerçek namespace'ini ve rol sağlayıcı adını ("R") hiçbir dokümanda bulamayınca, küçük bir C# konsol projesiyle ilgili NuGet paketini indirip **reflection ve IL byte analizi** ile gerçek API'yi doğrudan doğruladım, varsayımla ilerlemedim.

## Yapay zekânın kullanıldığı, kullanılmadığı alanlar

**Kullanıldı:** kod üretimi (her günün aggregate/repository/AppService/test kodu), hata ayıklama (stack trace okuma, log analizi), tasarım kararlarının önceki günlerin dokümanlarıyla (table-catalog.md, constraints.md) tutarlılığının kontrolü, dokümantasyon (günlükler, bu rapor dahil).

**Kullanılmadı / insana bırakıldı:** hangi günün hangi kapsamı alacağına dair nihai karar (kullanıcı onayı gerekti, ör. Gün 16 ve Gün 18-20 zaman çizelgesi sıkıştırması), canlı tarayıcı testi ve manuel doğrulama (bu oturumda çalışan bir tarayıcı/Postgres erişimi olmadığı için — bkz. `Gun-17.md`, `Gun-19.md`), gerçek bir "gönder" işlemi (Gmail draft-only kuralı, iş başvurusu görevlerinde — bu proje ile ilgisiz ama aynı oturumda geçerli olan bir kural).

## Doğrulama disiplini

Her önemli teknik iddia, mümkün olduğunda şu yollardan biriyle doğrulandı, salt "AI böyle dedi" ile bırakılmadı:

1. Testi çalıştırıp gerçek sonucu okumak (en sık kullanılan yöntem).
2. Mevcut kod tabanındaki bir emsali (precedent) bulup ona uymak.
3. Önceden yazılmış tasarım dokümanlarıyla (Gün 7-9) karşılaştırmak.
4. Belirsiz bir framework API'si için reflection/IL inceleme ile gerçek imzayı doğrulamak.

## Sınır

Bu raporun kapsadığı somut örnekler, bu oturumda doğrudan gözlemlenen Gün 14-20 aralığıyla sınırlıdır. Gün 6-13 için AI kullanım kayıtları kendi günlüklerinde mevcuttur; burada tekrar özetlenmedi çünkü bu raporun yazarı o günlerin ayrıntısını (context sıkıştırması nedeniyle) güvenilir şekilde hatırlamıyor — var olmayan bir detayı uydurmak yerine ilgili günlüğe yönlendirmek tercih edildi.
