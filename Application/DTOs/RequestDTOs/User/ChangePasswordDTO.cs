using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDTOs.User
{
    public class ChangePasswordDTO
    {
        [Required]
        public string OldPassword { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
        public string NewPassword { get; set; } = null!;

        [Compare("NewPassword", ErrorMessage = "Confirm password does not match")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
