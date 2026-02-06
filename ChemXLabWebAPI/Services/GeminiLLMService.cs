using Application.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Gọi Gemini API với retry logic để xử lý rate limit
    /// </summary>
    public class GeminiLLMService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<GeminiLLMService> _logger;
        
        // Retry configuration
        private const int MaxRetries = 3;
        private const int InitialDelayMs = 1000;

        public GeminiLLMService(
            HttpClient httpClient, 
            IConfiguration configuration,
            ILogger<GeminiLLMService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["AiSettings:GeminiApiKey"] 
                ?? throw new Exception("❌ Gemini API Key không tìm thấy trong appsettings.json!");
            _model = configuration["AiSettings:GeminiModel"] ?? "gemini-2.5-flash";
            
            _logger.LogInformation($"✅ Gemini LLM Service initialized with model: {_model}");
        }

        /// <summary>
        /// Gọi Gemini để lấy phản hồi (với retry logic)
        /// </summary>
        public async Task<string> GetCompletionAsync(string prompt)
        {
            return await RetryAsync(() => GetCompletionInternalAsync(prompt), MaxRetries);
        }

        /// <summary>
        /// Logic gọi API (không có retry)
        /// </summary>
        private async Task<string> GetCompletionInternalAsync(string prompt)
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
                        
                        // Nếu là rate limit error, throw để retry
                        if (response.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            throw new HttpRequestException($"Rate limit exceeded: {errorMessage}", null, response.StatusCode);
                        }
                        
                        return $"❌ Lỗi Gemini API: {errorMessage} (HTTP {response.StatusCode})";
                    }
                    catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        throw;
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
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw; // Re-throw để retry handler xử lý
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Exception: {ex.Message}");
                _logger.LogError($"   StackTrace: {ex.StackTrace}");
                return $"❌ Lỗi: {ex.Message}";
            }
        }

        /// <summary>
        /// Retry logic với exponential backoff
        /// </summary>
        private async Task<string> RetryAsync(Func<Task<string>> operation, int maxRetries)
        {
            int delayMs = InitialDelayMs;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    _logger.LogInformation($"🔁 Attempt {attempt}/{maxRetries}");
                    return await operation();
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests && attempt < maxRetries)
                {
                    _logger.LogWarning($"⏸️ Rate limited! Chờ {delayMs}ms trước khi retry...");
                    await Task.Delay(delayMs);
                    delayMs *= 2;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Attempt {attempt} failed: {ex.Message}");
                    if (attempt == maxRetries)
                        throw;
                    
                    await Task.Delay(delayMs);
                    delayMs *= 2;
                }
            }
            
            throw new Exception("❌ Tất cả retry đều thất bại");
        }
    }
}