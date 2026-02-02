using Application.DTOs.RequestDTOs.Auth;
using Application.DTOs.ResponseDTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Defines services for user authentication and registration.
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
    }
}