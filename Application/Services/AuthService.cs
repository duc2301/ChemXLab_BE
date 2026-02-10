using Application.DTOs.RequestDTOs.Auth;
using Application.DTOs.ResponseDTOs.User;
using Application.ExceptionMidleware;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

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
        private readonly IEmailService _emailService;

        // Inject services into Constructor
        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IMapper mapper,
            IRedisService redisService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _mapper = mapper;
            _redisService = redisService;
            _emailService = emailService;
        }

        /// <summary>
        /// Authenticates a user based on email and password.
        /// </summary>
        /// <param name="loginDTO">The login credentials (email and password).</param>
        /// <returns>A JWT string if authentication is successful.</returns>
        /// <exception cref="ApiExceptionResponse">Thrown when the email or password is incorrect.</exception>
        public async Task<string> Login(LoginDTO loginDTO)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmail(loginDTO.Email);
            if (user == null) throw new ApiExceptionResponse("Invalid email or password", 401);

            // CHECK HASH
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash);
            if (!isPasswordValid) throw new ApiExceptionResponse("Invalid email or password", 401);

            return _jwtService.GenerateToken(user);
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="registerDTO">The user registration details.</param>
        /// <returns>The profile information of the newly created user.</returns>
        /// <exception cref="ApiExceptionResponse">Thrown when the provided email address is already in use.</exception>
        public async Task<UserResponseDTO> Register(RegisterDTO registerDTO)
        {
            if (await _unitOfWork.UserRepository.CheckEmailExist(registerDTO.Email))
                throw new ApiExceptionResponse("Email already exists");

            var user = _mapper.Map<User>(registerDTO);
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            user.Role = "STUDENT";

            // HASH PASSWORD
            // Lưu ý: Đảm bảo field này khớp với DTO (Password hoặc PasswordHash)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.PasswordHash);

            await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }

        /// <summary>
        /// Generates an OTP and sends it to the user's email for password recovery.
        /// </summary>
        /// <param name="email">The email address of the user requesting the OTP.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ApiExceptionResponse">Thrown if the user is not found.</exception>
        public async Task SendOtpAsync(string email)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmail(email);
            if (user == null) throw new ApiExceptionResponse("User not found");

            string otp = new Random().Next(100000, 999999).ToString();

            // Save OTP to Redis with a 5-minute expiration
            await _redisService.SetAsync($"OTP_{email}", otp, TimeSpan.FromMinutes(5));

            string subject = "ChemXLab - Mã xác thực đặt lại mật khẩu";
            string body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #2c3e50;'>Yêu cầu đặt lại mật khẩu</h2>
                    <p>Xin chào <b>{user.FullName}</b>,</p>
                    <p>Mã xác thực (OTP) của bạn là:</p>
                    <h1 style='color: #e74c3c; letter-spacing: 5px;'>{otp}</h1>
                    <p>Mã này sẽ hết hạn sau <b>5 phút</b>.</p>
                </div>";

            await _emailService.SendEmailAsync(email, subject, body);
        }

        /// <summary>
        /// Verifies if the provided OTP matches the one stored in the system.
        /// </summary>
        /// <param name="email">The email address associated with the OTP.</param>
        /// <param name="otp">The OTP code to verify.</param>
        /// <returns>True if the OTP is valid; otherwise, false.</returns>
        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var storedOtp = await _redisService.GetAsync<string>($"OTP_{email}");
            if (string.IsNullOrEmpty(storedOtp) || storedOtp != otp)
                return false;
            return true;
        }

        /// <summary>
        /// Resets the user's password after successful OTP verification.
        /// </summary>
        /// <param name="request">The object containing the email, OTP code, and the new password.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ApiExceptionResponse">Thrown if the OTP is invalid/expired or the user is not found.</exception>
        public async Task ResetPasswordAsync(ResetPasswordDTO request)
        {
            var isValid = await VerifyOtpAsync(request.Email, request.OtpCode);
            if (!isValid) throw new ApiExceptionResponse("Invalid or expired OTP");

            var user = await _unitOfWork.UserRepository.GetUserByEmail(request.Email);
            if (user == null) throw new ApiExceptionResponse("User not found");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate the OTP after use
            await _redisService.RemoveAsync($"OTP_{request.Email}");
        }
    }
}