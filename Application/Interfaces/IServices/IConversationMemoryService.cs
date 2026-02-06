using Application.DTOs.ChatbotModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Quản lý lịch sử cuộc trò chuyện trong bộ nhớ (in-memory)
    /// Không lưu vào database, mất khi reload trang
    /// </summary>
    public interface IConversationMemoryService
    {
        /// <summary>
        /// Tạo cuộc trò chuyện mới
        /// </summary>
        ConversationSession CreateConversation(Guid userId, string topic = "Chemistry Chat");

        /// <summary>
        /// Lấy cuộc trò chuyện theo ID
        /// </summary>
        ConversationSession? GetConversation(Guid sessionId);

        /// <summary>
        /// Thêm tin nhắn vào cuộc trò chuyện
        /// </summary>
        void AddMessage(Guid sessionId, string role, string content, List<string>? toolsUsed = null);

        /// <summary>
        /// Lấy tất cả tin nhắn của cuộc trò chuyện
        /// </summary>
        List<ChatMessage>? GetMessages(Guid sessionId);

        /// <summary>
        /// Xóa cuộc trò chuyện khỏi memory
        /// </summary>
        bool DeleteConversation(Guid sessionId);

        /// <summary>
        /// Lấy tất cả cuộc trò chuyện của user
        /// </summary>
        List<ConversationSession> GetUserConversations(Guid userId);

        /// <summary>
        /// Clear tất cả conversations của user
        /// </summary>
        void ClearUserConversations(Guid userId);
    }
}
