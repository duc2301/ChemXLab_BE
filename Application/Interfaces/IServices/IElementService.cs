using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.RequestDTOs.Element;
using Application.DTOs.ResponseDTOs.Element;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Service interface for managing chemical elements.
    /// </summary>
    public interface IElementService
    {
        /// <summary>
        /// Retrieves all chemical elements.
        /// </summary>
        Task<IEnumerable<ElementResponseDTO>> GetAllElementsAsync();

        /// <summary>
        /// Retrieves a specific element by its ID.
        /// </summary>
        Task<ElementResponseDTO?> GetElementByIdAsync(int id);

        /// <summary>
        /// Retrieves a specific element by its chemical symbol.
        /// </summary>
        Task<ElementResponseDTO?> GetElementBySymbolAsync(string symbol);

        /// <summary>
        /// Creates a new chemical element.
        /// </summary>
        Task<ElementResponseDTO> CreateElementAsync(CreateElementDTO createElementDTO);

        /// <summary>
        /// Updates an existing chemical element.
        /// </summary>
        Task<ElementResponseDTO?> UpdateElementAsync(int id, UpdateElementDTO updateElementDTO);

        /// <summary>
        /// Deletes a chemical element from the system.
        /// </summary>
        Task<bool> DeleteElementAsync(int id);
    }
}
