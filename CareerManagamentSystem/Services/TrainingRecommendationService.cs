using CareerManagamentSystem.DTOs;
using CareerManagamentSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace CareerManagamentSystem.Services
{
    public class TrainingRecommendationService
    {
        CareerSystemEntities1 db = new CareerSystemEntities1();

        // Çalışanın eksik yetkinliklerini bulmak için kullanılır.
        CompetencyGapService competencyGapService =
            new CompetencyGapService();


        // Çalışanın hedef pozisyona göre alması gereken eğitimleri bulur.
        public List<Trainings> GetRecommendedTrainings(
            int employeeId,
            int targetPositionId)
        {
            List<Trainings> recommendedTrainings =
                new List<Trainings>();


            // Hedef pozisyona göre yetkinlik farkları alınır.
            List<CompetencyGapDto> gaps =
                competencyGapService.GetCompetencyGaps(
                    employeeId,
                    targetPositionId
                );


            // Çalışanın daha önce tamamladığı eğitimlerin ID'leri alınır.
            var tamamlananEgitimler = db.Employee_Trainings
                .Where(x =>
                    x.EmployeeID == employeeId &&
                    x.Status == "Completed")
                .Select(x => x.TrainingID)
                .ToList();


            foreach (CompetencyGapDto gap in gaps)
            {
                // Yalnızca eksik yetkinlikler için eğitim aranır.
                if (gap.EksikMi)
                {
                    var trainings = db.Trainings
                       .Where(x =>
                         x.CompetencyID == gap.CompetencyID &&
                         x.Level > gap.CurrentLevel &&
                         x.Level <= gap.RequiredLevel &&
                         !tamamlananEgitimler.Contains(x.TrainingID))
                       .ToList();

                    foreach (Trainings training in trainings)
                    {
                        // Aynı eğitimin birden fazla kez önerilmesi engellenir.
                        bool zatenEklendiMi =
                            recommendedTrainings.Any(x =>
                                x.TrainingID == training.TrainingID);


                        if (!zatenEklendiMi)
                        {
                            recommendedTrainings.Add(training);
                        }
                    }
                }
            }


            return recommendedTrainings;
        }
    }
}