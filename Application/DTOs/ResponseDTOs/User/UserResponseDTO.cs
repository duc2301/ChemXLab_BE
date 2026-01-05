using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDTOs.User
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? FullName { get; set; }

        public string? AvatarUrl { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
