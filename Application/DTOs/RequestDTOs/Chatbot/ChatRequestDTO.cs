using System;

namespace Application.DTOs.RequestDTOs.Chatbot
{
    public class CreateSessionDTO
    {
        public string Topic { get; set; } = "Chemistry Chat";
    }

    public class SendMessageDTO
    {
        public Guid SessionId { get; set; }
        public string Message { get; set; } = null!;
    }
}
