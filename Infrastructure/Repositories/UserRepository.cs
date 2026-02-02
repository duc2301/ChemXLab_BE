using Application.DTOs.RequestDTOs.Auth;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing User entities.
    /// </summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ChemXlabContext _context;

        public UserRepository(ChemXlabContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if an email address is already associated with an existing user.
        /// </summary>
        /// <returns>True if the email exists, otherwise False.</returns>
        public async Task<bool> CheckEmailExist(string email)
        {
            var user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            return user != null;
        }

        /// <summary>
        /// Retrieves a user matching the provided email and password.
        /// </summary>
        /// <returns>The user entity if credentials match, otherwise null.</returns>
        public async Task<User?> Login(LoginDTO request)
        {
            return await _context.Users.Where(u => u.Email == request.Email && u.PasswordHash == request.Password).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a user entity based on their email address.
        /// </summary>
        /// <returns>The user entity if found, otherwise null.</returns>
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }
    }
}