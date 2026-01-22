using Application.DTOs.RequestDTOs.Auth;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ChemXlabContext _context;
        public UserRepository(ChemXlabContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CheckEmailExist(string email)
        {
            var user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            return user != null;
        }

        public async Task<User?> Login(LoginDTO request)
        {
            return await _context.Users.Where(u => u.Email == request.Email && u.PasswordHash == request.Password).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }
    }
}
