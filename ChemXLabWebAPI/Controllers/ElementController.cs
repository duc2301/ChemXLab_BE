using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.Element;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ChemXLabWebAPI.Controllers
{
    /// <summary>
    /// Manages chemical elements (CRUD operations).
    /// </summary>
    [Route("api/elements")]
    [ApiController]
    public class ElementController : ControllerBase
    {
        private readonly IElementService _elementService;

        public ElementController(IElementService elementService)
        {
            _elementService = elementService;
        }

        /// <summary>
        /// Retrieves a list of all chemical elements.
        /// </summary>
        /// <returns>A collection of elements.</returns>
        /// <response code="200">Request successful, returns the list of elements.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var elements = await _elementService.GetAllElementsAsync();
            return Ok(ApiResponse.Success("Get all elements successfully", elements));
        }

        /// <summary>
        /// Retrieves details of a specific element by ID.
        /// </summary>
        /// <returns>The detailed information of the requested element.</returns>
        /// <response code="200">Request successful, returns the element details.</response>
        [HttpGet("{id}", Name = "GetElementById")]
        public async Task<IActionResult> GetById(int id)
        {
            var element = await _elementService.GetElementByIdAsync(id);
            if (element == null)
            {
                return NotFound(ApiResponse.Fail($"Element with ID = {id} not found"));
            }
            return Ok(ApiResponse.Success("Get element details successfully", element));
        }

        /// <summary>
        /// Retrieves details of a specific element by its chemical symbol.
        /// </summary>
        /// <param name="symbol">The chemical symbol of the element (e.g., "H", "He", "Li")</param>
        /// <returns>The detailed information of the requested element.</returns>
        /// <response code="200">Request successful, returns the element details.</response>
        [HttpGet("symbol/{symbol}")]
        public async Task<IActionResult> GetBySymbol(string symbol)
        {
            var element = await _elementService.GetElementBySymbolAsync(symbol);
            if (element == null)
            {
                return NotFound(ApiResponse.Fail($"Element with symbol = {symbol} not found"));
            }
            return Ok(ApiResponse.Success("Get element details successfully", element));
        }

        /// <summary>
        /// Creates a new chemical element.
        /// </summary>
        /// <returns>The newly created element object.</returns>
        /// <response code="201">Element created successfully.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateElementDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail("Invalid input data", ModelState));
            }

            try
            {
                var result = await _elementService.CreateElementAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id },
                    ApiResponse.Success("Element created successfully", result));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Updates an existing element information.
        /// </summary>
        /// <returns>The updated element details.</returns>
        /// <response code="200">Element updated successfully.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateElementDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.Fail("Invalid input data", ModelState));

            try
            {
                var success = await _elementService.UpdateElementAsync(id, dto);
                if (success == null)
                {
                    return NotFound(ApiResponse.Fail("Update failed. Element not found."));
                }
                return Ok(ApiResponse.Success("Element updated successfully", success));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Deletes an element from the system.
        /// </summary>
        /// <returns>A success message indicating deletion.</returns>
        /// <response code="200">Element deleted successfully.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _elementService.DeleteElementAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse.Fail("Delete failed. Element not found."));
            }
            return Ok(ApiResponse.Success("Element deleted successfully"));
        }
    }
}
