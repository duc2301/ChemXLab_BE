using Application.DTOs.RequestDTOs.Auth;
using Application.DTOs.ResponseDTOs.User;
using Application.ExceptionMidleware;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    /// <summary>
    /// Service responsible for handling user authentication and registration logic.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _mapper = mapper;
            _redisService = redisService;
        }

        /// <summary>
        /// Authenticates a user based on email and password.
        /// </summary>
        /// <param name="request">The login credentials (email and password).</param>
        /// <returns>A JWT string if authentication is successful.</returns>
        /// <exception cref="ApiExceptionResponse">Thrown when the email or password is incorrect.</exception>
        public async Task<string> Login(LoginDTO loginDTO)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmail(loginDTO.Email);
            if (user == null) throw new Exception("Invalid email or password");

            // CHECK HASH
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash);
            if (!isPasswordValid) throw new Exception("Invalid email or password");

            return _jwtService.GenerateToken(user);
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="request">The user registration details.</param>
        /// <returns>The profile information of the newly created user.</returns>
        /// <exception cref="ApiExceptionResponse">Thrown when the provided email address is already in use.</exception>
        public async Task<UserResponseDTO> Register(RegisterDTO registerDTO)
        {
            if (await _unitOfWork.UserRepository.CheckEmailExist(registerDTO.Email))
                throw new Exception("Email already exists");

            var user = _mapper.Map<User>(registerDTO);
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            user.Role = "STUDENT";

            // HASH PASSWORD
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.PasswordHash);

            await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task SendOtpAsync(string email)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmail(email);
            if (user == null) throw new Exception("User not found");

            string otp = new Random().Next(100000, 999999).ToString();

            await _redisService.SetAsync($"OTP_{email}", otp, TimeSpan.FromMinutes(5));

            Console.WriteLine($"[MOCK EMAIL] OTP for {email} is: {otp}");
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var storedOtp = await _redisService.GetAsync<string>($"OTP_{email}");
            if (string.IsNullOrEmpty(storedOtp) || storedOtp != otp)
                return false;
            return true;
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO request)
        {
            var isValid = await VerifyOtpAsync(request.Email, request.OtpCode);
            if (!isValid) throw new Exception("Invalid or expired OTP");

            var user = await _unitOfWork.UserRepository.GetUserByEmail(request.Email);
            if (user == null) throw new Exception("User not found");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            await _redisService.RemoveAsync($"OTP_{request.Email}");
        }
    }
}