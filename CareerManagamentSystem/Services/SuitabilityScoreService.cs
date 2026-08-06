using System;
using System.Collections.Generic;
using CareerManagamentSystem.DTOs;

namespace CareerManagamentSystem.Services
{
    public class SuitabilityScoreService
    {
        // Yetkinlik farklarını hesaplayan servis.
        CompetencyGapService competencyGapService =
            new CompetencyGapService();


        // Çalışanın hedef pozisyona yetkinlik uygunluk puanını hesaplar.
        public double CalculateSuitabilityScore(
            int employeeId,
            int targetPositionId)
        {
            List<CompetencyGapDto> gaps =
                competencyGapService.GetCompetencyGaps(
                    employeeId,
                    targetPositionId
                );


            // Hedef pozisyon için yetkinlik tanımlanmamışsa puan hesaplanamaz.
            if (gaps.Count == 0)
            {
                return 0;
            }


            double totalScore = 0;

            // Gerçekten puan hesabına katılan yetkinlik sayısı.
            int validCompetencyCount = 0;


            foreach (CompetencyGapDto gap in gaps)
            {
                // RequiredLevel 0 ise bölme işlemi yapılmaz.
                if (gap.RequiredLevel > 0)
                {
                    double competencyScore =
                        (double)gap.CurrentLevel /
                        gap.RequiredLevel;


                    // Uygunluk puanı %100'ü geçmez.
                    if (competencyScore > 1)
                    {
                        competencyScore = 1;
                    }


                    competencyScore =
                        competencyScore * 100;


                    totalScore =
                        totalScore + competencyScore;


                    validCompetencyCount++;
                }
            }


            // Geçerli yetkinlik bulunamazsa puan hesaplanamaz.
            if (validCompetencyCount == 0)
            {
                return 0;
            }


            double suitabilityScore =
                totalScore / validCompetencyCount;


            return Math.Round(
                suitabilityScore,
                2
            );
        }
    }
}