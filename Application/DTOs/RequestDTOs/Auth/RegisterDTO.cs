using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for user registration requests.
    /// </summary>
    public class RegisterDTO
    {
        /// <summary>
        /// The user's full name.
        /// </summary>
        [MinLength(3, ErrorMessage = "Full name must be at least 3 characters long.")]
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        /// <summary>
        /// The password for the new account.
        /// </summary>
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [Required(ErrorMessage = "Password is required.")]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Confirmation field to ensure the password was typed correctly.
        /// </summary>
        [Compare("PasswordHash", ErrorMessage = "Passwords do not match.")]
        [Required(ErrorMessage = "Confirm Password is required.")]
        public string ConfirmPassword { get; set; }
    }
}