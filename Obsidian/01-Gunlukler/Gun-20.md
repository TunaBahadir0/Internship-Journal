# Gün 20 — Teslim

Tarih: 26 Ağustos 2026, Çarşamba

## Bugün tamamladığım işler

Bugün yeni bir özellik eklemedim — programın son günü, `00-Stajyer-Program-Rehberi.md`'nin "4. Hafta: ... test ve teslim" ve "12. Final teslimatları" listesine göre eksik kalan teslimatları tamamladım:

- **Final test doğrulaması**: Kullanıcı `dotnet run` sürecini durdurduktan sonra tam çözüm derlemesini (0 hata) ve üç test projesini (`Domain.Tests` 41, `EntityFrameworkCore.Tests` 56, `Web.Tests` 3 — toplam **100/100**) yeniden çalıştırıp doğruladım. Gün 19'da `Web.Tests`'i çalıştıramamıştım (kullanıcının canlı süreci bin çıktısını kilitliyordu); bugün bu boşluk kapandı.
- **`docker-compose.yml`** (kök dizin) — PostgreSQL servisi, `appsettings.json`'daki **committed** (kullanıcının yerel, commit edilmemiş değerleri değil) varsayılan bağlantı dizesiyle (`root`/`myPassword`/`InternshipJournal`) birebir uyumlu. Program Rehberi'nin "Final teslimatları" listesindeki "Docker Compose yapılandırması" maddesi daha önce hiç karşılanmamıştı.
- **`docs/test-strategy.md`** — üç katmanlı test yaklaşımının (saf domain / NSubstitute-mock'lu Manager / tam DI+SQLite AppService / host boot) neden bu şekilde ayrıldığının açıklaması, artı test host'ta permission kontrolünün bilerek kapalı olduğunun (Gün 19'da keşfedilen `IsDynamicPermissionStoreEnabled = false`) belgelenmesi.
- **`docs/security-checklist.md`** — Gün 19'un yetkilendirme/sahiplik çalışmasının bir kontrol listesi hâline getirilmesi, artı **dürüstçe işaretlenmiş bilinen sınırlamalar** (Workplace/InternProfile'da sahiplik kontrolü yok, menü izin bazlı gizlenmiyor, ayrı bir rol atama ekranı yok).
- **`docs/ai-usage-report.md`** — programın "önce kendi cümlenle yaz, sonra doğrula" kuralının bu oturumda (Gün 14-20) somut örneklerle nasıl uygulandığının özeti. Gün 6-13 için kendi hatırlamadığım ayrıntıyı uydurmak yerine ilgili günlüklere yönlendirdim.
- **`docs/final-demo-script.md`** — uçtan uca akış, hata/validasyon senaryoları, mentor akışı, DBeaver incelemesi, test çalıştırması, bir teknik karar ve bir problem+çözüm anlatımı örneğini içeren çalıştırılabilir bir demo senaryosu.
- **`Obsidian/10-Haftalik-Degerlendirmeler/Hafta-04.md`** — 4. Hafta için müfredatta özel bir şablon olmadığından, Program Rehberi'nin genel haftalık değerlendirme şablonu (bölüm 13) kullanıldı.
- **README.md** güncellendi: Gün 17-20 satırları, docker-compose ile kurulum adımı, roller özeti, yeni `docs/` dosyalarına referans.

## Öğrendiğim / pekiştirdiğim konu

**Teslim, kod yazmakla bitmiyor.** Kod tarafı (Gün 19 sonu itibarıyla) zaten çalışıyordu; ama "Final teslimatları" listesindeki Docker Compose, test stratejisi, güvenlik kontrol listesi, AI kullanım raporu, final demo senaryosu gibi maddeler hiçbiri kod değildi — hepsi, zaten yapılmış işin **başka birinin (bir değerlendiricinin, gelecekteki bir geliştiricinin) kolayca anlayıp çalıştırabileceği** hâle getirilmesiydi. Bu, "çalışıyor" ile "teslim edilebilir" arasındaki farkı somut olarak gösterdi.

## Alınan kararlar

1. `docker-compose.yml`'i kullanıcının yerel makinesinde kullandığı (`postgres`/`StajGunlugu2026!`) değerlerle DEĞİL, repodaki **committed** varsayılan değerlerle (`root`/`myPassword`) uyumlu yazdım — çünkü docker-compose.yml'in amacı, depoyu klonlayan HERHANGİ birinin `appsettings.json`'ı hiç değiştirmeden çalıştırabilmesi; kullanıcının kendi yerel tercihi ayrı bir konu.
2. AI kullanım raporunda Gün 6-13'ün ayrıntısını özetlemek yerine ilgili günlüklere yönlendirdim — bu günlerin AI kullanım detayları context sıkıştırması nedeniyle elimde güvenilir şekilde yok; var olmayan bir anıyı rapor için uydurmak, raporun kendisinin güvenilirliğini baltalardı.
3. `fix/workplace-country-dropdown-bundling` PR'ının hâlâ merge edilmediğini fark ettim (bkz. "Bilinen açık iş" bölümü) — bunu sessizce görmezden gelmek yerine hem bu günlükte hem final rapor kanalında (kullanıcıya mesaj) açıkça belirttim.

## Kabul kriteri kontrolü (bkz. `docs/final-demo-script.md` "Demo öncesi kontrol")

- [x] Tam çözüm 0 hata ile derleniyor
- [x] 100/100 test geçiyor (Domain 41, EntityFrameworkCore 56, Web 3)
- [x] docker-compose.yml ile PostgreSQL tek komutla ayağa kalkıyor
- [x] Final teslimatlar listesindeki tüm dokümanlar mevcut (test stratejisi, güvenlik kontrol listesi, AI kullanım raporu, final demo senaryosu)
- [x] README ve Obsidian güncel, 4. Hafta değerlendirmesi yazıldı

## Bilinen açık iş (teslimden önce kullanıcının karar vermesi gereken)

**`fix/workplace-country-dropdown-bundling` PR'ı hâlâ `main`'e merge edilmedi.** Bu, yöneticinin bildirdiği ve doğrulanmış, çalışan bir düzeltme (Gün 19) — ama teslimden önce merge edilmezse `main`'deki Workplace Create/Edit ekranında ülke dropdown'ı hâlâ boş görünecektir. Bu programın kapsamı dışında (benim commit/push/merge yapabileceğim ama "merge et" kararı kullanıcıya ait) olduğu için burada açıkça not ediyorum, sessizce varsaymıyorum.

## Program tamamlandı

20 günlük program bu günle bitiyor. Dört haftalık yol haritası (Teknik Çalışma Kartları → Analiz/DDD/ABP → Domain ve veri katmanları → Razor Pages/mentor akışı/test/teslim) uygulandı; final teslimatlar `docs/` ve `Obsidian/` altında toplandı.
