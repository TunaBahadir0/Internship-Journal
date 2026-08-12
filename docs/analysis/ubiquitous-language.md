# Ortak Dil (Ubiquitous Language)

Bu sözlük, projede geçen terimlerin **tek ve ortak** tanımını verir. Amaç: analiz, kod ve konuşmada herkesin aynı kelimeyi aynı anlamda kullanmasıdır. Aynı kavram için farklı kelimeler kullanılırsa yanlış anlaşılmalar ve hatalı model oluşur.

---

## Kişiler ve roller

**Stajyer (Intern):** Staj yapan ve günlük tutan kullanıcı. Yalnızca kendi verilerine erişir.

**Mentor:** Stajyerin günlüklerini inceleyen, onaylayan veya düzeltme isteyen sorumlu kullanıcı.

**Admin:** Referans verilerini (konum, yetkinlik), kullanıcıları ve eşleştirmeleri yöneten kullanıcı.

## Yapılar

**Çalışma Yeri (Workplace):** Stajyerin staj yaptığı firma/iş yeri. Bir ilçeye bağlı adresi vardır.

**Staj Profili (InternProfile):** Bir kullanıcının staj bilgilerini (çalışma yeri, mentor, dönem, okul) tutan kayıt. Bir kullanıcının aynı anda tek aktif profili olur.

**Staj Dönemi (Internship Period):** Stajın başlangıç ve bitiş tarihini kapsayan aralık (bir DateRange değeri).

**Yetkinlik (Skill):** Öğrenilen/çalışılan teknik veya genel beceri (örn. C#, Docker, DDD). Aktif/pasif olabilir.

**Öğrenme Seviyesi (LearningLevel):** Bir yetkinliğin o günkü çalışılma derinliği: Introduced (tanıtıldı), Practiced (uygulandı-pratik), Applied (gerçek işte uygulandı), Improved (geliştirildi).

## Günlük ve içeriği

**Günlük (DailyLog):** Stajyerin belirli bir çalışma gününde yaptığı faaliyetleri, süreleri, yetkinlikleri ve problemleri içeren kayıt. Ana aggregate'tir.

**Çalışma Maddesi (DailyLogItem):** Günlük içinde tek bir yapılan işi temsil eder (başlık, tür, süre). Günlüğün child'ıdır.

**Çalışma Türü (WorkType):** Bir çalışma maddesinin sınıfı: Setup, Training, Development, Research, Testing, Documentation, Meeting, ProblemSolving.

**Günlük Yetkinliği (DailyLogSkill):** Bir günlükte çalışılan yetkinliğin, seviyesi ve notuyla birlikte kaydı. Aynı yetkinlik bir günlükte tek kez.

**Problem Kaydı (ProblemSolvingEntry):** Karşılaşılan bir teknik problemin açıklaması, denenen çözümler, kök neden, nihai çözüm ve varsa yapay zekâ kullanımı.

**Kök Neden (Root Cause):** Problemin asıl kaynağı (belirtiyle değil, gerçek sebeple ilgili).

**Nihai Çözüm (Final Solution):** Problemi çözen, uygulanan asıl çözüm.

## Süreç ve inceleme

**Mentor İncelemesi (MentorReview):** Mentorun bir günlüğe verdiği karar (onay/düzeltme) ve yorumu.

**Onay (Approval):** Mentorun günlüğü kabul etmesi; günlük "Approved" durumuna geçer ve dondurulur.

**Düzeltme Talebi (Revision Request):** Mentorun, zorunlu yorumla günlüğü stajyere geri göndermesi; günlük "RevisionRequested" durumuna geçer.

**Günlük Durumu (DailyLogStatus):** Günlüğün yaşam döngüsü aşaması: Draft (taslak), Submitted (gönderildi), RevisionRequested (düzeltme istendi), Approved (onaylandı).

## Konum

**Ülke (Country):** En üst konum birimi (kod + ad). Örn. TR / Türkiye.

**İl (Province):** Bir ülkeye bağlı konum birimi. Örn. İstanbul.

**İlçe (District):** Bir ile bağlı konum birimi. Örn. Şişli. Çalışma yeri adresi doğrudan ilçeye bağlanır.

---

## Terim tutarlılığı notları

- "Günlük" her zaman **DailyLog**'u ifade eder; "kayıt" gibi belirsiz kelimeler yerine kullanılır.
- "Madde" = **DailyLogItem** (çalışma maddesi); yetkinlik veya problemle karıştırılmaz.
- "İnceleme/onay/düzeltme" mentor tarafındaki eylemlerdir; stajyer "gönderir".
- Toplam süreden bahsederken her zaman "maddelerden hesaplanan" toplam kastedilir (kullanıcı girişi değil).
