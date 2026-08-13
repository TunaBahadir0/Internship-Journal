# Gün 8

Tarih: 12 Ağustos 2026, Çarşamba

## Bugün tamamladığım işler

- Tek büyük tablodan başlayıp 1NF/2NF/3NF adımlarını örnekle anlattım (`docs/database/normalization.md`).
- 11 tablonun tam kataloğunu (kolon, tip, null, PK/FK, unique, silme davranışı) çıkardım (`docs/database/table-catalog.md`).
- Constraint dokümanı (unique, FK, cascade/restrict gerekçeleri) yazdım (`docs/database/constraints.md`).
- İndeks dokümanı (sorgu bazlı, kolon sırası, yazma maliyeti) yazdım (`docs/database/indexes.md`).
- Graphviz ile ER diyagramını görsel ürettim (`docs/database/erd-v1.png`) + Mermaid metni (`erd-v1.mermaid`).

## Öğrendiğim teknik konular

- **1NF:** Atomiklik; `WorkItem1/WorkItem2`, `Skill1/Skill2` gibi tekrarlayan gruplar ayrı satırlara taşınır.
- **2NF:** Bağlantı tablosunda yalnızca ilişkiye ait alanlar durur (yetkinlik adı değil, SkillId).
- **3NF:** Geçişli bağımlılık (`District → Province → Country`) yüzünden çalışma yerinde yalnızca `DistrictId` tutulur; il/ülke türetilir. Bu, "Ankara/Kadıköy" gibi çelişkiyi baştan engeller.
- **Çok-çoğa ilişki:** Günlük–yetkinlik ilişkisi `AppDailyLogSkills` bağlantı tablosuyla çözülür; `(DailyLogId, SkillId)` unique.
- **Cascade vs Restrict:** Aggregate child'ları cascade (günlükle gider); referans veriler restrict (silinmez, pasifleştirilir).
- **İndeks maliyeti:** Her indeks yazmayı yavaşlatır; yalnızca gerçek sorguya karşılık gelen indeks eklenir.

## Karşılaştığım belirsizlik / karar

- "Tek aktif profil" (BR-8) klasik unique ile tam ifade edilemiyor → kısmi (filtered) unique + Domain Service kontrolü önerdim.
- MentorReview için cascade yerine restrict seçtim (ayrı aggregate, denetim kaydı).

## Yapay zekâ kullanımı

ER diyagramını Graphviz DOT ile oluşturdum; ilişkileri ve cascade/restrict etiketlerini tablo kataloğuyla karşılaştırarak doğruladım.

## Kabul kriteri kontrolü

- [x] 1NF, 2NF, 3NF açıklanıyor
- [x] Tekrarlanan alanlar kaldırıldı
- [x] Çok-çok ilişki bağlantı tablosuyla çözüldü
- [x] Primary ve foreign key'ler tanımlandı
- [x] Unique constraint'ler gerekçelendirildi
- [x] İndeksler sorgu ihtiyacına bağlandı
- [x] Adres hiyerarşisi tutarlı (yalnızca DistrictId)
- [x] Domain modeli ile ER modeli ayrı ele alındı

## Yarın yapacaklarım

- Gün 9 (13 Ağustos, Perşembe): Tasarım sunumu ve kontrol kapısı — analiz + domain + ER'yi mentora savunmak, geri bildirimle v2 üretmek.
