using CareerManagamentSystem.DTOs;
using System.Text;

namespace CareerManagamentSystem.Services.AI
{
    // Kariyer analizi sonucunu yapay zekaya gönderilecek metne dönüştürür.
    public class AIRecommendationService
    {
        public string PromptOlustur(AIRecommendationDto analiz)
        {
            StringBuilder prompt = new StringBuilder();

            // Analizin temel sonuçları eklenir.
            prompt.AppendLine("Aşağıdaki kariyer analizi sonucuna göre çalışan için gelişim önerisi oluştur.");
            prompt.AppendLine();

            prompt.AppendLine("Mevcut Pozisyon: " + analiz.CurrentPositionName);
            prompt.AppendLine("Hedef Pozisyon: " + analiz.TargetPositionName);
            prompt.AppendLine("Uygunluk Puanı: %" + analiz.SuitabilityScore);
            prompt.AppendLine(
                  "Mevcut durumda hedef pozisyona geçiş şartları: " +
                  (analiz.HedefGeciseUygunMu
                    ? "Sağlanıyor"
                    : "Henüz sağlanmıyor")
            );
            prompt.AppendLine();

            // Sadece eksik yetkinlikler yapay zekaya gönderilir.
            prompt.AppendLine("Eksik Yetkinlikler:");

            foreach (CompetencyGapDto gap in analiz.CompetencyGaps)
            {
                if (gap.EksikMi)
                {
                    prompt.AppendLine(
                        "- " + gap.CompetencyName +
                        ": mevcut seviye " + gap.CurrentLevel +
                        ", gerekli seviye " + gap.RequiredLevel +
                        ", fark " + gap.Gap
                    );
                }
            }

            prompt.AppendLine();

            // Business logic tarafından bulunan eğitimler eklenir.
            prompt.AppendLine("Önerilen Eğitimler:");

            foreach (string training in analiz.RecommendedTrainings)
            {
                prompt.AppendLine("- " + training);
            }

            prompt.AppendLine();

            // Yapay zekanın hesaplanan sonucu değiştirmemesi için kurallar eklenir.
            prompt.AppendLine("Kurallar:");
            prompt.AppendLine("- Uygunluk puanını değiştirme.");
            prompt.AppendLine("- Hedef pozisyonu değiştirme.");
            prompt.AppendLine("- Yeni pozisyon, eğitim veya yetkinlik uydurma.");
            prompt.AppendLine("- Yalnızca verilen analiz bilgilerini kullan.");
            prompt.AppendLine("- Kesin terfi veya başarı garantisi verme.");
            prompt.AppendLine("- Geçiş şartları sağlanmıyorsa hedef pozisyonu mevcut terfi imkanı değil, gelişim hedefi olarak değerlendir.");
            prompt.AppendLine();

            prompt.AppendLine(
                "Çalışanın güçlü yönlerini, gelişim alanlarını ve uygulanabilir kariyer gelişim önerilerini kısa ve profesyonel şekilde açıkla."
            );

            return prompt.ToString();
        }
    }
}