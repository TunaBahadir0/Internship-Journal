# Staj Günlüğü ve Gelişim Takip Uygulaması (Internship-Journal)

Stajyer Gelişim Programı'nın **ana projesi**. Bir stajyerin staj süresince yaptığı çalışmaları günlük olarak kaydetmesini, öğrendiği yetkinlikleri ve karşılaştığı problemleri belgelemesini; mentorun bu günlükleri inceleyip onaylamasını/düzeltme istemesini sağlar.

Depo: https://github.com/TunaBahadir0/Internship-Journal

```bash
git clone https://github.com/TunaBahadir0/Internship-Journal.git
```

> Bu depo şu an **2. hafta (analiz ve tasarım)** aşamasındadır. Henüz uygulama kodu yoktur; çıktılar analiz, domain ve veritabanı tasarım dokümanlarıdır. Kod (ABP Framework ile) 3. haftada başlayacaktır.
>
> Depo herkese açıktır (public). Bu yüzden gerçek şifre, bağlantı anahtarı veya şirket sırrı kesinlikle repoya eklenmez; hassas ayarlar `.gitignore` ile dışlanır.

## Proje yapısı

```text
StajGunlugu/
├── docs/
│   ├── analysis/     # Gün 6 — gereksinim analizi (aktörler, senaryolar, kurallar, ortak dil, wireframe)
│   ├── domain/       # Gün 7 — DDD domain modeli (aggregate, invariant, domain service)
│   ├── database/     # Gün 8 — normalizasyon, ER diyagramı, tablo kataloğu, constraint, index
│   ├── decisions/    # Gün 9 — tasarım sunumu ve kararlar
│   └── abp/          # Gün 10 — ABP çözüm yapısı ve katman sorumlulukları
└── Obsidian/         # Günlük notlar, DDD/veritabanı notları, haftalık değerlendirme
```

## Hedef tasarım (özet)

11 uygulama tablosu: konum (`Country → Province → District`) + ana akış (`Workplace`, `InternProfile`, `Skill`, `DailyLog` (+ `DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry`), `MentorReview`).

Ana aggregate: **DailyLog** ve child'ları. Günlük durumları: Draft → Submitted → (Approved | RevisionRequested → Draft).

## Haftalık ilerleme

- **Gün 6:** Gereksinim analizi ve ortak dil — `docs/analysis/`
- **Gün 7:** DDD ve domain modelleme — `docs/domain/`
- **Gün 8:** Normalizasyon ve veritabanı tasarımı — `docs/database/`
- **Gün 9:** Tasarım sunumu ve kontrol kapısı — `docs/decisions/`
- **Gün 10:** ABP Framework temelleri — `docs/abp/`
