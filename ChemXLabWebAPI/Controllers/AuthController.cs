using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.Auth;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ChemXLabWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authService.Login(loginDTO);
            return Ok(ApiResponse.Success("Login successful", result));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var result = await _authService.Register(registerDTO);
            return Ok(ApiResponse.Success("Registration successful", result));
        }
    }
}
