# Gün 6

Tarih: 10 Ağustos 2026, Pazartesi

## Bugün tamamladığım işler

- Ana projenin (Staj Günlüğü ve Gelişim Takip Uygulaması) gereksinim analizini yaptım.
- Üç aktörü tanımladım: Stajyer, Mentor, Admin (`docs/analysis/actors.md`).
- 11 kullanım senaryosu yazdım; her biri ana akış + alternatif + hata + kabul kriteriyle (`docs/analysis/use-cases.md`).
- 27 iş kuralı çıkardım ve senaryolara bağladım (`docs/analysis/business-rules.md`).
- Ortak dil sözlüğünü hazırladım (`docs/analysis/ubiquitous-language.md`).
- 7 ekranın basit taslağını çizdim (`docs/analysis/wireframes.md`).
- Belirsiz noktaları mentora sorular hâline getirdim (`docs/analysis/open-questions.md`).

## Öğrendiğim teknik konular

- **Gereksinim ≠ çözüm:** Önce "kullanıcı ne istiyor" yazılır; sınıf/tablo daha sonra gelir.
- **İş kuralı:** Tek ekranın değil, tüm sistemin uyması gereken kısıt (örn. "aynı gün tek günlük"). UI'da değil, domain/serviste korunmalı.
- **Kullanım senaryosunda alternatif ve hata akışları**, sadece mutlu yolu değil, gerçek dünyayı da modeller.
- **Ortak dil**, ekipte kelime karışıklığını önler.

## Karşılaştığım belirsizlikler

- Toplam sürenin kullanıcıdan mı alınacağı yoksa hesaplanacağı mı (hesaplanan lehine karar verdim, mentora sordum).
- Tek madde için üst süre sınırının ne olacağı (mentora soru olarak bıraktım).
- Adresin nasıl saklanacağı (yalnızca DistrictId ile çözdüm; çelişkiyi baştan engelliyor).

## Uyguladığım yaklaşım

- Belirsizlikleri silmedim, "önerim + mentora soru" olarak kaydettim.
- Kabul kriterlerini (≥8 senaryo, ≥15 kural, 3 aktör) sağladım: 11 senaryo, 27 kural, 3 aktör.
- Erken çözüm sınıfı/tablo belirlemedim (kabul kriterine uygun).

## Yapay zekâ kullanımı

Analiz dokümanlarının yapısını (senaryo şablonu, iş kuralı alanları) dokümandaki şablonlarla karşılaştırıp doğruladım; içerik projenin kendi gereksinimlerinden çıkarıldı.

## Yarın yapacaklarım

- Gün 7: DDD — 11 kavramı Entity / Value Object / Aggregate olarak sınıflandırmak.
- DailyLog aggregate sınırını ve invariant'ları netleştirmek.
- Domain Service adaylarını (WorkplaceManager, InternProfileManager, DailyLogManager, MentorReviewManager) gerekçelendirmek.
