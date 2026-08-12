# Kullanım Senaryoları

Bu doküman, uygulamanın temel kullanım senaryolarını (use case) tanımlar. Her senaryo; amacı, aktörü, ana akışı, alternatif/hata akışlarını ve kabul kriterlerini içerir. Senaryolar "çözüm" değil, "kullanıcı ne yapmak istiyor" bakışıyla yazılmıştır.

İş kuralı referansları `business-rules.md` içindeki numaralara (BR-x) karşılık gelir.

---

## UC-1: Çalışma Yeri Oluşturma

### Amaç
Admin'in, stajyerin bağlanacağı çalışma yerini adresiyle birlikte tanımlaması.

### Aktör
Admin

### Ön koşullar
- Admin giriş yapmış.
- Adres için gerekli ülke, il ve ilçe verileri aktif olarak mevcut.

### Ana akış
1. Admin "Yeni Çalışma Yeri" ekranını açar.
2. Çalışma yeri adını ve iletişim bilgilerini girer.
3. Ülke seçer; ilgili iller yüklenir.
4. İl seçer; ilgili ilçeler yüklenir.
5. İlçe seçer, açık adresi girer.
6. Kaydeder.

### Alternatif akışlar
- 3a. Ülke değiştirilirse il ve ilçe seçimi sıfırlanır.
- 4a. İl değiştirilirse ilçe seçimi sıfırlanır.

### Hata durumları
- Ad boş → kayıt reddedilir (BR-12).
- İlçe seçilmeden kayıt denenirse → reddedilir (BR-13).
- Pasif ilçe seçilmeye çalışılırsa → reddedilir (BR-6).
- E-posta geçersiz formatta → reddedilir (BR-14).

### Son koşullar
Çalışma yeri aktif olarak sisteme eklenir ve stajyer profiline bağlanabilir hale gelir.

### İlgili iş kuralları
BR-6, BR-12, BR-13, BR-14

### Kabul kriterleri
- [ ] Ülke–il–ilçe bağımlı olarak dolar.
- [ ] İlçe seçilmeden kayıt yapılamaz.
- [ ] Geçerli kayıt sonrası çalışma yeri listede görünür.

---

## UC-2: Stajyer Profili Oluşturma

### Amaç
Admin'in bir kullanıcıyı stajyer olarak tanımlaması; çalışma yerine ve mentora bağlaması.

### Aktör
Admin

### Ön koşullar
- Hedef kullanıcı ve mentor kullanıcı sistemde mevcut.
- Aktif bir çalışma yeri mevcut.

### Ana akış
1. Admin "Yeni Stajyer Profili" ekranını açar.
2. Kullanıcıyı seçer.
3. Mentoru seçer.
4. Çalışma yerini seçer.
5. Eğitim bilgilerini ve staj dönemini (başlangıç–bitiş) girer.
6. Kaydeder.

### Alternatif akışlar
- 5a. Gerekli çalışma günü sayısı girilebilir.

### Hata durumları
- Başlangıç tarihi bitişten sonra → reddedilir (BR-7).
- Kullanıcının zaten aktif bir profili varsa → reddedilir (BR-8).
- Pasif çalışma yeri seçilirse → reddedilir (BR-9).
- Mentor boş → reddedilir (BR-10).

### Son koşullar
Stajyer artık günlük oluşturabilir.

### İlgili iş kuralları
BR-7, BR-8, BR-9, BR-10

### Kabul kriterleri
- [ ] Bir kullanıcı için yalnızca bir aktif profil olur.
- [ ] Profil bir çalışma yerine ve bir mentora bağlıdır.

---

## UC-3: Günlük Oluşturma

### Amaç
Stajyerin belirli bir gün için günlük kaydı başlatması.

### Aktör
Stajyer

### Ön koşullar
- Stajyerin aktif profili var.
- O gün için henüz günlük oluşturulmamış.

### Ana akış
1. Stajyer "Yeni Günlük" ekranını açar.
2. Günlük tarihini seçer (varsayılan: bugün).
3. Kısa bir özet girer (opsiyonel).
4. Kaydeder; günlük "Draft" (taslak) durumunda oluşur.

### Alternatif akışlar
- 2a. Geçmiş bir staj günü seçilebilir (dönem içindeyse).

### Hata durumları
- Aynı tarih için günlük varsa → reddedilir (BR-1).
- Gelecek tarih seçilirse → reddedilir (BR-2).
- Tarih staj dönemi dışındaysa → reddedilir (BR-3).

