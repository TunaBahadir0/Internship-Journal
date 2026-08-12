# Aktör Analizi

Bu doküman, Staj Günlüğü ve Gelişim Takip Uygulaması'nın aktörlerini (kullanıcı rollerini) tanımlar. Amaç: kim, neyi, hangi koşulda yapabiliyor sorusunu netleştirmek. Bu aşamada henüz sınıf veya tablo tasarlamıyoruz.

---

## Stajyer

### Amacı

Staj süresince yaptığı çalışmaları günlük olarak kaydetmek, öğrendiği yetkinlikleri ve karşılaştığı problemleri belgelemek, günlüklerini mentor onayına sunmak ve kendi gelişimini takip etmek.

### Yapabildiği işlemler

- Kendi profilini görüntüler.
- Bağlı olduğu çalışma yerini görüntüler.
- Günlük çalışma kaydı oluşturur (her gün için en fazla bir tane).
- Günlüğe çalışma maddeleri ekler (yapılan iş, tür, süre).
- Günlüğe çalışılan yetkinlikleri ve öğrenme seviyesini ekler.
- Karşılaştığı teknik problemleri ve yapay zekâ kullanımını kaydeder.
- Günlüğü mentor incelemesine gönderir.
- Mentorun yorumlarını ve kararını görüntüler.
- Düzeltme istenen günlüğü düzenleyip yeniden gönderir.
- Haftalık gelişim raporunu görüntüler.

### Erişebildiği veriler

- Yalnızca **kendi** profili, çalışma yeri bilgisi, günlükleri, problemleri ve raporları.
- Ortak referans verileri (ülke, il, ilçe, yetkinlik listesi) — sadece okuma.
- Başka stajyerlerin verilerine erişemez.

### İşlemlere başlamadan önce gerekli koşullar

- Sisteme "Intern" rolüyle giriş yapmış olmalı.
- Aktif bir staj profili bulunmalı (bir çalışma yerine ve bir mentora bağlı).
- Günlük ekleyebilmek için staj dönemi (başlangıç–bitiş) tanımlı ve aktif olmalı.

---

## Mentor

### Amacı

Kendisine bağlı stajyerin günlüklerini inceleyerek geri bildirim vermek, onaylamak veya düzeltme istemek ve stajyerin gelişimini takip etmek.

### Yapabildiği işlemler

- Kendisine bağlı stajyerin gönderilmiş günlüklerini görüntüler.
- Günlüğü ve içeriğini (maddeler, yetkinlikler, problemler) inceler.
- Yorum ekler.
- Günlüğü onaylar.
- Düzeltme talep eder (zorunlu yorumla).
- Stajyerin haftalık gelişimini görüntüler.

### Erişebildiği veriler

- Yalnızca **kendisine atanmış** stajyerlerin günlükleri ve raporları.
- Günlük içeriğini görüntüler ama **doğrudan değiştiremez** (sadece karar + yorum).
- Başka mentorların stajyerlerine erişemez.

### İşlemlere başlamadan önce gerekli koşullar

- Sisteme "Mentor" rolüyle giriş yapmış olmalı.
- İncelenecek günlük "Submitted" (gönderilmiş) durumunda olmalı.
- Mentor–stajyer eşleşmesi tanımlı olmalı.

---

## Admin

### Amacı

Sistemin temel/referans verilerini ve kullanıcı–rol yapısını yönetmek; stajyer ve mentorların çalışabilmesi için gerekli zemini hazırlamak.

### Yapabildiği işlemler

- Kullanıcıları ve rolleri yönetir.
- Ülke, il ve ilçe verilerini yönetir.
- Yetkinlik listesini yönetir.
- Çalışma yeri ve stajyer profili tanımlar.
- Mentor ile stajyer eşleştirmesi yapar.

### Erişebildiği veriler

- Tüm referans verileri (konum, yetkinlik) ve yönetim tabloları.
- Kullanıcı ve rol bilgileri.
- Not: Admin işlemlerinin büyük kısmı başlangıçta **seed verileriyle** hazırlanabilir (elle ekran yapmadan).

### İşlemlere başlamadan önce gerekli koşullar

- Sisteme "Admin" rolüyle giriş yapmış olmalı.
- Bir çalışma yeri tanımlamak için ilgili ilçenin (dolayısıyla il ve ülkenin) sistemde aktif olması gerekir.

---

## Özet ilişki

```text
Admin  → zemini hazırlar (konum, yetkinlik, çalışma yeri, profil, eşleştirme)
Stajyer → günlük üretir ve mentora gönderir
Mentor → günlüğü inceler, onaylar veya düzeltme ister
```
