using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.User;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChemXLabWebAPI.Controllers
{
    /// <summary>
    /// Manages user accounts, profiles, and administrative actions.
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
        /// Retrieves the profile information of the currently logged-in user.
        /// </summary>
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier);
                var user = await _userService.GetUserByIdAsync(Guid.Parse(userId.Value));
                return Ok(ApiResponse.Success("Get profile success", user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Updates the current user's profile (FullName, AvatarUrl only).
        /// </summary>
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDTO request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier);
                var updatedUser = await _userService.UpdateUserAsync(Guid.Parse(userId.Value), request);
                return Ok(ApiResponse.Success("Profile updated successfully", updatedUser));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Changes the current user's password. Requires old password verification.
        /// </summary>
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier);
                await _userService.ChangePasswordAsync(Guid.Parse(userId.Value), request);
                return Ok(ApiResponse.Success("Password changed successfully", null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a list of all users. (Admin Only)
        /// </summary>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse.Success("Get all users success", users));
        }

        /// <summary>
        /// Retrieves a specific user by ID. (Admin Only)
        /// </summary>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(ApiResponse.Success("Get user detail success", user));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail("User not found"));
            }
        }

        /// <summary>
        /// Creates a new user manually. (Admin Only)
        /// </summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var newUser = await _userService.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, ApiResponse.Success("User created successfully", newUser));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Deletes a user permanently. (Admin Only)
        /// </summary>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(ApiResponse.Success("User deleted successfully", null));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail("User not found"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }
        
    }
}