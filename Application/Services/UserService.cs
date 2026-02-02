using Application.DTOs.ResponseDTOs.User;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    /// <summary>
    /// Service responsible for managing user accounts and data retrieval.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all registered users in the system.
        /// </summary>
        /// <returns>A collection of user profile DTOs.</returns>
        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var listUsers = await _unitOfWork.UserRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(listUsers);
        }
    }
}