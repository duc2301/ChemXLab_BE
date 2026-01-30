using Application.DTOs.RequestDTOs.Auth;
using Application.DTOs.ResponseDTOs.User;
using Application.ExceptionMidleware;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<string> Login(LoginDTO request)
        {
            var user = await _unitOfWork.UserRepository.Login(request);
            if (user == null)
            {
                throw new ApiExceptionResponse("Invalid email or password");
            }
            var token = _jwtService.GenerateToken(user);
            return token;
        }
        
        public async Task<UserResponseDTO> Register(RegisterDTO request)
        {
            var checkEmailExist = await _unitOfWork.UserRepository.CheckEmailExist(request.Email);
            if (checkEmailExist)
            {
                throw new ApiExceptionResponse("Email already in use");
            }
            var newUser = _mapper.Map<User>(request);
            await _unitOfWork.UserRepository.CreateAsync(newUser);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserResponseDTO>(newUser);
        }
    }
}
