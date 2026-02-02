using Application.DTOs.RequestDTOs.Auth;
using Domain.Entities;

namespace Application.Interfaces.IRepositories
{
    /// <summary>
    /// Repository interface for managing Users.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Checks if an email address is already registered.
        /// </summary>
        /// <returns>True if the email exists, otherwise False.</returns>
        Task<bool> CheckEmailExist(string email);

        /// <summary>
        /// Validates user credentials for login.
        /// </summary>
        /// <returns>The user entity if credentials are valid, otherwise null.</returns>
        Task<User?> Login(LoginDTO request);

        /// <summary>
        /// Retrieves a user entity by their email address.
        /// </summary>
        /// <returns>The user entity if found, otherwise null.</returns>
        Task<User?> GetUserByEmail(string email);
    }
}