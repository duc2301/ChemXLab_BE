using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDTOs.User
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string? FullName { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? AvatarUrl { get; set; }

        public string? Role { get; set; } = "Student"; 
    }
}
