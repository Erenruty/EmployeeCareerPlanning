using CareerManagamentSystem.DTOs;
using System.Threading.Tasks;

namespace CareerManagamentSystem.Services.AI
{
    // Yapay zeka öneri servisinin kullanacağı metodu tanımlar.
    public interface IAIRecommendationService
    {
        // Kariyer analiz sonucuna göre yapay zeka önerisi oluşturur.
        Task<string> KariyerOnerisiOlusturAsync(
            AIRecommendationDto analiz);
    }
}