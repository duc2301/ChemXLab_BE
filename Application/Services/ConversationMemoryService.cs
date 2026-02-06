using Application.DTOs.ChatbotModels;
using Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services
{
    /// <summary>
    /// Lưu trữ cuộc trò chuyện trong memory (trong RAM)
    /// Không cần database, mất khi server restart
    /// </summary>
    public class ConversationMemoryService : IConversationMemoryService
    {
        /// <summary>
        /// Dictionary lưu tất cả conversations: SessionId -> ConversationSession
        /// </summary>
        private static readonly Dictionary<Guid, ConversationSession> _conversations = new();
        
        private static readonly object _lockObject = new();

        /// <summary>
        /// Tạo cuộc trò chuyện mới
        /// </summary>
        public ConversationSession CreateConversation(Guid userId, string topic = "Chemistry Chat")
        {
            lock (_lockObject)
            {
                var session = new ConversationSession
                {
                    UserId = userId,
                    Topic = topic,
                    CreatedAt = DateTime.UtcNow
                };

                _conversations[session.SessionId] = session;
                
                Console.WriteLine($"✅ Tạo session: {session.SessionId}");
                return session;
            }
        }

        /// <summary>
        /// Lấy cuộc trò chuyện
        /// </summary>
        public ConversationSession? GetConversation(Guid sessionId)
        {
            lock (_lockObject)
            {
                if (_conversations.TryGetValue(sessionId, out var session))
                    return session;
                
                Console.WriteLine($"⚠️ Session không tìm thấy: {sessionId}");
                return null;
            }
        }

        /// <summary>
        /// Thêm tin nhắn
        /// </summary>
        public void AddMessage(Guid sessionId, string role, string content, List<string>? toolsUsed = null)
        {
            lock (_lockObject)
            {
                if (!_conversations.TryGetValue(sessionId, out var session))
                {
                    Console.WriteLine($"⚠️ Session không tìm thấy: {sessionId}");
                    return;
                }

                var message = new ChatMessage
                {
                    Role = role,
                    Content = content,
                    ToolsUsed = toolsUsed ?? new(),
                    Timestamp = DateTime.UtcNow
                };

                session.Messages.Add(message);
                Console.WriteLine($"📝 Thêm tin nhắn từ {role}: {content.Substring(0, Math.Min(50, content.Length))}...");
            }
        }

        /// <summary>
        /// Lấy tất cả tin nhắn
        /// </summary>
        public List<ChatMessage>? GetMessages(Guid sessionId)
        {
            lock (_lockObject)
            {
                if (_conversations.TryGetValue(sessionId, out var session))
                    return session.Messages;
                
                return null;
            }
        }

        /// <summary>
        /// Xóa cuộc trò chuyện
        /// </summary>
        public bool DeleteConversation(Guid sessionId)
        {
            lock (_lockObject)
            {
                if (_conversations.Remove(sessionId))
                {
                    Console.WriteLine($"🗑️ Xóa session: {sessionId}");
                    return true;
                }
                
                return false;
            }
        }

        /// <summary>
        /// Lấy tất cả cuộc trò chuyện của user
        /// </summary>
        public List<ConversationSession> GetUserConversations(Guid userId)
        {
            lock (_lockObject)
            {
                return _conversations.Values
                    .Where(c => c.UserId == userId)
                    .ToList();
            }
        }

        /// <summary>
        /// Clear tất cả conversations của user
        /// </summary>
        public void ClearUserConversations(Guid userId)
        {
            lock (_lockObject)
            {
                var userSessions = _conversations.Values
                    .Where(c => c.UserId == userId)
                    .Select(c => c.SessionId)
                    .ToList();

                foreach (var sessionId in userSessions)
                {
                    _conversations.Remove(sessionId);
                }

                Console.WriteLine($"🗑️ Xóa {userSessions.Count} sessions của user {userId}");
            }
        }
    }
}
