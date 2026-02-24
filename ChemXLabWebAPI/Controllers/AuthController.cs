using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.Auth;
using Application.Interfaces.IServices;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChemXLabWebAPI.Controllers
{
    /// <summary>
    /// Manages user authentication and account registration.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }


        /// <summary>
        /// Authenticates a user and generates an access token.
        /// </summary>
        /// <returns>The JWT token and user information upon successful login.</returns>
        /// <response code="200">Login successful.</response>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authService.Login(loginDTO);
            return Ok(ApiResponse.Success("Login successful", result));
        }

        /// <summary>
        /// Registers a new user account in the system.
        /// </summary>
        /// <returns>The created user profile details.</returns>
        /// <response code="200">Registration successful.</response>
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var result = await _authService.Register(registerDTO);
            return Ok(ApiResponse.Success("Registration successful", result));
        }

        // POST: api/Auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            try
            {
                await _authService.SendOtpAsync(request.Email);
                return Ok(ApiResponse.Success("OTP sent to email", null));
            }
            catch (Exception ex) { return BadRequest(ApiResponse.Fail(ex.Message)); }
        }

        // POST: api/Auth/send-otp (Sends OTP even if user does not exist)
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtpToAnyEmail([FromBody] ForgotPasswordRequestDTO request)
        {
            try
            {
                await _authService.SendOtpToAnyEmailAsync(request.Email);
                return Ok(ApiResponse.Success("OTP sent to email", null));
            }
            catch (Exception ex) { return BadRequest(ApiResponse.Fail(ex.Message)); }
        }

        // POST: api/Auth/verify-otp (Dùng để check trước khi hiện form đổi pass)
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDTO request)
        {
            var isValid = await _authService.VerifyOtpAsync(request.Email, request.OtpCode);
            if (isValid) return Ok(ApiResponse.Success("OTP Valid", true));
            return BadRequest(ApiResponse.Fail("Invalid OTP"));
        }

        // POST: api/Auth/reset-password (Đổi pass sau khi có OTP)
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
        {
            try
            {
                await _authService.ResetPasswordAsync(request);
                return Ok(ApiResponse.Success("Password reset successfully. Please login.", null));
            }
            catch (Exception ex) { return BadRequest(ApiResponse.Fail(ex.Message)); }
        }

        // POST: api/Auth/GoogleLogin
        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO request)
        {
            try
            {
                var token = await _authService.GoogleLoginAsync(request.IdToken);
                return Ok(ApiResponse.Success("Login successful", token));
            }
            catch (Exception ex) { return BadRequest(ApiResponse.Fail(ex.Message)); }
        }
    }
}