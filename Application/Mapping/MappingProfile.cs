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
            // --- User & Auth Mappings ---
            CreateMap<User, UserResponseDTO>().ReverseMap();
            CreateMap<RegisterDTO, User>().ReverseMap();
            CreateMap<LoginDTO, User>().ReverseMap();

            // --- Payment Mappings ---
            CreateMap<PaymentTransaction, PaymentResponseDTO>().ReverseMap();
            CreateMap<CreatePaymentDTO, PaymentTransaction>().ReverseMap();

            // --- Package Mappings ---

            CreateMap<CreatePackageDTO, Package>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src =>
                    src.Features != null ? JsonSerializer.Serialize(src.Features, (JsonSerializerOptions?)null) : null));

            CreateMap<UpdatePackageDTO, Package>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src =>
                    src.Features != null ? JsonSerializer.Serialize(src.Features, (JsonSerializerOptions?)null) : null))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Package, PackageResponseDTO>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src =>
                    DeserializeFeaturesSafe(src.Features)));
        }

        private List<string> DeserializeFeaturesSafe(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                json = json.Trim();
                if (json.StartsWith("["))
                {
                    return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                }

                return new List<string> { json };
            }
            catch
            {
                return new List<string> { json };
            }
        }
    }
}