using Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Công cụ hóa học - AI sẽ gọi Gemini để phân tích
    /// </summary>
    public class ChemistryToolkit : IChemistryToolkit
    {
        private readonly ILLMService _llmService;

        public ChemistryToolkit(ILLMService llmService)
        {
            _llmService = llmService;
        }

        /// <summary>
        /// Cân bằng phương trình (AI tự phân tích)
        /// </summary>
        public async Task<EquationBalanceResult> BalanceEquationAsync(string equation)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(equation))
                    return new EquationBalanceResult { Success = false, Message = "Phương trình trống" };

                var prompt = $@"Hãy cân bằng phương trình hóa học sau:
{equation}

Trả lời CHÍNH XÁC theo format:
PHƯƠNG TRÌNH ĐÃ CÂN BẰNG: [phương trình]
HỆ SỐ: [hệ số cách nhau bởi dấu phẩy]
GIẢI THÍCH: [giải thích]";

                var aiResponse = await _llmService.GetCompletionAsync(prompt);
                var result = ParseBalanceResponse(equation, aiResponse);
                
                return result;
            }
            catch (Exception ex)
            {
                return new EquationBalanceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        /// <summary>
        /// Phân tích phân tử (AI tự search)
        /// </summary>
        public async Task<MolecularAnalysisResult> AnalyzeMoleculeAsync(string formula)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(formula))
                    return new MolecularAnalysisResult { Formula = formula, CommonName = "Lỗi" };

                var prompt = $@"Phân tích chất hóa học: {formula}

Trả lời theo format:
TÊN GỌI: [tên tiếng Việt]
KHỐI LƯỢNG MO: [số, đơn vị g/mol]
TRẠNG THÁI: [Solid/Liquid/Gas]
MÀU: [màu]
ỨNG DỤNG: [ứng dụng chính]";

                var aiResponse = await _llmService.GetCompletionAsync(prompt);
                var result = ParseMolecularResponse(formula, aiResponse);
                
                return result;
            }
            catch (Exception ex)
            {
                return new MolecularAnalysisResult { Formula = formula, CommonName = $"Lỗi: {ex.Message}" };
            }
        }

        /// <summary>
        /// Kiểm tra phản ứng
        /// </summary>
        public async Task<ReactionCheckResult> CheckReactionFeasibilityAsync(string reactant1, string reactant2)
        {
            try
            {
                var prompt = $@"Hai chất {reactant1} và {reactant2} có phản ứng không?

Trả lời theo format:
CÓ PHẢN ỨNG: [Có/Không]
GIẢI THÍCH: [tại sao]
SẢN PHẨM: [sản phẩm nếu có, hoặc 'Không']";

                var aiResponse = await _llmService.GetCompletionAsync(prompt);
                var result = ParseReactionResponse(reactant1, reactant2, aiResponse);
                
                return result;
            }
            catch (Exception ex)
            {
                return new ReactionCheckResult { CanReact = false, Explanation = ex.Message };
            }
        }

        // ===== PARSE METHODS =====

        private EquationBalanceResult ParseBalanceResponse(string original, string aiResponse)
        {
            var result = new EquationBalanceResult { Success = false };

            try
            {
                if (aiResponse.Contains("PHƯƠNG TRÌNH ĐÃ CÂN BẰNG:"))
                {
                    var start = aiResponse.IndexOf("PHƯƠNG TRÌNH ĐÃ CÂN BẰNG:") + "PHƯƠNG TRÌNH ĐÃ CÂN BẰNG:".Length;
                    var end = aiResponse.IndexOf("\n", start);
                    if (end == -1) end = aiResponse.Length;
                    
                    result.BalancedEquation = aiResponse.Substring(start, end - start).Trim();
                    result.Success = true;
                }

                if (aiResponse.Contains("HỆ SỐ:"))
                {
                    var start = aiResponse.IndexOf("HỆ SỐ:") + "HỆ SỐ:".Length;
                    var end = aiResponse.IndexOf("\n", start);
                    if (end == -1) end = aiResponse.Length;
                    
                    var coefStr = aiResponse.Substring(start, end - start).Trim();
                    var coefficients = coefStr
                        .Split(',')
                        .Select(x => int.TryParse(x.Trim(), out var n) ? n : 0)
                        .ToList();
                    
                    result.Coefficients = coefficients;
                }

                result.Message = result.Success ? "Cân bằng thành công" : "Không thể cân bằng";
                return result;
            }
            catch
            {
                return new EquationBalanceResult { Success = false, Message = "Parse error", BalancedEquation = aiResponse };
            }
        }

        private MolecularAnalysisResult ParseMolecularResponse(string formula, string aiResponse)
        {
            var result = new MolecularAnalysisResult { Formula = formula };

            try
            {
                ExtractField(aiResponse, "TÊN GỌI:", out var name);
                result.CommonName = name;

                ExtractField(aiResponse, "KHỐI LƯỢNG MO:", out var mass);
                if (double.TryParse(Regex.Match(mass, @"\d+\.?\d*").Value, out var molarMass))
                    result.MolarMass = molarMass;

                ExtractField(aiResponse, "TRẠNG THÁI:", out var state);
                result.StateAtRoomTemp = state.Contains("Solid") || state.Contains("rắn") ? "Solid" :
                                         state.Contains("Liquid") || state.Contains("lỏng") ? "Liquid" :
                                         state.Contains("Gas") || state.Contains("khí") ? "Gas" : state;

                return result;
            }
            catch
            {
                return result;
            }
        }

        private ReactionCheckResult ParseReactionResponse(string r1, string r2, string aiResponse)
        {
            var result = new ReactionCheckResult();

            try
            {
                ExtractField(aiResponse, "CÓ PHẢN ỨNG:", out var canReact);
                result.CanReact = canReact.ToLower().Contains("có");

                ExtractField(aiResponse, "GIẢI THÍCH:", out var explain);
                result.Explanation = explain;

                ExtractField(aiResponse, "SẢN PHẨM:", out var product);
                if (!product.Contains("Không"))
                    result.PredictedProduct = product;

                return result;
            }
            catch
            {
                return result;
            }
        }

        private void ExtractField(string text, string fieldName, out string value)
        {
            value = string.Empty;
            if (!text.Contains(fieldName))
                return;

            var start = text.IndexOf(fieldName) + fieldName.Length;
            var end = text.IndexOf("\n", start);
            if (end == -1) end = text.Length;

            value = text.Substring(start, end - start).Trim();
        }
    }
}
