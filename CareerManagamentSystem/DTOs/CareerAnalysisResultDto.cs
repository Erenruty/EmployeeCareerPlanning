using CareerManagamentSystem.Models;
using System.Collections.Generic;

namespace CareerManagamentSystem.DTOs
{
    // Bir çalışan için oluşturulan kariyer analizinin genel sonucunu taşır.
    public class CareerAnalysisResultDto
    {
        public int EmployeeID { get; set; }

        public int CurrentPositionID { get; set; }

        public string CurrentPositionName { get; set; }

        // Kariyer yolu bulunamazsa boş kalabilmesi için nullable tanımlandı.
        public int? TargetPositionID { get; set; }

        public string TargetPositionName { get; set; }

        // Hedef pozisyona göre hesaplanan uygunluk puanı
        public double SuitabilityScore { get; set; }

        // Çalışanın hedef pozisyona göre yetkinlik farkları
        public List<CompetencyGapDto> CompetencyGaps { get; set; }

        // Eksik yetkinliklere göre önerilen eğitimler
        public List<Trainings> RecommendedTrainings { get; set; }

        // Çalışanın mevcut pozisyonundan ilerleyebileceği kariyer yolu
        public List<CareerRoadmapDto> CareerRoadmap { get; set; }

        // Daha sonra AI tarafından oluşturulacak öneri metni
        public string RecommendationText { get; set; }

        public CareerAnalysisResultDto()
        {
            CompetencyGaps = new List<CompetencyGapDto>();
            RecommendedTrainings = new List<Trainings>();
            CareerRoadmap = new List<CareerRoadmapDto>();
        }
    }
}