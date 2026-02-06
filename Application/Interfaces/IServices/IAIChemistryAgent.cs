using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// AI Agent xử lý câu hỏi hóa học
    /// </summary>
    public interface IAIChemistryAgent
    {
        /// <summary>
        /// Xử lý câu hỏi của user
        /// </summary>
        Task<ChemistryAgentResponse> ProcessQuestionAsync(Guid sessionId, string userQuestion);
    }

    public class ChemistryAgentResponse
    {
        public string Response { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public List<string> ToolsUsed { get; set; } = new();
        public Dictionary<string, object>? AnalysisData { get; set; }
    }
}
