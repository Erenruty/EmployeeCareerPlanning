using System.Collections.Generic;

namespace CareerManagamentSystem.DTOs
{
    // Yapay zekaya gönderilecek kariyer analiz bilgilerini taşır.
    public class AIRecommendationDto
    {
        public string CurrentPositionName { get; set; }

        public string TargetPositionName { get; set; }

        public double SuitabilityScore { get; set; }
        public bool HedefGeciseUygunMu { get; set; }

        // Hedef pozisyona göre yetkinlik farkları
        public List<CompetencyGapDto> CompetencyGaps { get; set; }

        // Eksik yetkinliklere göre önerilen eğitimlerin adları
        public List<string> RecommendedTrainings { get; set; }

        public AIRecommendationDto()
        {
            CompetencyGaps = new List<CompetencyGapDto>();
            RecommendedTrainings = new List<string>();
        }
    }
}