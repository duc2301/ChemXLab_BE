using Application.DTOs.RequestDTOs.Chemical;
using Application.DTOs.ResponseDTOs.Chemical;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IChemicalService
    {
        Task<IEnumerable<ChemicalResponseDTO>> GetAllChemicalsAsync();
        Task<ChemicalResponseDTO?> GetChemicalByIdAsync(Guid id);
        Task<ChemicalResponseDTO> CreateChemicalAsync(CreateChemicalDTO createDto);
        Task<ChemicalResponseDTO> UpdateChemicalAsync(Guid id, UpdateChemicalDTO updateDto);
        Task<bool> DeleteChemicalAsync(Guid id);
    }
}
