using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDTOs.User
{
    public class UpdateUserDTO
    {
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string? FullName { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        public string? AvatarUrl { get; set; }
    }
}
