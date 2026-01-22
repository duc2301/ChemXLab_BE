using Application.DTOs.RequestDTOs.Auth;
using Application.DTOs.ResponseDTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<string> Login(LoginDTO request);
        Task<UserResponseDTO> Register(RegisterDTO request);
    }
}
