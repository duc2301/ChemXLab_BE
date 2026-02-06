using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Gọi Gemini AI để xử lý câu hỏi
    /// </summary>
    public interface ILLMService
    {
        Task<string> GetCompletionAsync(string prompt);
    }
}
