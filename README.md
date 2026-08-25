# Staj Günlüğü ve Gelişim Takip Uygulaması (Internship-Journal)

Stajyer Gelişim Programı'nın **ana projesi**. Bir stajyerin staj süresince yaptığı çalışmaları günlük olarak kaydetmesini, öğrendiği yetkinlikleri ve karşılaştığı problemleri belgelemesini; mentorun bu günlükleri inceleyip onaylamasını/düzeltme istemesini sağlar.

Depo: https://github.com/TunaBahadir0/Internship-Journal

```bash
git clone https://github.com/TunaBahadir0/Internship-Journal.git
```



## Proje yapısı

```text
StajGunlugu/
├── docs/
│   ├── analysis/     # Gün 6 — gereksinim analizi (aktörler, senaryolar, kurallar, ortak dil, wireframe)
│   ├── domain/       # Gün 7 — DDD domain modeli (aggregate, invariant, domain service)
│   ├── database/     # Gün 8 — normalizasyon, ER diyagramı, tablo kataloğu, constraint, index
│   ├── decisions/    # Gün 9 — tasarım sunumu ve kararlar
│   └── abp/          # Gün 10 — ABP çözüm yapısı ve katman sorumlulukları
├── Obsidian/         # Günlük notlar, DDD/veritabanı notları, haftalık değerlendirme
├── src/              # Gün 11'den itibaren — ABP Framework çözümü (Domain.Shared, Domain, Application, EntityFrameworkCore, Web, DbMigrator...)
└── test/             # Domain, Application ve EntityFrameworkCore test projeleri
```

## Hedef tasarım (özet)

11 uygulama tablosu: konum (`Country → Province → District`) + ana akış (`Workplace`, `InternProfile`, `Skill`, `DailyLog` (+ `DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry`), `MentorReview`).

Ana aggregate: **DailyLog** ve child'ları. Günlük durumları: Draft → Submitted → (Approved | RevisionRequested → Draft).

## Çözümü çalıştırma

* Gerekli: .NET 10 SDK, PostgreSQL, Node.js (istemci kütüphaneleri için).
* `src/InternshipJournal.Web` ve `src/InternshipJournal.DbMigrator` altındaki `appsettings.json` bağlantı dizesini kontrol edin.
* `InternshipJournal.DbMigrator` projesini çalıştırarak migration'ları uygulayın ve referans verileri (ülke/il/ilçe/yetkinlik) seed edin.
* Ardından `InternshipJournal.Web` projesini başlatın.

## Haftalık ilerleme

- **Gün 6:** Gereksinim analizi ve ortak dil — `docs/analysis/`
- **Gün 7:** DDD ve domain modelleme — `docs/domain/`
- **Gün 8:** Normalizasyon ve veritabanı tasarımı — `docs/database/`
- **Gün 9:** Tasarım sunumu ve kontrol kapısı — `docs/decisions/`
- **Gün 10:** ABP Framework temelleri — `docs/abp/`
- **Gün 11:** Domain.Shared, konum referansları (Country/Province/District), Skill, seed, LocationAppService — `src/`
- **Gün 12:** Workplace aggregate, WorkplaceManager, nested adres seçimi (ülke → il → ilçe), Razor Pages — `src/`
- **Gün 13:** InternProfile aggregate, DateRange Value Object, InternProfileManager, staj profili ekranları — `src/`
- **Gün 14:** DailyLog aggregate, child entity'ler (DailyLogItem, DailyLogSkill, ProblemSolvingEntry), DailyLogManager, domain testleri — `src/`
- **Gün 15:** DailyLog için EF Core mapping, migration (`Added_DailyLog_Module`), DailyLogRepository, Application Contracts ve başlangıç DailyLogAppService — `src/`
- **Gün 16:** DailyLogAppService'in tamamlanması — madde/yetkinlik/problem yönetimi ve durum geçişi (Submit/RequestRevision/Approve/ReturnToDraft) uçtan uca — `src/`
