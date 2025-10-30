using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace APIApp.Services
{
    public class OllamaService : IOllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OllamaService> _logger;

        public OllamaService(HttpClient httpClient, ILogger<OllamaService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri("http://localhost:11434");
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
        }

        public async Task<string> GenerateReportAsync(string dataSummary, string userPrompt, string reportType)
        {
            try
            {
                _logger.LogInformation("Starting AI report generation with Ollama");

                var systemPrompt = GetSystemPrompt(reportType);
                var fullPrompt = $@"{systemPrompt}

DATA TO ANALYZE:
{dataSummary}

USER REQUEST: {userPrompt}

Please analyze the data above and provide a comprehensive report based on the user's request:";

                var aiRequest = new 
                {
                    model = "llama2",
                    prompt = fullPrompt,
                    stream = false
                };

                var json = JsonSerializer.Serialize(aiRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending request to Ollama API...");
                var response = await _httpClient.PostAsync("/api/generate", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Received response from Ollama");
                    
                    using var document = JsonDocument.Parse(responseContent);
                    if (document.RootElement.TryGetProperty("response", out var responseElement))
                    {
                        return responseElement.GetString()?.Trim() ?? "No response generated from AI.";
                    }
                    return "No response generated from AI.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Ollama API error: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Ollama API returned status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Ollama service");
                return $"**AI Report Generation**\n\nDue to a technical issue with the AI service, here's the data summary:\n\n{dataSummary}\n\n**User Request:** {userPrompt}\n\n*Note: Please ensure Ollama is running on http://localhost:11434 with a model like 'llama2' installed.*";
            }
        }

        private string GetSystemPrompt(string reportType)
        {
            var basePrompt = "You are an expert data analyst. Generate a professional, well-structured report based on the provided data and user request. Use clear headings, bullet points, and organized sections.";

            return reportType?.ToLower() switch
            {
                "analysis" => $"{basePrompt} Provide detailed analysis with statistical insights, patterns, anomalies, and business implications.",
                "insights" => $"{basePrompt} Focus on key insights, actionable recommendations, opportunities, and risks.",
                "trends" => $"{basePrompt} Identify trends, patterns over time, growth areas, and future projections.",
                _ => $"{basePrompt} Create a comprehensive summary with executive overview, key metrics, and conclusions."
            };
        }
    }
}