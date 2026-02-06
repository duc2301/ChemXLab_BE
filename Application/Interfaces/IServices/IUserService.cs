using Application.DTOs.RequestDTOs.User;
using Application.DTOs.ResponseDTOs.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Service for managing user accounts and profiles.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a list of all users in the system.
        /// </summary>
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();

        /// <summary>
        /// Retrieves specific user details by ID.
        /// </summary>
        Task<UserResponseDTO> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Creates a new user (Admin function).
        /// </summary>
        Task<UserResponseDTO> CreateUserAsync(CreateUserDTO request);

        /// <summary>
        /// Updates user profile information (FullName, Avatar).
        /// </summary>
        Task<UserResponseDTO> UpdateUserAsync(Guid id, UpdateUserDTO request);

        /// <summary>
        /// Changes the user's password (requires old password).
        /// </summary>
        Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDTO request);

        /// <summary>
        /// Deletes a user permanently.
        /// </summary>
        Task<bool> DeleteUserAsync(Guid id);
    }
}