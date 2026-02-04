using Application.DTOs.ApiResponseDTO;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChemXLabWebAPI.Controllers
{
    /// <summary>
    /// Manages user accounts and profile information.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a list of all users registered in the system.
        /// </summary>
        /// <returns>A list of user profiles.</returns>
        /// <response code="200">Request successful, returns the list of users.</response>
        [Authorize]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse.Success("Get All success", users));
        }
    }
}