using System;
using System.Collections.Generic;

namespace Application.DTOs.ResponseDTOs.Chatbot
{
    public class SessionDTO
    {
        public Guid SessionId { get; set; }
        public string Topic { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int MessageCount { get; set; }
    }

    public class ChatResponseDTO
    {
        public string Response { get; set; } = string.Empty;
        public List<string> ToolsUsed { get; set; } = new();
        public Dictionary<string, object>? AnalysisData { get; set; }
    }

    public class ConversationHistoryDTO
    {
        public Guid SessionId { get; set; }
        public string Topic { get; set; } = string.Empty;
        public List<MessageHistoryDTO> Messages { get; set; } = new();
    }

    public class MessageHistoryDTO
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> ToolsUsed { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }
}
