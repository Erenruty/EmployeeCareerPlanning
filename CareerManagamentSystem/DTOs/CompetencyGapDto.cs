using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CareerManagamentSystem.DTOs
{
    // Çalışanın mevcut yetkinlik seviyesi ile hedef pozisyonun istediği seviye arasındaki farkı taşır.
    public class CompetencyGapDto
    {
        public int CompetencyID { get; set; } // Competencies tablosundaki yetkinlik ID'si

        public string CompetencyName { get; set; } // Competencies tablosundaki yetkinlik adı

        public int CurrentLevel { get; set; } // Employee_Competencies tablosundaki mevcut seviye

        public int RequiredLevel { get; set; } // Position_Requirements tablosundaki gerekli seviye

        // RequiredLevel - CurrentLevel sonucu
        public int Gap { get; set; }

        // Yetkinlik eksikliği olup olmadığını kontrol eder.
        public bool EksikMi
        {
            get
            {
                return Gap > 0;
            }
        }

        // Gap 2 veya daha fazlaysa kritik kabul edilir.
        public bool KritikMi
        {
            get
            {
                return Gap >= 2;
            }
        }
    }
}