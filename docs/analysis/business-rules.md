# İş Kuralları

Bu doküman, uygulamanın her ekran ve kullanım noktasında **aynı şekilde** koruması gereken davranışları listeler. İş kuralı, sadece bir ekranın değil, sistemin genelinin uyması gereken kısıttır; bu yüzden UI'da değil, domain/servis katmanında da korunmalıdır.

Her kural için: gerekçe, ilgili senaryolar, ihlal sonucu ve veri tutarlılığına etkisi belirtilmiştir. Toplam **27 kural** (kabul kriteri: en az 15).

---

## Günlük yaşam döngüsü kuralları

### BR-1: Aynı stajyer aynı gün için yalnızca bir günlük oluşturabilir.
- **Gerekçe:** Bir günün çalışması tek bir kayıtta toplanmalı; mükerrer günlük raporu bozar.
- **İlgili senaryolar:** UC-3
- **İhlal sonucu:** İkinci günlük oluşturma reddedilir, kullanıcı uyarılır.
- **Veri etkisi:** (InternProfileId, LogDate) çifti benzersiz olmalı.

### BR-2: Gelecek tarihli günlük oluşturulamaz.
- **Gerekçe:** Henüz yapılmamış bir çalışma kaydedilemez.
- **İlgili senaryolar:** UC-3
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** LogDate ≤ bugün.

### BR-3: Günlük tarihi staj dönemi (başlangıç–bitiş) içinde olmalıdır.
- **Gerekçe:** Staj dışı bir güne çalışma kaydı mantıksızdır.
- **İlgili senaryolar:** UC-3
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** LogDate, profilin dönem aralığında olmalı.

### BR-4: Onaylanmış günlüğe yeni çalışma maddesi eklenemez.
- **Gerekçe:** Onaydan sonra içerik dondurulmalı.
- **İlgili senaryolar:** UC-4
- **İhlal sonucu:** Ekleme reddedilir.
- **Veri etkisi:** Yalnızca Draft/RevisionRequested durumda child ekleme.

### BR-11: Günlüğün toplam süresi, çalışma maddelerinin sürelerinin toplamından hesaplanır.
- **Gerekçe:** Kullanıcıdan alınan toplam, maddelerle çelişebilir; tek doğru kaynak maddelerdir.
- **İlgili senaryolar:** UC-4, UC-11
- **İhlal sonucu:** Kullanıcı toplam süreyi elle giremez.
- **Veri etkisi:** TotalMinutes türetilmiş değerdir; DailyLog içinde hesaplanır.

### BR-21: Boş günlük (hiç çalışma maddesi olmayan) incelemeye gönderilemez.
- **Gerekçe:** İçeriksiz günlüğün incelenmesi anlamsızdır.
- **İlgili senaryolar:** UC-7, UC-10
- **İhlal sonucu:** Gönderim reddedilir.
- **Veri etkisi:** En az bir DailyLogItem şartı.

### BR-22: Onaylanan günlük değiştirilemez; yalnızca Draft veya RevisionRequested durumundaki günlük düzenlenebilir.
- **Gerekçe:** Onaylı kaydın bütünlüğü korunmalı; süreç izlenebilir olmalı.
- **İlgili senaryolar:** UC-7, UC-8, UC-9, UC-10
- **İhlal sonucu:** Düzenleme/gönderme reddedilir.
- **Veri etkisi:** Durum (Status) tüm değişiklik işlemlerinin ön koşuludur.

### BR-27: Stajyer yalnızca kendi günlüğünü görüntüleyebilir ve düzenleyebilir.
- **Gerekçe:** Veri gizliliği ve sahipliği.
- **İlgili senaryolar:** UC-3..UC-7, UC-10, UC-11
- **İhlal sonucu:** Başkasının günlüğüne erişim engellenir.
- **Veri etkisi:** Sorgular InternProfileId ile filtrelenir; kontrol serviste yapılır.

---

## Yetkinlik ve madde kuralları

### BR-5: Aynı yetkinlik bir günlüğe iki kez eklenemez.
- **Gerekçe:** Tekrarlı yetkinlik raporu şişirir, anlamsızdır.
- **İlgili senaryolar:** UC-5
- **İhlal sonucu:** İkinci ekleme reddedilir.
- **Veri etkisi:** (DailyLogId, SkillId) benzersiz olmalı.

### BR-15: Çalışma maddesi başlığı boş olamaz.
- **Gerekçe:** Ne yapıldığı anlaşılmalı.
- **İlgili senaryolar:** UC-4
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** Title zorunlu.

### BR-16: Çalışma maddesi süresi sıfırdan büyük olmalı ve günlük maksimumunu aşmamalıdır.
- **Gerekçe:** Sıfır/aşırı süre gerçekçi değildir.
- **İlgili senaryolar:** UC-4
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** 0 < DurationMinutes ≤ makul üst sınır.

### BR-17: Pasif yetkinlik günlüğe eklenemez.
- **Gerekçe:** Kullanımdan kaldırılmış yetkinlik yeni kayıtta seçilmemeli.
- **İlgili senaryolar:** UC-5
- **İhlal sonucu:** Seçim reddedilir.
- **Veri etkisi:** Skill.IsActive = true şartı.

---

## Problem ve yapay zekâ kuralları

### BR-18: Problem açıklaması boş olamaz.
- **Gerekçe:** Problem kaydının bir içeriği olmalı.
- **İlgili senaryolar:** UC-6
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** ProblemDescription zorunlu.

### BR-19: Yapay zekâ kullanıldıysa araç/yöntem ve kabul-ret gerekçesi belirtilmelidir.
- **Gerekçe:** AI kullanımının sorumlu ve izlenebilir olması.
- **İlgili senaryolar:** UC-6
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** UsedArtificialIntelligence = true ise AiToolName ve gerekçe zorunlu.

