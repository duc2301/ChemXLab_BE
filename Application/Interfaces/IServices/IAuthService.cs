using Application.DTOs.RequestDTOs.Auth;
using Application.DTOs.ResponseDTOs.User;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Defines services for user authentication, registration, and password recovery.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user and generates an access token.
        /// </summary>
        /// <returns>A JWT token string if successful.</returns>
        Task<string> Login(LoginDTO request);

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <returns>The details of the registered user.</returns>
        Task<UserResponseDTO> Register(RegisterDTO request);

        /// <summary>
        /// Sends an OTP to the user's email for password recovery.
        /// </summary>
        Task SendOtpAsync(string email);

        /// <summary>
        /// Verifies if the provided OTP is valid for the given email.
        /// </summary>
        Task<bool> VerifyOtpAsync(string email, string otp);

        /// <summary>
        /// Resets the user's password using a valid OTP.
        /// </summary>
        Task ResetPasswordAsync(ResetPasswordDTO request);
    }
}