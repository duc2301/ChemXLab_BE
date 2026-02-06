using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Service for handling JSON Web Token (JWT) operations.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a new JWT token for the specified user.
        /// </summary>
        /// <returns>The generated token string.</returns>
        string GenerateToken(User user);
        Task<string> GenerateSepayKeyAccess();
    }
}