### BR-20: Gerçek şifre, bağlantı anahtarı veya şirket sırrı kaydedilemez.
- **Gerekçe:** Güvenlik ve gizlilik.
- **İlgili senaryolar:** UC-6
- **İhlal sonucu:** Bu tür içerik girilmemelidir (uyarı/politika).
- **Veri etkisi:** Hassas veri saklanmaz.

---

## Çalışma yeri ve adres kuralları

### BR-12: Çalışma yeri adı boş olamaz.
- **Gerekçe:** Çalışma yeri tanımlanabilir olmalı.
- **İlgili senaryolar:** UC-1
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** Name zorunlu.

### BR-13: İlçe seçilmeden ve adres satırı boş bırakılarak adres kaydedilemez.
- **Gerekçe:** Adres eksik olamaz; ilçe adresin çapasıdır.
- **İlgili senaryolar:** UC-1
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** DistrictId ve AddressLine zorunlu.

### BR-14: E-posta girilmişse geçerli formatta olmalıdır.
- **Gerekçe:** İletişim bilgisinin kullanılabilir olması.
- **İlgili senaryolar:** UC-1
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** Email opsiyonel ama girilirse format kontrolü.

---

## Konum referans kuralları

### BR-6: Pasif ilçe/il/ülke seçilemez ve kullanılan konum fiziksel olarak silinmez.
- **Gerekçe:** Geçmiş kayıtların adresi bozulmamalı; referans bütünlüğü korunmalı.
- **İlgili senaryolar:** UC-1
- **İhlal sonucu:** Pasif konum seçimi reddedilir; silme yerine pasifleştirme.
- **Veri etkisi:** IsActive bayrağı; silme için restrict davranışı.

### BR-28: Ülke kodu benzersizdir; aynı ülke içinde il adı, aynı il içinde ilçe adı benzersizdir.
- **Gerekçe:** Konum verisi tutarlı ve çakışmasız olmalı.
- **İlgili senaryolar:** UC-1 (dolaylı)
- **İhlal sonucu:** Mükerrer kayıt reddedilir.
- **Veri etkisi:** Country.Code unique; (CountryId, Name) ve (ProvinceId, Name) unique.

### BR-29: Adres hiyerarşisi tutarlı olmalıdır (ilçe, bağlı olduğu ile ve ülkeye göre çelişemez).
- **Gerekçe:** "Ankara / Kadıköy" gibi hatalı kombinasyonlar oluşamamalı.
- **İlgili senaryolar:** UC-1
- **İhlal sonucu:** Yalnızca DistrictId saklanarak çelişki baştan engellenir.
- **Veri etkisi:** Workplace yalnızca DistrictId tutar; il/ülke ilişkiden türetilir.

---

## Stajyer profili ve mentor kuralları

### BR-7: Staj başlangıç tarihi bitiş tarihinden sonra olamaz.
- **Gerekçe:** Mantıksal tarih tutarlılığı.
- **İlgili senaryolar:** UC-2
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** InternshipStartDate ≤ InternshipEndDate (DateRange value object).

### BR-8: Bir kullanıcı için yalnızca bir aktif staj profili bulunabilir.
- **Gerekçe:** Aynı anda birden fazla aktif staj karışıklık yaratır.
- **İlgili senaryolar:** UC-2
- **İhlal sonucu:** İkinci aktif profil reddedilir.
- **Veri etkisi:** UserId için aktif profil tekilliği.

### BR-9: Stajyer aktif bir çalışma yerine bağlı olmalıdır; pasif çalışma yerine yeni stajyer atanamaz.
- **Gerekçe:** Kapalı bir iş yerine staj bağlanamaz.
- **İlgili senaryolar:** UC-2
- **İhlal sonucu:** Atama reddedilir.
- **Veri etkisi:** Workplace.IsActive = true şartı.

### BR-10: Mentor kullanıcı kimliği boş olamaz.
- **Gerekçe:** Her stajyerin bir sorumlu mentoru olmalı.
- **İlgili senaryolar:** UC-2
- **İhlal sonucu:** Kayıt reddedilir.
- **Veri etkisi:** MentorUserId zorunlu.

---

## Mentor incelemesi kuralları

### BR-23: Yalnızca "Submitted" durumundaki günlük incelenebilir.
- **Gerekçe:** Taslak/onaylı günlüğün incelenmesi süreci bozar.
- **İlgili senaryolar:** UC-8, UC-9
- **İhlal sonucu:** İnceleme reddedilir.
- **Veri etkisi:** İşlem ön koşulu: Status = Submitted.

### BR-24: Mentor yalnızca kendisine atanmış stajyerin günlüğünü inceleyebilir.
- **Gerekçe:** Yetki ve gizlilik.
- **İlgili senaryolar:** UC-8, UC-9
- **İhlal sonucu:** Erişim/işlem reddedilir.
- **Veri etkisi:** Mentor–stajyer eşleşmesi kontrolü.

### BR-25: Mentor günlük içeriğini doğrudan değiştiremez; yalnızca karar ve yorum ekler.
- **Gerekçe:** İçeriğin sahipliği stajyerdedir; inceleme tarafsız olmalı.
- **İlgili senaryolar:** UC-8, UC-9
- **İhlal sonucu:** Mentorun içerik değişikliği engellenir.
- **Veri etkisi:** MentorReview yalnızca Decision + Comment tutar.

### BR-26: Düzeltme talebinde yorum zorunludur.
- **Gerekçe:** Stajyer neyi düzelteceğini bilmeli.
- **İlgili senaryolar:** UC-9
- **İhlal sonucu:** Yorumsuz düzeltme talebi reddedilir.
- **Veri etkisi:** RevisionRequested kararında Comment zorunlu.
