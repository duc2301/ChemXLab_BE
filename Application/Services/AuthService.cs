using Application.DTOs.RequestDTOs.Auth;
using Application.DTOs.ResponseDTOs.User;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using ChemXLabWebAPI.DataHandler.ExceptionMidleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserResponseDTO> Login(LoginDTO request)
        {
            var user = await _unitOfWork.UserRepository.Login(request);
            if (user == null)
            {
                throw new ApiExceptionResponse("Invalid email or password");
            }
            return _mapper.Map<UserResponseDTO>(user);
        }
    }
}
