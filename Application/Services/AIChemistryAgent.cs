using Application.DTOs.ChatbotModels;
using Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// AI Agent xử lý câu hỏi hóa học
    /// FLOW: Phân loại → Chọn tool → Chạy tool → Gọi Gemini → Trả về
    /// </summary>
    public class AIChemistryAgent : IAIChemistryAgent
    {
        private readonly IChemistryToolkit _toolkit;
        private readonly IConversationMemoryService _memoryService;
        private readonly ILLMService _llmService;

        public AIChemistryAgent(
            IChemistryToolkit toolkit,
            IConversationMemoryService memoryService,
            ILLMService llmService)
        {
            _toolkit = toolkit;
            _memoryService = memoryService;
            _llmService = llmService;
        }

        /// <summary>
        /// Xử lý câu hỏi của user
        /// </summary>
        public async Task<ChemistryAgentResponse> ProcessQuestionAsync(Guid sessionId, string userQuestion)
        {
            try
            {
                // LƯU câu hỏi user vào memory
                _memoryService.AddMessage(sessionId, "user", userQuestion);

                // BƯỚC 1: Phân loại câu hỏi
                var intent = ClassifyQuestion(userQuestion);
                Console.WriteLine($"📌 Intent: {intent}");

                // BƯỚC 2: Chọn tools
                var selectedTools = SelectTools(userQuestion, intent);
                Console.WriteLine($"🔧 Tools: {string.Join(", ", selectedTools)}");

                // BƯỚC 3: Chạy tools
                var toolResults = await ExecuteToolsAsync(selectedTools, userQuestion);

                // BƯỚC 4: Lấy lịch sử cuộc trò chuyện
                var messages = _memoryService.GetMessages(sessionId);
                var conversationContext = BuildConversationContext(messages);

                // BƯỚC 5: Gọi Gemini để sinh phản hồi
                var prompt = BuildPrompt(userQuestion, intent, toolResults, conversationContext);
                var aiResponse = await _llmService.GetCompletionAsync(prompt);

                // BƯỚC 6: Tạo response
                var response = new ChemistryAgentResponse
                {
                    Response = aiResponse,
                    ToolsUsed = selectedTools,
                    AnalysisData = toolResults
                };

                // LƯU phản hồi vào memory
                _memoryService.AddMessage(sessionId, "assistant", aiResponse, selectedTools);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                return new ChemistryAgentResponse
                {
                    Response = $"❌ Lỗi: {ex.Message}",
                    ToolsUsed = new(),
                    AnalysisData = null
                };
            }
        }

        /// <summary>
        /// Bước 1: Phân loại câu hỏi
        /// </summary>
        private string ClassifyQuestion(string question)
        {
            var q = question.ToLower();
            
            if (q.Contains("cân bằng") || q.Contains("balancing") || q.Contains("→") || q.Contains("->"))
                return "equation_balancing";
            
            if (q.Contains("phân tích") || q.Contains("analyze") || q.Contains("tính chất"))
                return "analysis";
            
            if (q.Contains("phản ứng") || q.Contains("reaction"))
                return "reaction";
            
            if (q.Contains("giải") || q.Contains("tính") || q.Contains("solve"))
                return "problem_solving";
            
            return "general_chemistry";
        }

        /// <summary>
        /// Bước 2: Chọn tools phù hợp
        /// </summary>
        private List<string> SelectTools(string question, string intent)
        {
            var tools = new List<string>();

            if (question.Contains("→") || question.Contains("->") || intent == "equation_balancing")
                tools.Add("EquationBalancer");

            if (ContainsChemicalFormula(question))
                tools.Add("MolecularAnalyzer");

            if (intent == "reaction" || intent == "analysis")
                tools.Add("ReactionChecker");

            // Nếu không chọn tool nào, dùng AI để trả lời chung chung
            if (tools.Count == 0)
                tools.Add("GeneralAI");

            return tools;
        }

        /// <summary>
        /// Bước 3: Chạy tools
        /// </summary>
        private async Task<Dictionary<string, object>> ExecuteToolsAsync(List<string> tools, string question)
        {
            var results = new Dictionary<string, object>();

            foreach (var tool in tools)
            {
                try
                {
                    if (tool == "EquationBalancer")
                    {
                        var equation = ExtractEquation(question);
                        if (!string.IsNullOrEmpty(equation))
                        {
                            var balanceResult = await _toolkit.BalanceEquationAsync(equation);
                            results["equation_balancing"] = new
                            {
                                original = equation,
                                balanced = balanceResult.BalancedEquation,
                                coefficients = balanceResult.Coefficients,
                                success = balanceResult.Success
                            };
                            Console.WriteLine("✅ EquationBalancer done");
                        }
                    }
                    else if (tool == "MolecularAnalyzer")
                    {
                        var formulas = ExtractChemicalFormulas(question);
                        var data = new List<object>();
                        
                        foreach (var formula in formulas)
                        {
                            var analysis = await _toolkit.AnalyzeMoleculeAsync(formula);
                            data.Add(new
                            {
                                formula = analysis.Formula,
                                name = analysis.CommonName,
                                molarMass = analysis.MolarMass,
                                state = analysis.StateAtRoomTemp
                            });
                        }
                        
                        results["molecular_analysis"] = data;
                        Console.WriteLine("✅ MolecularAnalyzer done");
                    }
                    else if (tool == "ReactionChecker")
                    {
                        var (r1, r2) = ExtractReactants(question);
                        if (!string.IsNullOrEmpty(r1) && !string.IsNullOrEmpty(r2))
                        {
                            var checkResult = await _toolkit.CheckReactionFeasibilityAsync(r1, r2);
                            results["reaction_check"] = new
                            {
                                canReact = checkResult.CanReact,
                                explanation = checkResult.Explanation,
                                product = checkResult.PredictedProduct
                            };
                            Console.WriteLine("✅ ReactionChecker done");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Tool {tool} error: {ex.Message}");
                    results[$"{tool}_error"] = ex.Message;
                }
            }

            return results;
        }

        /// <summary>
        /// Xây dựng context từ lịch sử cuộc trò chuyện
        /// </summary>
        private string BuildConversationContext(List<ChatMessage>? messages)
        {
            if (messages == null || messages.Count == 0)
                return "Đây là câu hỏi đầu tiên trong cuộc trò chuyện.";

            var recentMessages = messages.TakeLast(4).ToList(); // Lấy 4 tin nhắn gần nhất
            var context = "Lịch sử cuộc trò chuyện gần đây:\n";
            
            foreach (var msg in recentMessages)
            {
                var role = msg.Role == "user" ? "Học sinh" : "AI";
                context += $"{role}: {msg.Content.Substring(0, Math.Min(100, msg.Content.Length))}\n";
            }

            return context;
        }

        /// <summary>
        /// Xây dựng prompt cho Gemini
        /// </summary>
        private string BuildPrompt(
            string question, 
            string intent, 
            Dictionary<string, object> toolResults,
            string conversationContext)
        {
            var toolData = toolResults.Any() 
                ? JsonSerializer.Serialize(toolResults, new JsonSerializerOptions { WriteIndented = true })
                : "Không có dữ liệu từ tools";

            var prompt = $@"Bạn là giáo viên hóa học tài giỏi, nhiệt tình, giảng dạy cho học sinh.

THÔNG TIN CÓ SẴN:
- Loại câu hỏi: {intent}
- Dữ liệu phân tích từ tools:
{toolData}

{conversationContext}

CÂU HỎI HIỆN TẠI:
{question}

YÊUCẦU:
1. Trả lời chính xác và đầy đủ
2. Giải thích chi tiết, dễ hiểu
3. Nếu là bài toán, hướng dẫn từng bước
4. Dùng tiếng Việt, tránh jargon phức tạp
5. Thêm ví dụ nếu cần

TRẢLỜI:";

            return prompt;
        }

        // ===== HELPER METHODS =====

        private bool ContainsChemicalFormula(string text)
        {
            return Regex.IsMatch(text, @"[A-Z][a-z]?\d*");
        }

        private string ExtractEquation(string text)
        {
            var parts = text.Split(new[] { "→", "->" }, StringSplitOptions.None);
            return parts.Length == 2 ? text : string.Empty;
        }

        private List<string> ExtractChemicalFormulas(string text)
        {
            var formulas = new List<string>();
            var matches = Regex.Matches(text, @"[A-Z][a-z]?\d*");
            
            foreach (Match match in matches)
            {
                if (!formulas.Contains(match.Value))
                    formulas.Add(match.Value);
            }
            
            return formulas;
        }

        private (string, string) ExtractReactants(string text)
        {
            var formulas = ExtractChemicalFormulas(text);
            return formulas.Count >= 2 ? (formulas[0], formulas[1]) : (string.Empty, string.Empty);
        }
    }
}
