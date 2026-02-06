using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDTOs.Auth
{
    public class ForgotPasswordRequestDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
    }

    public class VerifyOtpDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string OtpCode { get; set; } = null!;
    }

    public class ResetPasswordDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string OtpCode { get; set; } = null!; 
        [Required, MinLength(6)]
        public string NewPassword { get; set; } = null!;
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
