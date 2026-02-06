using System;
using System.Collections.Generic;

namespace Application.DTOs.ChatbotModels
{
    /// <summary>
    /// Đại diện cho 1 cuộc trò chuyện (lưu trong memory, không vào DB)
    /// </summary>
    public class ConversationSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Topic { get; set; } = "Chemistry Chat";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Danh sách các tin nhắn trong cuộc trò chuyện
        /// </summary>
        public List<ChatMessage> Messages { get; set; } = new();
    }

    /// <summary>
    /// Một tin nhắn trong cuộc trò chuyện
    /// </summary>
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> ToolsUsed { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
