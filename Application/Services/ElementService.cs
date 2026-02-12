using Application.DTOs.RequestDTOs.Element;
using Application.DTOs.ResponseDTOs.Element;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    /// <summary>
    /// Service responsible for managing chemical elements.
    /// </summary>
    public class ElementService : IElementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ElementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all chemical elements.
        /// </summary>
        public async Task<IEnumerable<ElementResponseDTO>> GetAllElementsAsync()
        {
            var elements = await _unitOfWork.ElementRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ElementResponseDTO>>(elements);
        }

        /// <summary>
        /// Retrieves a specific element by its ID.
        /// </summary>
        public async Task<ElementResponseDTO?> GetElementByIdAsync(int id)
        {
            var element = await _unitOfWork.ElementRepository.GetByIdAsync(id);
            if (element == null) return null;
            return _mapper.Map<ElementResponseDTO>(element);
        }

        /// <summary>
        /// Retrieves a specific element by its chemical symbol.
        /// </summary>
        public async Task<ElementResponseDTO?> GetElementBySymbolAsync(string symbol)
        {
            var element = await _unitOfWork.ElementRepository.GetBySymbolAsync(symbol);
            if (element == null) return null;
            return _mapper.Map<ElementResponseDTO>(element);
        }

        /// <summary>
        /// Creates a new chemical element in the database.
        /// </summary>
        public async Task<ElementResponseDTO> CreateElementAsync(CreateElementDTO createElementDTO)
        {
            var symbolExists = await _unitOfWork.ElementRepository.ElementSymbolExistsAsync(createElementDTO.Symbol);
            if (symbolExists)
                throw new InvalidOperationException($"Element with symbol '{createElementDTO.Symbol}' already exists.");

            var element = _mapper.Map<Element>(createElementDTO);
            await _unitOfWork.ElementRepository.CreateAsync(element);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ElementResponseDTO>(element);
        }

        /// <summary>
        /// Updates an existing chemical element.
        /// </summary>
        public async Task<ElementResponseDTO?> UpdateElementAsync(int id, UpdateElementDTO updateElementDTO)
        {
            var existingElement = await _unitOfWork.ElementRepository.GetByIdAsync(id);
            if (existingElement == null) return null;

            // Check if new symbol is being set and if it already exists elsewhere
            if (!string.IsNullOrEmpty(updateElementDTO.Symbol) && 
                updateElementDTO.Symbol != existingElement.Symbol)
            {
                var symbolExists = await _unitOfWork.ElementRepository.ElementSymbolExistsAsync(updateElementDTO.Symbol);
                if (symbolExists)
                    throw new InvalidOperationException($"Element with symbol '{updateElementDTO.Symbol}' already exists.");
            }

            _mapper.Map(updateElementDTO, existingElement);
            _unitOfWork.ElementRepository.Update(existingElement);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ElementResponseDTO>(existingElement);
        }

        /// <summary>
        /// Deletes a chemical element from the system.
        /// </summary>
        public async Task<bool> DeleteElementAsync(int id)
        {
            var existingElement = await _unitOfWork.ElementRepository.GetByIdAsync(id);
            if (existingElement == null) return false;

            _unitOfWork.ElementRepository.Delete(existingElement);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
