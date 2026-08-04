using CareerManagamentSystem.DTOs;
using CareerManagamentSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace CareerManagamentSystem.Services
{
    public class TrainingRecommendationService
    {
        CareerSystemEntities1 db = new CareerSystemEntities1();

        // Çalışanın eksik yetkinliklerini bulmak için daha önce hazırladığımız servisi kullanıyoruz.
        CompetencyGapService competencyGapService = new CompetencyGapService();


        // Çalışanın hedef pozisyona göre alması gereken eğitimleri bulan metot.
        public List<Trainings> GetRecommendedTrainings(
            int employeeId,
            int targetPositionId)
        {
            // Önerilecek eğitimleri tutacağımız boş liste.
            List<Trainings> recommendedTrainings =
                new List<Trainings>();


            // Önce çalışanın hedef pozisyona göre yetkinlik farklarını alıyoruz.
            List<CompetencyGapDto> gaps =
                competencyGapService.GetCompetencyGaps(
                    employeeId,
                    targetPositionId
                );


            // Yetkinlikleri tek tek kontrol ediyoruz.
            foreach (CompetencyGapDto gap in gaps)
            {
                // Sadece eksik olan yetkinlikler için eğitim önerisi yapıyoruz.
                if (gap.EksikMi)
                {
                    // Trainings tablosundan, eksik yetkinliğe ait eğitimleri buluyoruz.
                    var trainings = db.Trainings
                        .Where(x => x.CompetencyID == gap.CompetencyID)
                        .ToList();


                    // Bulunan eğitimleri sonuç listesine ekliyoruz.
                    foreach (Trainings training in trainings)
                    {
                        // Aynı eğitimin listeye iki kez eklenmesini önlüyoruz.
                        bool alreadyAdded = recommendedTrainings
                            .Any(x => x.TrainingID == training.TrainingID);


                        if (alreadyAdded == false)
                        {
                            recommendedTrainings.Add(training);
                        }
                    }
                }
            }

            // Önerilen eğitimlerin listesini geri döndürüyoruz.
            return recommendedTrainings;
        }
    }
}