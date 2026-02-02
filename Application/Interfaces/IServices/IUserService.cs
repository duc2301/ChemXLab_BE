using Application.DTOs.ResponseDTOs.User;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// <returns>A collection of user profiles.</returns>
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
    }
}