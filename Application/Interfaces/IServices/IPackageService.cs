using Application.DTOs.RequestDTOs.Package;
using Application.DTOs.ResponseDTOs.Package;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Service for managing subscription packages logic.
    /// </summary>
    public interface IPackageService
    {
        /// <summary>
        /// Retrieves all available subscription packages.
        /// </summary>
        /// <returns>A collection of package details.</returns>
        Task<IEnumerable<PackageResponseDTO>> GetAllPackagesAsync();

        /// <summary>
        /// Retrieves detailed information about a specific package.
        /// </summary>
        /// <returns>The package details if found, otherwise null.</returns>
        Task<PackageResponseDTO?> GetPackageByIdAsync(int id);

        /// <summary>
        /// Creates a new subscription package.
        /// </summary>
        /// <returns>The details of the created package.</returns>
        Task<PackageResponseDTO> CreatePackageAsync(CreatePackageDTO createPackageDTO);

        /// <summary>
        /// Updates an existing subscription package.
        /// </summary>
        /// <returns>The updated package details.</returns>
        Task<PackageResponseDTO?> UpdatePackageAsync(int id, UpdatePackageDTO updatePackageDTO);

        /// <summary>
        /// Deletes a subscription package from the system.
        /// </summary>
        /// <returns>True if deletion was successful, otherwise False.</returns>
        Task<bool> DeletePackageAsync(int id);
    }
}