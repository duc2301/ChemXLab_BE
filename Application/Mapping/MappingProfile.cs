using Application.DTOs.RequestDTOs.Auth;
using Application.DTOs.RequestDTOs.Chemical;
using Application.DTOs.RequestDTOs.Package;
using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.RequestDTOs.User;
using Application.DTOs.ResponseDTOs.Chatbot;
using Application.DTOs.ResponseDTOs.Chemical;
using Application.DTOs.ResponseDTOs.Package;
using Application.DTOs.ResponseDTOs.Payment;
using Application.DTOs.ResponseDTOs.Subscriptions;
using Application.DTOs.ResponseDTOs.User;
using Application.Interfaces.IServices;
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
            CreateMap<CreateUserDTO, User>();
            CreateMap<UpdateUserDTO, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

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

            // --- Chemical Mappings ---
            CreateMap<Chemical, ChemicalResponseDTO>()
                .ForMember(dest => dest.MolecularData, opt => opt.MapFrom(src =>
                    DeserializeFeaturesSafe(src.MolecularData)));
            CreateMap<CreateChemicalDTO, Chemical>()
                .ForMember(dest => dest.MolecularData, opt => opt.MapFrom(src =>
                    src.MolecularData != null? JsonSerializer.Serialize(src.MolecularData, (JsonSerializerOptions?)null): null));
            CreateMap<UpdateChemicalDTO, Chemical>()
                 .ForMember(dest => dest.MolecularData, opt => opt.MapFrom(src =>
                    src.MolecularData != null? JsonSerializer.Serialize(src.MolecularData, (JsonSerializerOptions?)null): null))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ChemistryAgentResponse, ChatResponseDTO>();

            CreateMap<Subscription, SubscriptionResponseDTO>().ReverseMap();
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