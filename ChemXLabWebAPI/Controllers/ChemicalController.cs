using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.Chemical;
using Application.DTOs.ResponseDTOs.Chemical;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChemXLabWebAPI.Controllers
{
    /// <summary>
    /// Represents an API controller that manages chemical resources, providing endpoints for creating, retrieving,
    /// updating, and deleting chemicals.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ChemicalController : ControllerBase
    {
        private readonly IChemicalService _chemicalService;

        public ChemicalController(IChemicalService chemicalService)
        {
            _chemicalService = chemicalService;
        }
        /// <summary>
        /// Retrieves all chemical records.
        /// </summary>
        /// <remarks>This method handles HTTP GET requests to obtain all available chemicals. The response
        /// includes a standardized API result object. If an error occurs during retrieval, the method returns a bad
        /// request with the error message.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing a success response with the list of chemicals if the operation
        /// succeeds; otherwise, a bad request response with error details.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _chemicalService.GetAllChemicalsAsync();
                return Ok(ApiResponse.Success("Get all chemicals success", result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the details of a chemical by its unique identifier.
        /// </summary>
        /// <remarks>Returns a 404 response if the specified chemical does not exist. Returns a 400
        /// response if an error occurs during retrieval.</remarks>
        /// <param name="id">The unique identifier of the chemical to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the chemical details if found; otherwise, a response indicating
        /// that the chemical was not found or an error occurred.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _chemicalService.GetChemicalByIdAsync(id);
                if (result == null)
                {
                    return NotFound(ApiResponse.Fail("Chemical not found"));
                }
                return Ok(ApiResponse.Success("Get chemical detail success", result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Creates a new chemical entity using the provided data.
        /// </summary>
        /// <param name="createDto">The data transfer object containing the information required to create a new chemical. Cannot be null.</param>
        /// <returns>An IActionResult that represents the result of the create operation. Returns a 201 Created response with the
        /// created chemical on success, or a 400 Bad Request response if validation fails or an error occurs.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChemicalDTO createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _chemicalService.CreateChemicalAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse.Success("Chemical created successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Updates the details of an existing chemical with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the chemical to update.</param>
        /// <param name="updateDto">An object containing the updated chemical information. The <see cref="UpdateChemicalDTO.Id"/> property must
        /// match the <paramref name="id"/> parameter.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the update operation. Returns 200 (OK) with the
        /// updated chemical on success, 400 (Bad Request) if validation fails or the IDs do not match, or 404 (Not
        /// Found) if the chemical does not exist.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChemicalDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail("Validation failed"));
            }

            try
            {
                var result = await _chemicalService.UpdateChemicalAsync(id, updateDto);

                return Ok(ApiResponse.Success("Chemical updated successfully", result));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail("Chemical not found"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Deletes the chemical with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the chemical to delete.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the delete operation. Returns 200 (OK) if the
        /// chemical was deleted successfully, 404 (Not Found) if the chemical does not exist, or 400 (Bad Request) if
        /// an error occurs.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var isDeleted = await _chemicalService.DeleteChemicalAsync(id);
                if (!isDeleted)
                {
                    return NotFound(ApiResponse.Fail("Chemical not found"));
                }
                return Ok(ApiResponse.Success("Chemical deleted successfully", null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }
    }
}