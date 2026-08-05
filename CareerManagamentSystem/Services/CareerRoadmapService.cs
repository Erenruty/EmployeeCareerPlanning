using CareerManagamentSystem.DTOs;
using CareerManagamentSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace CareerManagamentSystem.Services
{
    // Çalışanın mevcut pozisyonuna göre geçebileceği kariyer adımlarını belirler.
    public class CareerRoadmapService
    {
        private readonly CareerSystemEntities1 db;

        // Veritabanı bağlantısını oluşturur.
        public CareerRoadmapService()
        {
            db = new CareerSystemEntities1();
        }

        // Çalışanın mevcut pozisyonundan geçebileceği pozisyonları oluşturur.
        public List<CareerRoadmapDto> KariyerYolunuOlustur(int employeeId)
        {
            // Kariyer yolu sonuçlarının tutulacağı liste
            var sonuc = new List<CareerRoadmapDto>();

            // EmployeeID'ye göre çalışan bilgisi alınır.
            var calisan = db.Employees
                .FirstOrDefault(x => x.EmployeeID == employeeId);

            // Çalışan bulunamazsa boş sonuç döndürülür.
            if (calisan == null)
            {
                return sonuc;
            }

            // Çalışanın en güncel performans kaydı alınır.
            var performans = db.Performance
                .Where(x => x.EmployeeID == employeeId)
                .OrderByDescending(x => x.EvaluationDate)
                .FirstOrDefault();

            // Performans kaydı yoksa puan 0 kabul edilir.
            decimal performansPuani = 0;

            if (performans != null && performans.PerformanceScore.HasValue)
            {
                performansPuani = performans.PerformanceScore.Value;
            }

            // Mevcut pozisyondan yapılabilecek kariyer geçişleri alınır.
            var kariyerGecisleri = db.CareerPath
                .Where(x => x.CurrentPositionID == calisan.PositionID)
                .ToList();

            // Bulunan her kariyer geçişi değerlendirilir.
            foreach (var gecis in kariyerGecisleri)
            {
                // Geçiş yapılabilecek hedef pozisyon bulunur.
                var hedefPozisyon = db.Positions
                    .FirstOrDefault(x =>
                        x.PositionID == gecis.TargetPositionID);

                // Kariyer geçişi için gerekli minimum deneyim alınır.
                int minimumDeneyim = gecis.MinimumExperience ?? 0;

                // Kariyer geçişi için gerekli minimum performans alınır.
                decimal minimumPerformans =
                    gecis.MinimumPerformance ?? 0;

                // Çalışanın toplam deneyim yılı alınır.
                int calisanDeneyimi =
                    calisan.TotalExperienceYear ?? 0;

                // Kariyer geçişi sonucu listeye eklenir.
                sonuc.Add(new CareerRoadmapDto
                {
                    CurrentPositionID = gecis.CurrentPositionID,

                    TargetPositionID = gecis.TargetPositionID,

                    TargetPositionName = hedefPozisyon != null
                        ? hedefPozisyon.PositionName
                        : "Bilinmeyen Pozisyon",

                    MinimumExperience = minimumDeneyim,

                    MinimumPerformance = minimumPerformans,

                    // Çalışanın deneyim şartını sağlayıp sağlamadığı kontrol edilir.
                    DeneyimUygunMu =
                        calisanDeneyimi >= minimumDeneyim,

                    // Çalışanın performans şartını sağlayıp sağlamadığı kontrol edilir.
                    PerformansUygunMu =
                        performansPuani >= minimumPerformans
                });
            }

            // Oluşturulan kariyer yolu sonuçları döndürülür.
            return sonuc;
        }
    }
}