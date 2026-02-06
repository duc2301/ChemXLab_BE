using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Defines the contract for an email service capable of sending emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously to a specified recipient.
        /// </summary>
        /// <param name="toEmail">The email address of the recipient.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="message">The body content of the email (usually HTML).</param>
        /// <returns>A task that represents the asynchronous send operation.</returns>
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}