using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDTOs.User
{
    /// <summary>
    /// Data Transfer Object representing user profile information.
    /// </summary>
    public class UserResponseDTO
    {
        /// <summary>
        /// The unique identifier of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// The hashed password of the user.
        /// </summary>
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// The user's full name.
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// The role assigned to the user within the system.
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// The URL of the user's avatar image.
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// The date and time when the user account was created.
        /// </summary>
        public DateTime? CreatedAt { get; set; }
    }
}