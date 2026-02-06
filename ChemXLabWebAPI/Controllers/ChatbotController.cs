using Application.DTOs.ApiResponseDTO;
using Application.DTOs.ChatbotModels;
using Application.DTOs.RequestDTOs.Chatbot;
using Application.DTOs.ResponseDTOs.Chatbot;
using Application.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChemXLabWebAPI.Controllers
{
    [Route("api/chatbot")]
    [ApiController]
    [AllowAnonymous]
    public class ChatbotController : ControllerBase
    {
        private readonly IAIChemistryAgent _agent;
        private readonly IConversationMemoryService _memoryService;
        private readonly IMapper _mapper;
        private readonly ILLMService _llmService;

        public ChatbotController(
            IAIChemistryAgent agent,
            IConversationMemoryService memoryService,
            IMapper mapper,
            ILLMService llmService)
        {
            _agent = agent;
            _memoryService = memoryService;
            _mapper = mapper;
            _llmService = llmService;
        }

        /// <summary>
        /// Tạo cuộc trò chuyện mới (tạo session)
        /// </summary>
        [HttpPost("sessions")]
        public IActionResult CreateSession([FromBody] CreateSessionDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var session = _memoryService.CreateConversation(userId, dto.Topic);
                
                var result = new SessionDTO
                {
                    SessionId = session.SessionId,
                    Topic = session.Topic,
                    CreatedAt = session.CreatedAt,
                    MessageCount = 0
                };

                return Ok(ApiResponse.Success("✅ Tạo cuộc trò chuyện thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail($"❌ Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gửi câu hỏi và nhận phản hồi từ AI
        /// </summary>
        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] SendMessageDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Message))
                    return BadRequest(ApiResponse.Fail("❌ Câu hỏi không được để trống"));

                // Lấy session
                var session = _memoryService.GetConversation(dto.SessionId);
                if (session == null)
                    return NotFound(ApiResponse.Fail("❌ Session không tìm thấy"));

                // Xử lý câu hỏi bằng AI Agent
                var response = await _agent.ProcessQuestionAsync(dto.SessionId, dto.Message);

                var resultDTO = _mapper.Map<ChatResponseDTO>(response);

                return Ok(ApiResponse.Success("✅ Xử lý thành công", resultDTO));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail($"❌ Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy lịch sử cuộc trò chuyện
        /// </summary>
        [HttpGet("sessions/{sessionId}/history")]
        public IActionResult GetConversationHistory(Guid sessionId)
        {
            try
            {
                var session = _memoryService.GetConversation(sessionId);
                if (session == null)
                    return NotFound(ApiResponse.Fail("❌ Session không tìm thấy"));

                var messages = _memoryService.GetMessages(sessionId);
                
                var historyDTO = new ConversationHistoryDTO
                {
                    SessionId = sessionId,
                    Topic = session.Topic,
                    Messages = messages?.Select(m => new MessageHistoryDTO
                    {
                        Role = m.Role,
                        Content = m.Content,
                        ToolsUsed = m.ToolsUsed,
                        Timestamp = m.Timestamp
                    }).ToList() ?? new()
                };

                return Ok(ApiResponse.Success("✅ Lấy lịch sử thành công", historyDTO));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail($"❌ Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy tất cả cuộc trò chuyện của user
        /// </summary>
        [HttpGet("sessions")]
        public IActionResult GetUserSessions()
        {
            try
            {
                var userId = GetUserId();
                var sessions = _memoryService.GetUserConversations(userId);

                var sessionDTOs = sessions.Select(s => new SessionDTO
                {
                    SessionId = s.SessionId,
                    Topic = s.Topic,
                    CreatedAt = s.CreatedAt,
                    MessageCount = s.Messages.Count
                }).ToList();

                return Ok(ApiResponse.Success("✅ Lấy danh sách cuộc trò chuyện thành công", sessionDTOs));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail($"❌ Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa cuộc trò chuyện
        /// </summary>
        [HttpDelete("sessions/{sessionId}")]
        public IActionResult DeleteSession(Guid sessionId)
        {
            try
            {
                var success = _memoryService.DeleteConversation(sessionId);
                if (!success)
                    return NotFound(ApiResponse.Fail("❌ Session không tìm thấy"));

                return Ok(ApiResponse.Success("✅ Xóa cuộc trò chuyện thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail($"❌ Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa tất cả cuộc trò chuyện của user
        /// </summary>
        [HttpDelete("sessions/clear-all")]
        public IActionResult ClearAllSessions()
        {
            try
            {
                var userId = GetUserId();
                _memoryService.ClearUserConversations(userId);
                
                return Ok(ApiResponse.Success("✅ Xóa tất cả cuộc trò chuyện thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail($"❌ Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Kiểm tra cấu hình API (chỉ dùng để gỡ lỗi)
        /// </summary>
        [HttpGet("debug/config")]
        [AllowAnonymous]
        public IActionResult CheckConfig([FromServices] IConfiguration configuration)
        {
            var apiKey = configuration["AiSettings:GeminiApiKey"] ?? "NOT SET";
            var model = configuration["AiSettings:GeminiModel"] ?? "NOT SET";

            return Ok(new
            {
                apiKeySet = apiKey != "NOT SET",
                apiKeyLength = apiKey.Length,
                apiKeyPreview = apiKey == "NOT SET" ? "NOT SET" : (apiKey.Substring(0, Math.Min(20, apiKey.Length)) + "***"),
                model = model,
                fullApiKey = apiKey  // ⚠️ Chỉ dùng để debug, không push lên production!
            });
        }

        /// <summary>
        /// Test kết nối và phản hồi của Gemini API (chỉ dùng để gỡ lỗi)
        /// </summary>
        [HttpGet("debug/test-gemini")]
        [AllowAnonymous]
        public async Task<IActionResult> TestGemini()
        {
            var response = await _llmService.GetCompletionAsync("Say 'Hello World'");
            return Ok(new { response });
        }

        [HttpGet("debug/list-models")]
        [AllowAnonymous]
        public async Task<IActionResult> ListAvailableModels([FromServices] IConfiguration configuration)
        {
            try
            {
                var apiKey = configuration["AiSettings:GeminiApiKey"];
                var url = $"https://generativelanguage.googleapis.com/v1beta/models?key={apiKey}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var doc = JsonDocument.Parse(responseString);
                        return Ok(doc.RootElement);
                    }
                    else
                    {
                        return BadRequest(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ===== HELPER =====

        private Guid GetUserId()
        {
            // Thử tìm "UserId" claim trước (custom claim)
            var userIdClaim = User.FindFirst("UserId")?.Value 
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("❌ Không tìm thấy UserId trong token");

            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            throw new Exception($"❌ UserId không hợp lệ: {userIdClaim}");
        }
    }
}
