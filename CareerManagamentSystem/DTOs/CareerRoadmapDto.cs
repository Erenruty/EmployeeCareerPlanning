using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CareerManagamentSystem.DTOs
{
    // Kariyer yolundaki bir pozisyon geçişinin bilgilerini taşır.
    public class CareerRoadmapDto
    {
        public int CurrentPositionID { get; set; } // Mevcut pozisyon

        public int TargetPositionID { get; set; } // Hedef pozisyon

        public string TargetPositionName { get; set; } // Hedef pozisyonun adı

        public int MinimumExperience { get; set; } // Geçiş için gerekli minimum deneyim

        public decimal MinimumPerformance { get; set; } // Gerekli minimum performans

        // Çalışanın deneyim şartını karşılayıp karşılamadığını gösterir.
        public bool DeneyimUygunMu { get; set; }

        // Çalışanın performans şartını karşılayıp karşılamadığını gösterir.
        public bool PerformansUygunMu { get; set; }

        // Her iki şart da sağlanıyorsa geçiş uygun kabul edilir.
        public bool GeciseUygunMu
        {
            get
            {
                return DeneyimUygunMu && PerformansUygunMu;
            }
        }
    }
}       