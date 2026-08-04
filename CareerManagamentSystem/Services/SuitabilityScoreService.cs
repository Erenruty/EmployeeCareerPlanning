using System;
using System.Collections.Generic;
using CareerManagamentSystem.DTOs;

namespace CareerManagamentSystem.Services
{
    public class SuitabilityScoreService
    {
        // Yetkinlik farklarını hesaplayan servisten nesne oluşturuyoruz.
        CompetencyGapService competencyGapService = new CompetencyGapService();


        // Çalışanın hedef pozisyona uygunluk puanını hesaplayan metot.
        public double CalculateSuitabilityScore(int employeeId, int targetPositionId)
        {
            // Önce çalışanın hedef pozisyona göre yetkinlik farklarını alıyoruz.
            List<CompetencyGapDto> gaps =
                competencyGapService.GetCompetencyGaps(employeeId, targetPositionId);


            // Eğer hedef pozisyon için herhangi bir yetkinlik tanımlanmamışsa
            // puan hesaplanamayacağı için 0 döndürüyoruz.
            if (gaps.Count == 0)
            {
                return 0;
            }


            // Bütün yetkinliklerden gelecek puanları burada toplayacağız.
            double totalScore = 0;


            // Yetkinlikleri tek tek kontrol ediyoruz.
            foreach (CompetencyGapDto gap in gaps)
            {
                // Gerekli seviyenin 0 olması durumunda
                // bölme hatası oluşmaması için kontrol ediyoruz.
                if (gap.RequiredLevel > 0)
                {
                    // Çalışanın mevcut seviyesini gerekli seviyeye bölüyoruz.
                    double competencyScore =
                        (double)gap.CurrentLevel / gap.RequiredLevel;


                    // Çalışanın seviyesi gerekli seviyeden daha yüksek olabilir.
                    // Bu durumda puanın %100'ü geçmesini istemiyoruz.
                    if (competencyScore > 1)
                    {
                        competencyScore = 1;
                    }


                    // Yetkinlik puanını yüzde değerine çeviriyoruz.
                    competencyScore = competencyScore * 100;


                    // Hesaplanan puanı toplam puana ekliyoruz.
                    totalScore = totalScore + competencyScore;
                }
            }


            // Bütün yetkinlik puanlarının ortalamasını alıyoruz.
            double suitabilityScore = totalScore / gaps.Count;


            // Sonucu virgülden sonra 2 basamak olacak şekilde döndürüyoruz.
            return Math.Round(suitabilityScore, 2);
        }
    }
}