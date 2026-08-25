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
│   ├── abp/          # Gün 10 — ABP çözüm yapısı ve katman sorumlulukları
│   ├── test-strategy.md        # Gün 20 — test katmanları ve yaklaşımı
│   ├── security-checklist.md   # Gün 20 — yetkilendirme/sahiplik/gizli bilgi kontrol listesi
│   ├── ai-usage-report.md      # Gün 20 — proje boyunca yapay zekâ kullanımı özeti
│   └── final-demo-script.md    # Gün 20 — final demo senaryosu
├── Obsidian/         # Günlük notlar, DDD/veritabanı notları, haftalık değerlendirme
├── src/              # Gün 11'den itibaren — ABP Framework çözümü (Domain.Shared, Domain, Application, EntityFrameworkCore, Web, DbMigrator...)
├── test/             # Domain, Application, EntityFrameworkCore ve Web test projeleri
└── docker-compose.yml  # PostgreSQL (Gün 20)
```

## Hedef tasarım (özet)

11 uygulama tablosu: konum (`Country → Province → District`) + ana akış (`Workplace`, `InternProfile`, `Skill`, `DailyLog` (+ `DailyLogItem`, `DailyLogSkill`, `ProblemSolvingEntry`), `MentorReview`).

Ana aggregate: **DailyLog** ve child'ları. Günlük durumları: Draft → Submitted → (Approved | RevisionRequested → Draft).

Roller: **Stajyer** (kendi günlüğünü oluşturur/düzenler/gönderir), **Mentor** (kendisine bağlı stajyerlerin günlüklerini onaylar/düzeltme ister), **Admin** (kullanıcı/rol, konum, yetkinlik, çalışma yeri ve staj profili yönetimi).

## Çözümü çalıştırma

* Gerekli: .NET 10 SDK, Docker (PostgreSQL için) veya yerel bir PostgreSQL kurulumu, Node.js (istemci kütüphaneleri için).
* PostgreSQL'i başlatın: `docker compose up -d` (kök dizindeki `docker-compose.yml`, `appsettings.json`'daki varsayılan bağlantı dizesiyle birebir uyumlu — `root`/`myPassword`/`InternshipJournal`). Kendi PostgreSQL'inizi kullanacaksanız `src/InternshipJournal.Web` ve `src/InternshipJournal.DbMigrator` altındaki `appsettings.json` bağlantı dizesini buna göre güncelleyin.
* `src/InternshipJournal.DbMigrator` projesini çalıştırarak migration'ları uygulayın ve referans verileri (ülke/il/ilçe/yetkinlik, Stajyer/Mentor rolleri, admin kullanıcısı) seed edin: `dotnet run` (proje dizininden).
* Ardından `src/InternshipJournal.Web` projesini `dotnet run` ile başlatın ve **https://localhost:44399** adresine gidin. Varsayılan admin girişi: `admin` / `1q2w3E*`.

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
- **Gün 17:** DailyLog için stajyer tarafı Razor Pages (liste/oluştur/detay), eksik Skill Application katmanı — `src/`
- **Gün 18:** MentorReview aggregate'i uçtan uca (Domain + EF Core + Application) — `src/`
- **Gün 19:** Sahiplik kontrolleri, permission yapısı, Stajyer/Mentor rolleri, mentor tarafı Razor Pages — `src/`
- **Gün 20:** Final test geçişi, docker-compose, test stratejisi/güvenlik kontrol listesi/AI kullanım raporu/final demo senaryosu, 4. Hafta değerlendirmesi, teslim — `docs/`, `Obsidian/`
