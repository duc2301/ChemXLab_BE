using Application.DTOs.RequestDTOs.Auth;
using Domain.Entities;

namespace Application.Interfaces.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<bool> CheckEmailExist(string email);
        Task<User?> Login(LoginDTO request);
        Task<User?> GetUserByEmail(string email);
    }
}
