using Application.DTOs.RequestDTOs.Package;
using Application.DTOs.ResponseDTOs.Package;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IPackageService
    {
        Task<IEnumerable<PackageResponseDTO>> GetAllPackagesAsync();
        Task<PackageResponseDTO?> GetPackageByIdAsync(int id);
        Task<PackageResponseDTO> CreatePackageAsync(CreatePackageDTO createPackageDTO);
        Task<PackageResponseDTO?> UpdatePackageAsync(int id, UpdatePackageDTO updatePackageDTO);
        Task<bool> DeletePackageAsync(int id);
    }
}
