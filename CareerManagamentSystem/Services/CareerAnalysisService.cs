using CareerManagamentSystem.DTOs;
using CareerManagamentSystem.Models;
using CareerManagamentSystem.Services.AI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly IAIRecommendationService aiRecommendationService;
        private readonly AIRecommendationSaveService aiRecommendationSaveService;

        public CareerAnalysisService()
        {
            db = new CareerSystemEntities1();

            competencyGapService = new CompetencyGapService();
            suitabilityScoreService = new SuitabilityScoreService();
            trainingRecommendationService = new TrainingRecommendationService();
            careerRoadmapService = new CareerRoadmapService();

            // Yapay zeka önerileri için Gemini servisi kullanılır.
            aiRecommendationService = new GeminiRecommendationService();

            // Oluşturulan AI önerisini veritabanına kaydetmek için kullanılır.
            aiRecommendationSaveService = new AIRecommendationSaveService();

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

                // Hedef pozisyon için deneyim ve performans şartlarının durumu tutulur.
                sonuc.HedefGeciseUygunMu =
                    enUygunGecis.GeciseUygunMu;


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

        // Kariyer analiz sonucunu yapay zekaya gönderilecek veri yapısına dönüştürür.
        public AIRecommendationDto AIVerisiOlustur(CareerAnalysisResultDto analizSonucu)
        {
            if (analizSonucu == null)
            {
                return null;
            }

            AIRecommendationDto aiVerisi =
                new AIRecommendationDto
                {
                    CurrentPositionName =
                        analizSonucu.CurrentPositionName,

                    TargetPositionName =
                        analizSonucu.TargetPositionName,

                    SuitabilityScore =
                        analizSonucu.SuitabilityScore,

                    HedefGeciseUygunMu =
                        analizSonucu.HedefGeciseUygunMu,

                    CompetencyGaps =
                        analizSonucu.CompetencyGaps,

                    RecommendedTrainings =
                        analizSonucu.RecommendedTrainings
                            .Select(x => x.TrainingName)
                            .ToList()
                };

            return aiVerisi;
        }
        // Kariyer analizini yapar ve sonucu Gemini ile yorumlatır.
        public async Task<CareerAnalysisResultDto> KariyerAnaliziVeAIOnerisiYapAsync(
            int employeeId)
        {
            // Önce mevcut business logic ile kariyer analizi yapılır.
            CareerAnalysisResultDto analizSonucu =
                KariyerAnaliziYap(employeeId);

            // Çalışan bulunamazsa işlem devam etmez.
            if (analizSonucu == null)
            {
                return null;
            }

            // Hedef pozisyon oluşmadıysa AI'ya gönderilecek yeterli veri yoktur.
            if (!analizSonucu.TargetPositionID.HasValue)
            {
                analizSonucu.RecommendationText =
                    "Çalışan için tanımlı bir kariyer hedefi bulunamadı.";

                return analizSonucu;
            }

            // Business logic sonucu AI'nın kullanacağı yapıya dönüştürülür.
            AIRecommendationDto aiVerisi =
                AIVerisiOlustur(analizSonucu);

            // Gemini'den kariyer gelişim önerisi alınır.
            string aiOnerisi =
                await aiRecommendationService
                    .KariyerOnerisiOlusturAsync(aiVerisi);

            // Oluşturulan AI metni genel analiz sonucuna eklenir.
            analizSonucu.RecommendationText = aiOnerisi;

            // Analiz sonucu AIRecommendations ve Recommendation_Training
            // tablolarına kaydedilir.
            int recommendationId =
                aiRecommendationSaveService.OneriyiKaydet(analizSonucu);

            // Oluşturulan kayıt ID'si sonuç içerisinde tutulur.
            if (recommendationId > 0)
            {
                analizSonucu.RecommendationID = recommendationId;
            }

            return analizSonucu;
        }
    }
}