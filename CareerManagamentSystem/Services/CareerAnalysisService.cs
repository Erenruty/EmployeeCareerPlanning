using CareerManagamentSystem.DTOs;
using CareerManagamentSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace CareerManagamentSystem.Services
{
    // Kariyer analizi için hazırlanan servisleri bir araya getirir.
    public class CareerAnalysisService
    {
        private readonly CareerSystemEntities1 db;

        private readonly CompetencyGapService competencyGapService;
        private readonly SuitabilityScoreService suitabilityScoreService;
        private readonly TrainingRecommendationService trainingRecommendationService;
        private readonly CareerRoadmapService careerRoadmapService;

        public CareerAnalysisService()
        {
            db = new CareerSystemEntities1();

            competencyGapService = new CompetencyGapService();
            suitabilityScoreService = new SuitabilityScoreService();
            trainingRecommendationService = new TrainingRecommendationService();
            careerRoadmapService = new CareerRoadmapService();
        }


        // Çalışan için kariyer analizini gerçekleştirir.
        public CareerAnalysisResultDto KariyerAnaliziYap(int employeeId)
        {
            // EmployeeID'ye göre çalışan bulunur.
            var calisan = db.Employees
                .FirstOrDefault(x => x.EmployeeID == employeeId);

            // Çalışan bulunamazsa analiz yapılamaz.
            if (calisan == null)
            {
                return null;
            }


            // Çalışanın mevcut pozisyon bilgisi alınır.
            var mevcutPozisyon = db.Positions
                .FirstOrDefault(x => x.PositionID == calisan.PositionID);


            // Çalışanın CareerPath tablosuna göre kariyer geçişleri bulunur.
            List<CareerRoadmapDto> kariyerYolu =
                careerRoadmapService.KariyerYolunuOlustur(employeeId);


            // Genel analiz sonucu oluşturulur.
            var sonuc = new CareerAnalysisResultDto
            {
                EmployeeID = calisan.EmployeeID,

                CurrentPositionID = calisan.PositionID,

                CurrentPositionName = mevcutPozisyon != null
                    ? mevcutPozisyon.PositionName
                    : "Bilinmeyen Pozisyon",

                CareerRoadmap = kariyerYolu
            };


            // Çalışanın mevcut pozisyonundan tanımlı bir kariyer geçişi yoksa
            // mevcut bilgilerle sonuç döndürülür.
            if (kariyerYolu.Count == 0)
            {
                return sonuc;
            }


            // Önce deneyim ve performans şartlarını sağlayan geçişler alınır.
            List<CareerRoadmapDto> adayGecisler = kariyerYolu
                .Where(x => x.GeciseUygunMu)
                .ToList();


            // Şartları tamamen sağlayan geçiş yoksa
            // gelişim hedefi olarak tüm kariyer geçişleri değerlendirilir.
            if (adayGecisler.Count == 0)
            {
                adayGecisler = kariyerYolu;
            }


            CareerRoadmapDto enUygunGecis = null;

            double enYuksekPuan = -1;


            // Aday pozisyonların uygunluk puanları karşılaştırılır.
            foreach (CareerRoadmapDto gecis in adayGecisler)
            {
                double puan =
                    suitabilityScoreService.CalculateSuitabilityScore(
                        employeeId,
                        gecis.TargetPositionID
                    );


                // En yüksek uygunluk puanına sahip pozisyon tutulur.
                if (puan > enYuksekPuan)
                {
                    enYuksekPuan = puan;
                    enUygunGecis = gecis;
                }
            }


            if (enUygunGecis != null)
            {
                // En uygun hedef pozisyon analiz sonucuna eklenir.
                sonuc.TargetPositionID =
                    enUygunGecis.TargetPositionID;

                sonuc.TargetPositionName =
                    enUygunGecis.TargetPositionName;

                sonuc.SuitabilityScore =
                    enYuksekPuan;


                // Seçilen hedef pozisyona göre yetkinlik farkları hesaplanır.
                sonuc.CompetencyGaps =
                    competencyGapService.GetCompetencyGaps(
                        employeeId,
                        enUygunGecis.TargetPositionID
                    );


                // Eksik yetkinliklere göre eğitim önerileri alınır.
                sonuc.RecommendedTrainings =
                    trainingRecommendationService.GetRecommendedTrainings(
                        employeeId,
                        enUygunGecis.TargetPositionID
                    );
            }


            return sonuc;
        }
    }
}