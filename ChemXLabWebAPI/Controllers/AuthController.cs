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

        //[Authorize(Roles = "ADMIN")]
        //[HttpGet("Sepay")]
        //public async Task<IActionResult> GenerateSepayKey()
        //{
        //    string token = await _jwtService.GenerateSepayKeyAccess();
        //    return Ok(ApiResponse.Success("Generate sepay token successful", token.ToString()));
        //}
    }
}