# Gün 9

Tarih: 13 Ağustos 2026, Perşembe

## Bugün tamamladığım işler

- Analiz + domain + veritabanı tasarımını sunum akışına göre savundum (`docs/decisions/design-review.md`).
- Kritik soruları (domain + veritabanı) cevapladım.
- Geri bildirimleri kayıt altına aldım ve tasarımı güncelledim.
- Güncellenmiş domain modelini v2 olarak yazdım (`docs/domain/domain-model-v2.md`).
- Güncellenmiş ER diyagramını v2 olarak ürettim (`docs/database/erd-v2.png`).

## Öğrendiğim / pekiştirdiğim konular

- **Kontrol kapısı (design review) neden önemli:** Koda geçmeden tasarımı savunmak, yanlış bir modeli sonradan pahalıya düzeltmekten korur.
- **Uygulama kontrolü + veritabanı kısıtı birlikte:** Tek aktif profil gibi kurallar yarış durumuna karşı DB'de de garantilenmeli (kısmi unique index).
- **Kardinalite netliği:** MentorReview'ı 1..* yapmak inceleme geçmişini korur.
- **Soft-delete:** ABP varsayılanı; "kullanılan veri silinmez" kuralını destekler.

## Alınan kararlar (v2)

1. InternProfiles(UserId) için kısmi unique (Status=Active) + servis kontrolü.
2. DailyLog ↔ MentorReview ilişkisi 1..* (inceleme geçmişi).
3. Soft-delete varsayılan; FK davranışları tanımlı kalır.
4. DailyLogItem.WorkType 8 değerli ana proje enum'una hizalandı.

## Yapay zekâ kullanımı

Kritik soru cevaplarını ve v2 kararlarını, önceki günlerin dokümanlarıyla (business-rules, invariants, table-catalog) çapraz kontrol ederek tutarlılığını doğruladım.

## Kabul kriteri kontrolü

- [x] Kritik sorular cevaplandı
- [x] Tasarım geri bildirimlere göre güncellendi (v2)
- [x] Domain ve veritabanı modeli farkı açıklanabiliyor
- [x] Ana iş kuralları korunuyor
- [x] Kod geliştirmeye geçiş için tasarım onaylandı

## Yarın yapacaklarım

- Gün 10 (14 Ağustos, Cuma): ABP Framework temelleri — çözüm yapısı, katman sorumlulukları, DbMigrator, mini proje karşılaştırması; onaylı tasarımın katmanlara yerleştirilmesi.
