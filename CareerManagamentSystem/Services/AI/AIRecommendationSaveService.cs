using CareerManagamentSystem.DTOs;
using CareerManagamentSystem.Models;
using System;
using System.Linq;

namespace CareerManagamentSystem.Services.AI
{
    // Kariyer analizi ve AI önerisini veritabanına kaydeder.
    public class AIRecommendationSaveService
    {
        private readonly CareerSystemEntities1 db;

        public AIRecommendationSaveService()
        {
            db = new CareerSystemEntities1();
        }

        // AIRecommendations ve Recommendation_Training tablolarına kayıt yapar.
        public int OneriyiKaydet(CareerAnalysisResultDto analizSonucu)
        {
            // Analiz veya hedef pozisyon yoksa kayıt yapılmaz.
            if (analizSonucu == null ||
                !analizSonucu.TargetPositionID.HasValue)
            {
                return 0;
            }

            // Uygunluk puanına göre yetkinlik açığı hesaplanır.
            decimal gapScore =
                100 - (decimal)analizSonucu.SuitabilityScore;

            if (gapScore < 0)
            {
                gapScore = 0;
            }

            // AI öneri kaydı hazırlanır.
            AIRecommendations recommendation =
                new AIRecommendations
                {
                    EmployeeID = analizSonucu.EmployeeID,

                    TargetPositionID =
                        analizSonucu.TargetPositionID.Value,

                    GapScore =
                        Math.Round(gapScore, 2),

                    RecommendationText =
                        analizSonucu.RecommendationText,

                    CreatedDate =
                        DateTime.Now
                };

            // Önce AIRecommendations kaydı oluşturulur.
            db.AIRecommendations.Add(recommendation);
            db.SaveChanges();

            // Önerilen eğitimler varsa ana öneriye bağlanır.
            if (analizSonucu.RecommendedTrainings != null)
            {
                foreach (Trainings training
                    in analizSonucu.RecommendedTrainings)
                {
                    bool kayitVarMi =
                        db.Recommendation_Training.Any(x =>
                            x.AIRecommendationID ==
                                recommendation.RecommendationID
                            &&
                            x.TrainingID ==
                                training.TrainingID);

                    if (!kayitVarMi)
                    {
                        Recommendation_Training recommendationTraining =
                            new Recommendation_Training
                            {
                                AIRecommendationID =
                                    recommendation.RecommendationID,

                                TrainingID =
                                    training.TrainingID
                            };

                        db.Recommendation_Training
                            .Add(recommendationTraining);
                    }
                }
            }

            // Eğitim bağlantıları kaydedilir.
            db.SaveChanges();

            // Oluşturulan önerinin ID'si geri döndürülür.
            return recommendation.RecommendationID;
        }
    }
}