### Son koşullar
Taslak bir günlük oluşur; içine madde/yetkinlik/problem eklenebilir.

### İlgili iş kuralları
BR-1, BR-2, BR-3

### Kabul kriterleri
- [ ] Aynı güne ikinci günlük engellenir.
- [ ] Gelecek/dönem dışı tarih engellenir.

---

## UC-4: Günlüğe Çalışma Maddesi Ekleme

### Amaç
Stajyerin günlüğe yaptığı bir işi (başlık, tür, süre) eklemesi.

### Aktör
Stajyer

### Ön koşullar
- Düzenlenebilir (Draft veya RevisionRequested) bir günlük var.

### Ana akış
1. Stajyer günlüğün detayında "Madde Ekle" der.
2. Başlık, açıklama, çalışma türü ve süre (dakika) girer.
3. Kaydeder.
4. Günlüğün toplam süresi maddelerin toplamından yeniden hesaplanır.

### Alternatif akışlar
- 4a. Madde silinir/güncellenirse toplam süre tekrar hesaplanır.

### Hata durumları
- Başlık boş → reddedilir (BR-15).
- Süre 0 veya negatif → reddedilir (BR-16).
- Onaylanmış günlüğe madde eklenmeye çalışılırsa → reddedilir (BR-4).

### Son koşullar
Madde günlüğe eklenir; toplam süre güncellenir.

### İlgili iş kuralları
BR-4, BR-11, BR-15, BR-16

### Kabul kriterleri
- [ ] Toplam süre kullanıcıdan alınmaz, otomatik hesaplanır (BR-11).
- [ ] Onaylı günlüğe madde eklenemez.

---

## UC-5: Günlüğe Yetkinlik Ekleme

### Amaç
Stajyerin o gün çalıştığı bir yetkinliği ve öğrenme seviyesini kaydetmesi.

### Aktör
Stajyer

### Ön koşullar
- Düzenlenebilir bir günlük var.
- Yetkinlik listesi aktif kayıtlar içeriyor.

### Ana akış
1. Stajyer "Yetkinlik Ekle" der.
2. Bir yetkinlik seçer, öğrenme seviyesini (Introduced/Practiced/Applied/Improved) belirler, not ekler.
3. Kaydeder.

### Hata durumları
- Aynı yetkinlik günlüğe ikinci kez eklenmeye çalışılırsa → reddedilir (BR-5).
- Pasif yetkinlik seçilirse → reddedilir (BR-17).

### Son koşullar
Yetkinlik günlüğe eklenir.

### İlgili iş kuralları
BR-5, BR-17

### Kabul kriterleri
- [ ] Aynı yetkinlik bir günlükte tekrarlanamaz.

---

## UC-6: Problem Çözme Kaydı Ekleme

### Amaç
Stajyerin karşılaştığı bir problemi, çözüm sürecini ve varsa yapay zekâ kullanımını kaydetmesi.

### Aktör
Stajyer

### Ön koşullar
- Düzenlenebilir bir günlük var.

### Ana akış
1. Stajyer "Problem Ekle" der.
2. Başlık, problem açıklaması, hata mesajı, denenen çözümler, kök neden ve nihai çözümü girer.
3. Yapay zekâ kullandıysa: araç adı, istem özeti, öneri, kabul/ret ve gerekçesini girer.
4. Kaydeder.

### Hata durumları
- Problem açıklaması boş → reddedilir (BR-18).
- Yapay zekâ kullanıldı işaretli ama araç/gerekçe boş → reddedilir (BR-19).
- Gerçek şifre/anahtar/şirket sırrı girilmesi → yasak (BR-20).

### Son koşullar
Problem kaydı günlüğe eklenir.

### İlgili iş kuralları
BR-18, BR-19, BR-20

### Kabul kriterleri
- [ ] AI kullanıldıysa doğrulama/gerekçe bilgisi zorunludur.

---

## UC-7: Günlüğü Mentor İncelemesine Gönderme

### Amaç
Stajyerin tamamladığı günlüğü mentora göndermesi.

### Aktör
Stajyer

### Ön koşullar
- Günlük Draft veya RevisionRequested durumunda.
- Günlükte en az bir çalışma maddesi var.

### Ana akış
1. Stajyer günlük detayında "Gönder" der.
2. Sistem kuralları kontrol eder.
3. Günlük "Submitted" durumuna geçer, gönderim zamanı kaydedilir.

