using Application.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Gọi Gemini API
    /// </summary>
    public class GeminiLLMService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<GeminiLLMService> _logger;

        public GeminiLLMService(
            HttpClient httpClient, 
            IConfiguration configuration,
            ILogger<GeminiLLMService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["AiSettings:GeminiApiKey"] 
                ?? throw new Exception("❌ Gemini API Key không tìm thấy trong appsettings.json!");
            _model = configuration["AiSettings:GeminiModel"] ?? "gemini-pro";
            
            _logger.LogInformation($"✅ Gemini LLM Service initialized with model: {_model}");
        }

        /// <summary>
        /// Gọi Gemini để lấy phản hồi
        /// </summary>
        public async Task<string> GetCompletionAsync(string prompt)
        {
            try
            {
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

                _logger.LogInformation($"🔄 Gọi Gemini API với model: {_model}");
                _logger.LogInformation($"📝 Prompt: {prompt.Substring(0, Math.Min(100, prompt.Length))}...");

                // Tạo request body
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 2048,
                    },
                    safetySettings = new[]
                    {
                        new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_ONLY_HIGH" },
                        new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_ONLY_HIGH" },
                        new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_ONLY_HIGH" },
                        new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_ONLY_HIGH" }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gọi Gemini API
                _logger.LogInformation($"🌐 URL: {url.Replace(_apiKey, "***")}");
                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📊 Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"❌ Gemini Error {response.StatusCode}:");
                    _logger.LogError($"   Response: {responseString}");
                    
                    // Parse error message
                    try
                    {
                        var errorDoc = JsonDocument.Parse(responseString);
                        var errorMessage = errorDoc.RootElement
                            .GetProperty("error")
                            .GetProperty("message")
                            .GetString() ?? "Unknown error";
                        
                        return $"❌ Lỗi Gemini API: {errorMessage} (HTTP {response.StatusCode})";
                    }
                    catch
                    {
                        return $"❌ Lỗi Gemini API: HTTP {response.StatusCode} - {responseString.Substring(0, Math.Min(200, responseString.Length))}";
                    }
                }

                // Parse response
                var result = JsonDocument.Parse(responseString);
                
                if (!result.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                {
                    _logger.LogWarning("⚠️ Không nhận được candidates từ Gemini");
                    return "❌ Không nhận được phản hồi từ Gemini";
                }

                var firstCandidate = candidates[0];
                if (!firstCandidate.TryGetProperty("content", out var contentProp))
                {
                    _logger.LogWarning("⚠️ Không tìm thấy content trong response");
                    return "❌ Gemini không trả về nội dung";
                }

                if (!contentProp.TryGetProperty("parts", out var parts) || parts.GetArrayLength() == 0)
                {
                    _logger.LogWarning("⚠️ Không tìm thấy parts trong content");
                    return "❌ Không tìm thấy text trong response";
                }

                var text = parts[0].GetProperty("text").GetString();
                
                if (string.IsNullOrEmpty(text))
                {
                    _logger.LogWarning("⚠️ Text trống từ Gemini");
                    return "❌ Text trống";
                }

                _logger.LogInformation($"✅ Nhận phản hồi từ Gemini: {text.Substring(0, Math.Min(100, text.Length))}...");
                return text;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Exception: {ex.Message}");
                _logger.LogError($"   StackTrace: {ex.StackTrace}");
                return $"❌ Lỗi: {ex.Message}";
            }
        }
    }
}
