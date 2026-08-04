using CareerManagamentSystem.DTOs;
using CareerManagamentSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace CareerManagamentSystem.Services
{
    public class CompetencyGapService
    {
        CareerSystemEntities1 db = new CareerSystemEntities1();

        // Bu metot çalışanın yetkinlikleri ile hedef pozisyonun istediği yetkinlikleri karşılaştırır.
        public List<CompetencyGapDto> GetCompetencyGaps(int employeeId, int targetPositionId)
        {
            // Hesaplanan yetkinlik farklarını tutacağımız boş liste.
            List<CompetencyGapDto> gapList = new List<CompetencyGapDto>();


            // Hedef pozisyonun istediği bütün yetkinlikleri veritabanından alıyoruz.
            var requirements = db.Position_Requirements
                .Where(x => x.PositionID == targetPositionId)
                .ToList();


            // Pozisyon için gerekli olan yetkinlikleri tek tek kontrol ediyoruz.
            foreach (var requirement in requirements)
            {
                // Yetkinliğin adını öğrenebilmek için Competencies tablosundan ilgili yetkinliği buluyoruz.
                var competency = db.Competencies
                    .FirstOrDefault(x =>
                        x.CompetencyID == requirement.CompetencyID);


                // Çalışanın bu yetkinliğe sahip olup olmadığını kontrol ediyoruz. Hem çalışan ID'si hem de yetkinlik ID'si eşleşmeli.
                var employeeCompetency = db.Employee_Competencies
                    .FirstOrDefault(x =>
                        x.EmployeeID == employeeId &&
                        x.CompetencyID == requirement.CompetencyID);


                // Başlangıçta çalışanın seviyesini 0 kabul ediyoruz. Eğer çalışanın o yetkinliği hiç yoksa seviyesi 0 olarak kalacak.
                int currentLevel = 0;


                // Eğer çalışanın bu yetkinliği varsa  mevcut seviyesini veritabanından alıyoruz.
                if (employeeCompetency != null)
                {
                    currentLevel = employeeCompetency.CurrentLevel;
                }

                // Pozisyonun istediği seviye ile çalışanın mevcut seviyesi arasındaki farkı hesaplıyoruz
                int gap = requirement.RequiredLevel - currentLevel;

                // Hesaplanan bilgileri taşımak için yeni bir DTO oluşturuyoruz.
                CompetencyGapDto result = new CompetencyGapDto();

                // Yetkinliğin ID bilgisini DTO'ya aktarıyoruz.
                result.CompetencyID = requirement.CompetencyID;

                // Yetkinliğin adını DTO'ya aktarıyoruz.
                result.CompetencyName = competency.CompetencyName;

                // Çalışanın mevcut seviyesini aktarıyoruz.
                result.CurrentLevel = currentLevel;

                // Pozisyonun istediği seviyeyi aktarıyoruz.
                result.RequiredLevel = requirement.RequiredLevel;

                // Aradaki farkı aktarıyoruz.
                result.Gap = gap;

                // Hazırladığımız sonucu listeye ekliyoruz.
                gapList.Add(result);
            }

            // Bütün yetkinlikler kontrol edildikten sonra sonuç listesini geri döndürüyoruz.
            return gapList;
        }
    }
}