
using Application.DTOs.RequestDTOs.Auth;
using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.ResponseDTOs.Payment;
using Application.DTOs.ResponseDTOs.User;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponseDTO>().ReverseMap();
            CreateMap<RegisterDTO, User>().ReverseMap();
            CreateMap<PaymentTransaction, PaymentResponseDTO>().ReverseMap();
            CreateMap<CreatePaymentDTO, PaymentTransaction>().ReverseMap();
        }
    }
}
