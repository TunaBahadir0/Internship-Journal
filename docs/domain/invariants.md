# Invariant'lar

**Invariant**, bir aggregate'in her işlemden **önce ve sonra** doğru kalması gereken kuralıdır. Yani aggregate hangi metodu çağırırsan çağır, bu kurallar asla bozulmaz. Invariant'lar UI'da değil, aggregate'in kendi içinde korunur.

Aşağıda invariant'lar, onları koruyan aggregate ve ilgili iş kuralı (BR) ile listelenmiştir.

---

## DailyLog aggregate'inin invariant'ları

| # | Invariant | İlgili kural |
|---|---|---|
| I-1 | Toplam süre her zaman çalışma maddelerinin sürelerinin toplamına eşittir. | BR-11 |
| I-2 | Aynı yetkinlik bir günlükte en fazla bir kez bulunur. | BR-5 |
| I-3 | Yalnızca Draft veya RevisionRequested durumundaki günlük değiştirilebilir; Approved değişmez. | BR-4, BR-22 |
| I-4 | İncelemeye gönderilen günlükte en az bir çalışma maddesi vardır. | BR-21 |
| I-5 | Durum geçişleri yalnızca geçerli yönde olur: Draft→Submitted, Submitted→Approved/RevisionRequested, RevisionRequested→Draft. | BR-22 |
| I-6 | Her çalışma maddesinin başlığı dolu ve süresi 0'dan büyüktür. | BR-15, BR-16 |

**Nasıl korunur:** Bu kurallar DailyLog metotlarının (AddItem, RemoveItem, AddSkill, Submit, Approve...) içinde uygulanır. Örneğin `AddItem` çağrıldığında hem başlık/süre kontrol edilir hem de toplam süre yeniden hesaplanır (I-1). Child'lar doğrudan değil, root üzerinden değiştiği için bu garanti korunur.

---

## InternProfile aggregate'inin invariant'ları

| # | Invariant | İlgili kural |
|---|---|---|
| I-7 | Staj dönemi geçerli bir aralıktır (başlangıç ≤ bitiş). | BR-7 |
| I-8 | Profil bir çalışma yerine ve bir mentora bağlıdır (boş olamaz). | BR-9, BR-10 |

**Nasıl korunur:** `DateRange` value object başlangıç > bitiş ise oluşturulamaz (I-7). Mentor/çalışma yeri değişimi metotları boş Id kabul etmez.

> Not: "Bir kullanıcı için tek aktif profil" (BR-8) kuralı tek bir profilin içinde görülemez; birden çok profili ilgilendirir. Bu yüzden bir **invariant değil**, InternProfileManager (Domain Service) tarafından korunan bir kuraldır.

---

## Workplace aggregate'inin invariant'ları

| # | Invariant | İlgili kural |
|---|---|---|
| I-9 | Çalışma yeri adı boş olamaz. | BR-12 |
| I-10 | Adres bir ilçeye bağlıdır ve adres satırı doludur. | BR-13 |
| I-11 | E-posta girilmişse geçerli formattadır; koordinatlar geçerli aralıktadır. | BR-14 |

---

## MentorReview aggregate'inin invariant'ları

| # | Invariant | İlgili kural |
|---|---|---|
| I-12 | Düzeltme (RevisionRequested) kararında yorum doludur. | BR-26 |

---

## Konum aggregate'lerinin invariant'ları

| # | Invariant | İlgili kural |
|---|---|---|
| I-13 | İl bir ülkeye, ilçe bir ile bağlıdır (bağlantı Id boş olamaz). | BR-28, BR-29 |

---

## Invariant ile "kontrol" farkı

- **Invariant:** Tek bir aggregate'in kendi içinde her zaman koruduğu kural (örn. toplam süre = maddeler toplamı).
- **Kontrol / Domain Service kuralı:** Birden çok aggregate'i veya dış durumu ilgilendiren, aggregate'in tek başına bilemeyeceği kural (örn. "aynı tarihe ikinci günlük var mı?", "kullanıcının başka aktif profili var mı?", "pasif yetkinlik mi?"). Bunlar `domain-services.md` içinde ele alınır.

Bu ayrım önemlidir: aggregate yalnızca **kendi bildiği** veriyle koruyabileceği kuralları invariant olarak taşır; başka aggregate'lere bakmak gerektiğinde Domain Service devreye girer.
