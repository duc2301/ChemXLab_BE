
using Application.DTOs.RequestDTOs.Auth;
using Application.DTOs.RequestDTOs.Package;
using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.ResponseDTOs.Package;
using Application.DTOs.ResponseDTOs.Payment;
using Application.DTOs.ResponseDTOs.User;
using AutoMapper;
using Domain.Entities;
using System.Text.Json;

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

            // Package
            CreateMap<CreatePackageDTO, Package>()
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src =>
                src.Features != null ? JsonSerializer.Serialize(src.Features, (JsonSerializerOptions?)null) : null));

            CreateMap<UpdatePackageDTO, Package>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src =>
                    src.Features != null ? JsonSerializer.Serialize(src.Features, (JsonSerializerOptions?)null) : null))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Package, PackageResponseDTO>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Features)
                        ? new List<string>()
                        : JsonSerializer.Deserialize<List<string>>(src.Features, (JsonSerializerOptions?)null)));
        }
    }
}
