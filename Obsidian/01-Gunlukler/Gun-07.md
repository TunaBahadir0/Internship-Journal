# Gün 7

Tarih: 12 Ağustos 2026, Çarşamba

## Bugün tamamladığım işler

- 11 kavramı Entity / Value Object / Aggregate Root / Child Entity olarak sınıflandırdım (`docs/domain/domain-model.md`).
- Aggregate sınırlarını çizdim ve her aggregate için karar dokümanı yazdım (`docs/domain/aggregate-decisions.md`).
- Invariant'ları çıkardım ve hangi aggregate'in koruduğunu belirttim (`docs/domain/invariants.md`).
- 4 Domain Service'i (WorkplaceManager, InternProfileManager, DailyLogManager, MentorReviewManager) gerekçelendirdim (`docs/domain/domain-services.md`).

## Öğrendiğim teknik konular

- **Entity vs Value Object:** Entity'nin kimliği var ve değişir; Value Object (DateRange) kimliksiz, değeriyle tanımlı ve değişmez.
- **Aggregate ve Root:** Birlikte tutarlı kalması gereken nesneler bir kümedir; dışarıya tek kapı Aggregate Root'tur. DailyLog kök; madde/yetkinlik/problem onun child'ı.
- **Her tablo Aggregate Root değildir:** DailyLogItem tablo olacak ama child; root üzerinden yönetilir.
- **Invariant vs Domain Service kuralı:** Aggregate yalnızca kendi verisiyle koruyabildiği kuralı invariant olarak taşır; başka aggregate'e bakmak gerekince Domain Service devreye girer.
- **Aggregate'ler birbirine Id ile referans verir**, nesne referansıyla değil.

## Verdiğim önemli kararlar

- **DailyLog** ana aggregate; child'ları toplam süre invariant'ı (I-1) için hep root üzerinden değişir.
- **MentorReview** ayrı aggregate; inceleme→durum değişimi koordinasyonu MentorReviewManager'da. (Geçen hafta mentora sorduğum aggregate sınırı sorusunun cevabı: ayrı aggregate + Domain Service.)
- **Country/Province/District** ayrı referans aggregate'leri; çoğunlukla seed.
- **DateRange** value object; staj dönemi için.

## Karşılaştığım belirsizlik

- "Tek aktif profil" kuralının invariant değil Domain Service kuralı olduğunu fark ettim (tek profil içinde görülemez). InternProfileManager'a taşıdım.

## Yapay zekâ kullanımı

Sınıflandırma tablosunu ve invariant/Domain Service ayrımını, ana proje dokümanındaki temel sınıflarla (FullAuditedAggregateRoot, Entity<Guid>) karşılaştırarak doğruladım.

## Kabul kriteri kontrolü

- [x] Entity/Value Object ayrımı açıklanıyor
- [x] Aggregate sınırları çizildi
- [x] Her aggregate'ın sorumluluğu yazıldı
- [x] İş kuralları ilgili modele bağlandı (BR referansları)
- [x] Child entity erişim yöntemi açıklandı (root üzerinden)
- [x] Domain Service adayları gerekçelendirildi
- [x] Her tablo Aggregate Root yapılmadı

## Yarın yapacaklarım

- Gün 8: Normalizasyon (1NF/2NF/3NF), ER diyagramı, tablo kataloğu, constraint ve index tasarımı.
