using Application.DTOs.RequestDTOs.Auth;
using Domain.Entities;

namespace Application.Interfaces.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> Login(LoginDTO request);
    }
}