### Hata durumları
- Boş günlük (madde yok) gönderilmeye çalışılırsa → reddedilir (BR-21).

### Son koşullar
Günlük mentorun inceleme listesine düşer; artık stajyer tarafından düzenlenemez (BR-22).

### İlgili iş kuralları
BR-21, BR-22

### Kabul kriterleri
- [ ] Boş günlük gönderilemez.
- [ ] Gönderilen günlük düzenlenemez hale gelir.

---

## UC-8: Günlüğü Onaylama

### Amaç
Mentorun gönderilmiş bir günlüğü onaylaması.

### Aktör
Mentor

### Ön koşullar
- Günlük "Submitted" durumunda.
- Günlük mentorun kendi stajyerine ait.

### Ana akış
1. Mentor bekleyen günlükler listesinden birini açar.
2. İçeriği inceler, isteğe bağlı yorum yazar.
3. "Onayla" der.
4. Bir MentorReview (karar: Approved) kaydı oluşur; günlük "Approved" durumuna geçer.

### Hata durumları
- Günlük Submitted değilse → işlem reddedilir (BR-23).
- Mentor bu stajyere atanmamışsa → reddedilir (BR-24).

### Son koşullar
Günlük onaylanır ve artık değiştirilemez (BR-22).

### İlgili iş kuralları
BR-22, BR-23, BR-24, BR-25

### Kabul kriterleri
- [ ] Onay, günlük durumunu da günceller.
- [ ] Mentor içeriği değiştiremez (BR-25).

---

## UC-9: Günlük İçin Düzeltme İsteme

### Amaç
Mentorun günlüğü düzeltme talebiyle stajyere geri göndermesi.

### Aktör
Mentor

### Ön koşullar
- Günlük "Submitted" durumunda ve mentorun stajyerine ait.

### Ana akış
1. Mentor günlüğü açar.
2. "Düzeltme İste" der ve **zorunlu** bir yorum yazar.
3. MentorReview (karar: RevisionRequested) oluşur; günlük "RevisionRequested" durumuna geçer.

### Hata durumları
- Yorum boş → reddedilir (BR-26).

### Son koşullar
Günlük stajyere geri döner ve tekrar düzenlenebilir hale gelir.

### İlgili iş kuralları
BR-22, BR-26

### Kabul kriterleri
- [ ] Düzeltme talebinde yorum zorunludur.

---

## UC-10: Düzeltilen Günlüğü Yeniden Gönderme

### Amaç
Stajyerin düzeltme istenen günlüğü güncelleyip tekrar göndermesi.

### Aktör
Stajyer

### Ön koşullar
- Günlük "RevisionRequested" durumunda.

### Ana akış
1. Stajyer günlüğü açar (durum düzenlemeye izin verir).
2. Gerekli değişiklikleri yapar.
3. Tekrar "Gönder" der; günlük "Submitted" durumuna döner.

### Son koşullar
Günlük yeniden mentor incelemesine düşer.

### İlgili iş kuralları
BR-22, BR-21

### Kabul kriterleri
- [ ] RevisionRequested günlük düzenlenip yeniden gönderilebilir.

---

## UC-11: Haftalık Rapor Görüntüleme

### Amaç
Stajyerin (veya mentorun) bir haftanın gelişim özetini görmesi.

### Aktör
Stajyer / Mentor

### Ön koşullar
- İlgili hafta için günlük kayıtları mevcut.

### Ana akış
1. Kullanıcı "Haftalık Rapor" ekranını açar.
2. Hafta aralığını seçer.
3. Sistem, günlük tablolarından raporu hesaplar (ayrı tablo yok).
4. Rapor gösterilir: günlük sayısı, toplam süre, tür dağılımı, yetkinlikler, problem/AI sayıları, onay durumları.

### Son koşullar
Rapor görüntülenir (kayıt oluşmaz, hesaplanır).

### İlgili iş kuralları
BR-11

### Kabul kriterleri
- [ ] Rapor mevcut günlük verilerinden türetilir, ayrı tablo tutulmaz.

---

## Kapsanan senaryo sayısı

Toplam 11 kullanım senaryosu tanımlandı (kabul kriteri: en az 8). Bunlar konum/çalışma yeri kurulumundan başlayıp günlük yaşam döngüsü (oluştur → doldur → gönder → incele → onay/düzeltme → rapor) boyunca ilerler.
