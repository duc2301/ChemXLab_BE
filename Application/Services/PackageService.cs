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
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PackageResponseDTO>> GetAllPackagesAsync()
        {
            var packages = await _unitOfWork.PackageRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PackageResponseDTO>>(packages);
        }

        public async Task<PackageResponseDTO?> GetPackageByIdAsync(int id)
        {
            var package = await _unitOfWork.PackageRepository.GetByIdAsync(id);
            if (package == null) return null;
            return _mapper.Map<PackageResponseDTO>(package);
        }

        public async Task<PackageResponseDTO> CreatePackageAsync(CreatePackageDTO createPackageDTO)
        {
            var package = _mapper.Map<Package>(createPackageDTO);

            await _unitOfWork.PackageRepository.CreateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PackageResponseDTO>(package);
        }

        public async Task<PackageResponseDTO> UpdatePackageAsync(int id, UpdatePackageDTO updatePackageDTO)
        {
            var existingPackage = await _unitOfWork.PackageRepository.GetByIdAsync(id);
            if (existingPackage == null) return null;

            _mapper.Map(updatePackageDTO, existingPackage);
            _unitOfWork.PackageRepository.Update(existingPackage);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PackageResponseDTO>(existingPackage);
        }

        public async Task<bool> DeletePackageAsync(int id)
        {
            // Dùng hàm overload int DeleteById
            var existingPackage = await _unitOfWork.PackageRepository.GetByIdAsync(id);
            if (existingPackage == null) return false;

            _unitOfWork.PackageRepository.Delete(existingPackage);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
