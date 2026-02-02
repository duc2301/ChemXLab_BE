using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDTOs.Auth
{
    /// <summary>
    /// Data Transfer Object containing user credentials for login.
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// The email address associated with the user account.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// The password for authentication.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}