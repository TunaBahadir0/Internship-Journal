# Ekran Taslakları (Wireframes)

Bu doküman, ana ekranların **basit** taslaklarını içerir. Amaç görsel güzellik değil; hangi alanların, butonların ve duruma göre değişen kontrollerin olacağını göstermektir. Kod veya tasarım detayı içermez.

Notasyon: `[____]` giriş alanı, `[▼]` açılır liste, `[Buton]` düğme, `( )` durum/rozet.

---

## 1. Dashboard (Stajyer)

```text
+--------------------------------------------------------------+
|  Merhaba, <Stajyer>            Kalan staj günü: 18           |
+--------------------------------------------------------------+
|  [Bugünün günlüğü yok]  -> [Bugün İçin Günlük Oluştur]        |
|                                                              |
|  Bu hafta: 4 günlük | 780 dk | 2 onaylı | 1 düzeltme        |
|                                                              |
|  Bekleyen taslaklar (2)      Onay bekleyenler (1)            |
|  Düzeltme istenenler (1)                                     |
|                                                              |
|  En çok çalışılan yetkinlikler: Docker, PostgreSQL, C#      |
|  Son mentor yorumu: "Testleri artır."                        |
+--------------------------------------------------------------+
```

Duruma göre: Bugün günlük varsa "Oluştur" yerine "Bugünün Günlüğünü Aç" görünür.

---

## 2. Günlük Listesi

```text
+--------------------------------------------------------------+
|  Günlüklerim                         [Yeni Günlük]           |
+--------------------------------------------------------------+
|  Filtre: Durum [Tümü ▼]  Tarih [____]                        |
+--------------------------------------------------------------+
|  Tarih       Özet                 Süre   Durum      İşlem    |
|  10.08.2026  EF Core çalışması     195    (Taslak)   [Aç]    |
|  09.08.2026  Docker kurulumu       390    (Onaylı)   [Aç]    |
|  08.08.2026  Test yazımı           240    (Gönderildi)[Aç]   |
+--------------------------------------------------------------+
|  Kayıt yoksa: "Henüz günlük yok. Yeni günlük ekleyin."       |
+--------------------------------------------------------------+
```

Durum rozetleri: Taslak / Gönderildi / Düzeltme İstendi / Onaylı.

---

## 3. Günlük Oluşturma

```text
+--------------------------------------------------------------+
|  Yeni Günlük                                                 |
+--------------------------------------------------------------+
|  Tarih:  [10.08.2026]   (varsayılan bugün)                   |
|  Özet:   [___________________________________]  (opsiyonel)  |
|                                                              |
|                        [Kaydet]   [Vazgeç]                   |
+--------------------------------------------------------------+
|  Hatalar alan altında: "Bu tarih için günlük zaten var."     |
+--------------------------------------------------------------+
```

---

## 4. Günlük Detay

```text
+--------------------------------------------------------------+
|  Günlük - 10.08.2026            Durum: (Taslak)              |
|  Özet: EF Core çalışması            Toplam süre: 195 dk      |
+--------------------------------------------------------------+
|  Çalışma Maddeleri                     [Madde Ekle]          |
|   - Migration çalışması  Development  90 dk    [Düzenle][Sil]|
|   - Test yazımı          Testing      45 dk    [Düzenle][Sil]|
+--------------------------------------------------------------+
|  Yetkinlikler                          [Yetkinlik Ekle]      |
|   - EF Core   Applied   "Repository denendi"   [Sil]         |
+--------------------------------------------------------------+
|  Problemler                            [Problem Ekle]        |
|   - Postgres bağlantısı   (AI kullanıldı)      [Aç]          |
+--------------------------------------------------------------+
|  [Mentora Gönder]     (sadece Taslak/Düzeltme durumunda)     |
+--------------------------------------------------------------+
```

Duruma göre: Onaylı günlükte ekle/düzenle/sil butonları ve "Gönder" gizlenir; sadece görüntülenir.

---

## 5. Çalışma Yeri Oluşturma (nested adres)

```text
+--------------------------------------------------------------+
|  Yeni Çalışma Yeri                                           |
+--------------------------------------------------------------+
|  Ad:        [____________________________]                   |
|  Ülke:      [Türkiye     ▼]                                  |
|  İl:        [İstanbul     ▼]  (ülke seçilince dolar)         |
|  İlçe:      [Şişli        ▼]  (il seçilince dolar)           |
|  Adres:     [Esentepe Mah. ...            ]                  |
|  Posta:     [34394]   Telefon: [________]  E-posta:[______]  |
|                        [Kaydet]   [Vazgeç]                   |
+--------------------------------------------------------------+
|  Ülke değişince il+ilçe sıfırlanır; ilçe seçilmeden kayıt yok|
+--------------------------------------------------------------+
```

---

## 6. Mentor İnceleme

```text
+--------------------------------------------------------------+
|  İnceleme Bekleyen Günlükler                                 |
+--------------------------------------------------------------+
|  Stajyer     Tarih       Süre   [Aç]                         |
|  Ali Akın    10.08.2026  390    [İncele]                     |
+--------------------------------------------------------------+

  İncele ekranı:
+--------------------------------------------------------------+
|  Günlük - Ali Akın - 10.08.2026     Durum: (Gönderildi)      |
|  (maddeler, yetkinlikler, problemler - salt okunur)          |
|                                                              |
|  Yorum: [____________________________________]              |
|             [Onayla]        [Düzeltme İste]                  |
+--------------------------------------------------------------+
|  "Düzeltme İste" için yorum zorunlu.                         |
+--------------------------------------------------------------+
```

Mentor içeriği değiştiremez (alanlar salt okunur).

---

## 7. Haftalık Rapor

```text
+--------------------------------------------------------------+
|  Haftalık Rapor    Hafta: [04-10 Ağustos 2026 ▼]            |
+--------------------------------------------------------------+
|  Günlük sayısı: 5     Toplam süre: 1.850 dk                  |
|  Tür dağılımı:  Development 40% | Testing 25% | ...          |
|  Çalışılan yetkinlikler: Docker, PostgreSQL, C#, EF Core     |
|  Çözülen problem: 3   (AI kullanılan: 2)                     |
|  Onaylanan: 4   Düzeltme istenen: 1                          |
|  Mentor yorumları: ...                                       |
+--------------------------------------------------------------+
```

Rapor ayrı tabloya yazılmaz; günlük tablolarından hesaplanır.

---

## Ortak kurallar (tüm ekranlar)

- Validasyon hataları ilgili alanın altında gösterilir.
- Duruma göre butonlar gizlenir/gösterilir (örn. onaylı günlükte düzenleme yok).
- Stajyer yalnızca kendi verisini, mentor yalnızca kendi stajyerini görür.
