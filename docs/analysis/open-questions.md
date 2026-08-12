# Belirsizlikler ve Mentora Sorular

Analiz sırasında net olmayan noktaları soru hâline getirdim. Her sorunun yanında **benim önerim/varsayımım** var; mentor onaylarsa varsayımı kural olarak işlerim.

---

1. **Aynı gün için kaç günlük oluşturulabilir?**
   Önerim: Tek (BR-1). Onaylıyor musunuz?

2. **Gelecek tarihli günlük oluşturulabilir mi?**
   Önerim: Hayır (BR-2).

3. **Staj dönemi dışındaki bir tarih kullanılabilir mi?**
   Önerim: Hayır (BR-3).

4. **Günlük hangi durumda düzenlenebilir?**
   Önerim: Yalnızca Draft ve RevisionRequested (BR-22).

5. **Mentor günlük içeriğini değiştirebilir mi?**
   Önerim: Hayır; yalnızca karar + yorum (BR-25).

6. **Düzeltme istenen günlük hangi duruma geçer?**
   Önerim: RevisionRequested; düzenlenip yeniden gönderilince Submitted.

7. **Toplam süre kullanıcıdan mı alınmalı, maddelerden mi hesaplanmalı?**
   Önerim: Maddelerden hesaplanır (BR-11). (Bu, hem bir invariant hem de UI kararı.)

8. **Aynı yetkinlik aynı günlüğe birden fazla eklenebilir mi?**
   Önerim: Hayır (BR-5).

9. **Yapay zekâ kullanıldıysa hangi bilgiler zorunlu?**
   Önerim: Araç adı, istem özeti, öneri, kabul/ret ve gerekçe (BR-19).

10. **Bir kullanıcının birden fazla aktif staj profili olabilir mi?**
    Önerim: Hayır, tek aktif profil (BR-8).

11. **Çalışma yeri adresi hangi bilgilerden oluşur ve nasıl saklanır?**
    Önerim: Yalnızca DistrictId + AddressLine (+ posta kodu); il/ülke ilişkiden türetilir (BR-29).

12. **Ülke, il ve ilçe ilişkisi nasıl doğrulanır?**
    Önerim: İlçe seçilir, il/ülke ondan türetilir; böylece çelişki oluşamaz.

13. **Günlük maddesi için "günlük maksimum süresi" nedir?** (BR-16 üst sınırı)
    Önerim: Tek madde için makul bir üst sınır (örn. 1440 dk / 24 saat) veya günlük toplamı için sınır. Mentorun tercih ettiği değer nedir?

14. **Mentor–stajyer ilişkisi bire bir mi, bir mentora çok stajyer mi?**
    Önerim: Bir mentora çok stajyer; bir stajyerin tek mentoru. Doğru mu?

15. **Haftalık rapor için ayrı tablo tutulmalı mı?**
    Önerim: Hayır; günlük tablolarından hesaplanır (dokümanla uyumlu).

---

Bu sorular Gün 9 (tasarım sunumu) öncesinde mentorla netleştirilecek. Şimdilik "önerim" olarak işaretlenen varsayımlarla ilerliyorum.
