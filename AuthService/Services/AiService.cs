using System;
using System.Net.Http;
using System.Net.Http.Json;
using Serilog;
using AuthService.Models;

namespace AuthService.Services
{
    class AiService
    {
        private static HttpClient client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };

        public string Ask(string question)
        {
            var url = Config.Settings["Ollama:Url"] + "/api/generate";
            var model = Config.Settings["Ollama:Model"];

            Log.Information("Запрос к ИИ, модель {Model}: {Question}", model, question);

            var body = new { model = model, prompt = question, stream = false };
            var response = client.PostAsJsonAsync(url, body).Result;
            response.EnsureSuccessStatusCode();

            var result = response.Content.ReadFromJsonAsync<OllamaResponse>().Result;

            Log.Information("Ответ от ИИ получен");

            return result.Response;
        }
    }
}