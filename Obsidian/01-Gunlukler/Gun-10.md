# Gün 10

Tarih: 14 Ağustos 2026, Cuma

## Bugün tamamladığım işler

- ABP çözüm yapısını inceledim ve projeleri belgeledim (`docs/abp/solution-structure.md`).
- Katman sorumluluklarını, v2 domain modeliyle eşleştirerek çıkardım (`docs/abp/layer-responsibilities.md`).
- Bir kullanım senaryosunu (günlük gönderme) katmanlar üzerinden uçtan uca izledim (`docs/abp/request-flow.md`).
- Mini proje ile ABP yapısını karşılaştırdım (`docs/abp/mini-project-comparison.md`).
- DbMigrator'ın amacını ve çalıştırma sırasını belgeledim (`docs/abp/dbmigrator.md`).
- ABP not çalışması sorularını cevapladım (`docs/abp/layer-responsibilities.md` içinde).

## Öğrendiğim / pekiştirdiğim konular

- **Katman bağımlılık yönü:** Alt katman üst katmanı bilmez; Domain, Web'e veya EntityFrameworkCore'a bağımlı olamaz.
- **DTO neden ayrı proje:** Application.Contracts, dış sözleşmeyi implementasyondan ayırır.
- **DbMigrator'ın rolü:** Migration + seed, uygulama başlamadan önce ayrı çalıştırılır.
- **Application Service ile Domain arasındaki sınır:** İş kuralı Domain'de yaşar, Application Service sadece koordine eder.
- **Permission ile veri sahipliği farkı:** Biri genel yetki, diğeri kayda özgü sahiplik kontrolü.

## Alınan kararlar

1. Aggregate'ler ve enum'lar Domain / Domain.Shared ayrımına göre dağıtıldı.
2. Örnek istek akışı (DailyLog Submit) referans senaryo olarak seçildi.
3. DbMigrator'ın Web'den önce, her migration sonrası çalıştırılacağı netleştirildi.

## Yapay zekâ kullanımı

ABP katman sorumluluklarını ve not çalışması cevaplarını, `docs/domain/domain-model-v2.md` ve `docs/database/table-catalog.md` ile çapraz kontrol ederek önceki günlerin tasarımıyla tutarlı kalmasını sağladım.

## Kabul kriteri kontrolü

- [x] ABP çözüm yapısı belgelendi
- [x] Katman sorumlulukları v2 tasarımla eşleştirildi
- [x] İstek akışı uçtan uca örneklendi
- [x] Mini proje karşılaştırması yapıldı
- [x] DbMigrator adımları ve zamanlaması netleştirildi
- [x] ABP not çalışması soruları cevaplandı

## Yarın yapacaklarım

- 2. hafta tamamlandı; haftalık demo ve haftalık değerlendirme yapıldı (`Obsidian/10-Haftalik-Degerlendirmeler/Hafta-02.md`).
- 3. hafta: kod geliştirmeye başlama (Domain katmanı entity implementasyonları).
