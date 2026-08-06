using CareerManagamentSystem.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CareerManagamentSystem.Services.AI
{
    // Kariyer analizini Gemini API'ye gönderir
    // ve oluşturulan öneri metnini döndürür.
    public class GeminiRecommendationService : IAIRecommendationService
    {
        private static readonly HttpClient httpClient =
            new HttpClient();

        private readonly AIRecommendationService aiRecommendationService;

        public GeminiRecommendationService()
        {
            aiRecommendationService =
                new AIRecommendationService();
        }

        // Kariyer analiz sonucunu Gemini'ye göndererek öneri oluşturur.
        public async Task<string> KariyerOnerisiOlusturAsync(
            AIRecommendationDto analiz)
        {
            // Analiz verisi yoksa istek gönderilmez.
            if (analiz == null)
            {
                return "Kariyer analizi verisi bulunamadı.";
            }

            // API anahtarı ortam değişkeninden alınır.
            string apiKey =
                Environment.GetEnvironmentVariable(
                    "GEMINI_API_KEY"
                );

            // API anahtarı yoksa standart öneri kullanılır.
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return StandartOneriOlustur(analiz);
            }

            // Gemini'ye gönderilecek prompt hazırlanır.
            string prompt =
                aiRecommendationService.PromptOlustur(
                    analiz
                );

            string model =
                "gemini-2.5-flash";

            string url =
                "https://generativelanguage.googleapis.com/v1beta/models/"
                + model
                + ":generateContent";

            // Gemini API'nin beklediği JSON hazırlanır.
            var istekGovdesi = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };

            string json =
                JsonConvert.SerializeObject(
                    istekGovdesi
                );

            try
            {
                using (HttpRequestMessage istek =
                    new HttpRequestMessage(
                        HttpMethod.Post,
                        url
                    ))
                {
                    istek.Headers.Add(
                        "x-goog-api-key",
                        apiKey
                    );

                    istek.Content =
                        new StringContent(
                            json,
                            Encoding.UTF8,
                            "application/json"
                        );

                    // Gemini API'ye istek gönderilir.
                    HttpResponseMessage cevap =
                        await httpClient.SendAsync(
                            istek
                        );

                    // API başarısızsa standart öneriye dönülür.
                    if (!cevap.IsSuccessStatusCode)
                    {
                        return StandartOneriOlustur(
                            analiz
                        );
                    }

                    string cevapJson =
                        await cevap.Content
                            .ReadAsStringAsync();

                    JObject cevapNesnesi =
                        JObject.Parse(
                            cevapJson
                        );

                    // Gemini'nin oluşturduğu metin alınır.
                    string oneriMetni =
                        cevapNesnesi["candidates"]?[0]?
                        ["content"]?["parts"]?[0]?
                        ["text"]?.ToString();

                    if (string.IsNullOrWhiteSpace(
                        oneriMetni))
                    {
                        return StandartOneriOlustur(
                            analiz
                        );
                    }

                    return oneriMetni.Trim();
                }
            }
            catch
            {
                // API veya bağlantı hatasında sistem çalışmaya devam eder.
                return StandartOneriOlustur(
                    analiz
                );
            }
        }

        // Gemini kullanılamadığında temel öneri oluşturur.
        private string StandartOneriOlustur(
            AIRecommendationDto analiz)
        {
            string eksikYetkinlikler =
                "belirgin bir yetkinlik eksikliği bulunmamaktadır";

            string egitimler =
                "özel bir eğitim önerisi bulunmamaktadır";


            // Yetkinlik listesi varsa eksikler alınır.
            if (analiz.CompetencyGaps != null)
            {
                string bulunanYetkinlikler =
                    string.Join(
                        ", ",
                        analiz.CompetencyGaps
                            .Where(x => x.EksikMi)
                            .Select(x => x.CompetencyName)
                    );

                if (!string.IsNullOrWhiteSpace(
                    bulunanYetkinlikler))
                {
                    eksikYetkinlikler =
                        bulunanYetkinlikler;
                }
            }


            // Eğitim listesi varsa metne eklenir.
            if (analiz.RecommendedTrainings != null)
            {
                string bulunanEgitimler =
                    string.Join(
                        ", ",
                        analiz.RecommendedTrainings
                    );

                if (!string.IsNullOrWhiteSpace(
                    bulunanEgitimler))
                {
                    egitimler =
                        bulunanEgitimler;
                }
            }


            string gecisDurumu =
                analiz.HedefGeciseUygunMu
                    ? "Mevcut deneyim ve performans şartları hedef pozisyon için sağlanmaktadır."
                    : "Hedef pozisyon mevcut durumda doğrudan geçişten ziyade gelişim hedefi olarak değerlendirilmelidir.";


            return
                analiz.TargetPositionName
                + " pozisyonu için yetkinlik uygunluk puanı %"
                + analiz.SuitabilityScore
                + " olarak hesaplanmıştır. "
                + gecisDurumu
                + " Gelişim alanları: "
                + eksikYetkinlikler
                + ". Önerilen eğitimler: "
                + egitimler
                + ".";
        }
    }
}