using Application.DTOs.RequestDTOs.Package;
using Application.DTOs.ResponseDTOs.Package;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    /// <summary>
    /// Service responsible for managing subscription packages.
    /// </summary>
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all available subscription packages.
        /// </summary>
        /// <returns>A collection of package DTOs.</returns>
        public async Task<IEnumerable<PackageResponseDTO>> GetAllPackagesAsync()
        {
            var packages = await _unitOfWork.PackageRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PackageResponseDTO>>(packages);
        }

        /// <summary>
        /// Retrieves a specific package by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the package.</param>
        /// <returns>The package details if found, otherwise null.</returns>
        public async Task<PackageResponseDTO?> GetPackageByIdAsync(Guid id) 
        {
            var package = await _unitOfWork.PackageRepository.GetByIdAsync(id);
            if (package == null || package.Status == "Inactive")
            {
                return null;
            }
            return _mapper.Map<PackageResponseDTO>(package);
        }

        /// <summary>
        /// Creates a new subscription package in the database.
        /// </summary>
        /// <param name="createPackageDTO">The data for the new package.</param>
        /// <returns>The created package details.</returns>
        public async Task<PackageResponseDTO> CreatePackageAsync(CreatePackageDTO createPackageDTO)
        {
            var package = _mapper.Map<Package>(createPackageDTO);

            package.Id = Guid.NewGuid();
            package.Status = "Active";

            await _unitOfWork.PackageRepository.CreateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PackageResponseDTO>(package);
        }

        /// <summary>
        /// Updates an existing subscription package.
        /// </summary>
        /// <param name="id">The ID of the package to update.</param>
        /// <param name="updatePackageDTO">The updated package data.</param>
        /// <returns>The updated package details, or null if the package does not exist.</returns>
        public async Task<PackageResponseDTO?> UpdatePackageAsync(Guid id, UpdatePackageDTO updatePackageDTO)
        {
            var existingPackage = await _unitOfWork.PackageRepository.GetByIdAsync(id);
            if (existingPackage == null) return null;

            _mapper.Map(updatePackageDTO, existingPackage);
            _unitOfWork.PackageRepository.Update(existingPackage);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PackageResponseDTO>(existingPackage);
        }

        /// <summary>
        /// Soft Deletes a subscription package (Changes status to InActive).
        /// </summary>
        /// <param name="id">The ID of the package to delete.</param>
        /// <returns>True if the deletion was successful, otherwise False.</returns>
        public async Task<bool> DeletePackageAsync(Guid id)
        {
            var existingPackage = await _unitOfWork.PackageRepository.GetByIdAsync(id);
            if (existingPackage == null) return false;

            existingPackage.Status = "Inactive";

            _unitOfWork.PackageRepository.Update(existingPackage);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}