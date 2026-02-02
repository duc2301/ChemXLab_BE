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
        public async Task<PackageResponseDTO?> GetPackageByIdAsync(int id)
        {
            var package = await _unitOfWork.PackageRepository.GetByIdAsync(id);
            if (package == null) return null;
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
        public async Task<PackageResponseDTO> UpdatePackageAsync(int id, UpdatePackageDTO updatePackageDTO)
        {
            var existingPackage = await _unitOfWork.PackageRepository.GetByIdAsync(id);
            if (existingPackage == null) return null;

            _mapper.Map(updatePackageDTO, existingPackage);
            _unitOfWork.PackageRepository.Update(existingPackage);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PackageResponseDTO>(existingPackage);
        }

        /// <summary>
        /// Deletes a subscription package from the system.
        /// </summary>
        /// <param name="id">The ID of the package to delete.</param>
        /// <returns>True if the deletion was successful, otherwise False.</returns>
        public async Task<bool> DeletePackageAsync(int id)
        {
            // Use overload int DeleteById
            var existingPackage = await _unitOfWork.PackageRepository.GetByIdAsync(id);
            if (existingPackage == null) return false;

            _unitOfWork.PackageRepository.Delete(existingPackage);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}