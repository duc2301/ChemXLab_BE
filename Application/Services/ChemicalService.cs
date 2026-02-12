using Application.DTOs.RequestDTOs.Chemical;
using Application.DTOs.ResponseDTOs.Chemical;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ChemicalService : IChemicalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChemicalService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChemicalResponseDTO>> GetAllChemicalsAsync()
        {
            var chemicals = await _unitOfWork.ChemicalRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ChemicalResponseDTO>>(chemicals);
        }

        public async Task<ChemicalResponseDTO?> GetChemicalByIdAsync(Guid id)
        {
            var chemical = await _unitOfWork.ChemicalRepository.GetByIdAsync(id);
            if (chemical == null || chemical.IsPublic == false)
            {
                return null;
            }
            return _mapper.Map<ChemicalResponseDTO>(chemical);
        }

        public async Task<ChemicalResponseDTO> CreateChemicalAsync(CreateChemicalDTO createDto)
        {
            var chemical = _mapper.Map<Chemical>(createDto);

            await _unitOfWork.ChemicalRepository.CreateAsync(chemical);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ChemicalResponseDTO>(chemical);
        }

        public async Task<ChemicalResponseDTO> UpdateChemicalAsync(Guid id, UpdateChemicalDTO updateDto)
        {
            var existingChemical = await _unitOfWork.ChemicalRepository.GetByIdAsync(id);
            if (existingChemical == null) return null;

            _mapper.Map(updateDto, existingChemical);

            _unitOfWork.ChemicalRepository.Update(existingChemical);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ChemicalResponseDTO>(existingChemical);
        }

        public async Task<bool> DeleteChemicalAsync(Guid id)
        {
            var chemical = await _unitOfWork.ChemicalRepository.GetByIdAsync(id);
            if (chemical == null) return false;
            chemical.IsPublic = false;

            _unitOfWork.ChemicalRepository.Update(chemical);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}