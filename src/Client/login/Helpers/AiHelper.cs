using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace login.Helpers
{
    internal static class AiHelper
    {
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private static string GetApiKey()
        {
            // Use your real API key here.
            return "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";
        }

        public static async Task<string> GenerateOverviewAsync(decimal income, decimal expenses)
        {
            var apiKey = GetApiKey();
            if (string.IsNullOrEmpty(apiKey))
                return "AI summary unavailable: missing API key.";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var prompt = $"Provide a short financial overview. Monthly income: {income:C2}. Monthly expenses: {expenses:C2}.";

                var request = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are a helpful finance assistant." },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 100,
                    temperature = 0.7
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(ApiUrl, content);
                    var body = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        // Optionally, add details for debugging
                        return $"AI request failed: {response.StatusCode}\n{body}";
                    }

                    using (var doc = JsonDocument.Parse(body))
                    {
                        var msg = doc.RootElement
                                     .GetProperty("choices")[0]
                                     .GetProperty("message")
                                     .GetProperty("content")
                                     .GetString();
                        return msg?.Trim() ?? "AI response empty.";
                    }
                }
                catch (Exception ex)
                {
                    return "AI request failed: " + ex.Message;
                }
            }
        }
    }
}
