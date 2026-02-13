using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.Package;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChemXLabWebAPI.Controllers
{
    /// <summary>
    /// Manages subscription packages (CRUD operations).
    /// </summary>
    [Route("api/packages")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        /// <summary>
        /// Retrieves a list of all available subscription packages.
        /// </summary>
        /// <returns>A collection of packages.</returns>
        /// <response code="200">Request successful, returns the list of packages.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var packages = await _packageService.GetAllPackagesAsync();
            return Ok(ApiResponse.Success("Get all packages successfully", packages));
        }

        /// <summary>
        /// Retrieves details of a specific package by ID.
        /// </summary>
        /// <returns>The detailed information of the requested package.</returns>
        /// <response code="200">Request successful, returns the package details.</response>
        [HttpGet("{id}", Name = "GetPackageById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
            {
                return NotFound(ApiResponse.Fail($"Package with ID = {id} not found"));
            }
            return Ok(ApiResponse.Success("Get package details successfully", package));
        }

        /// <summary>
        /// Creates a new subscription package.
        /// </summary>
        /// <returns>The newly created package object.</returns>
        /// <response code="201">Package created successfully.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePackageDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail("Invalid input data", ModelState));
            }
            var result = await _packageService.CreatePackageAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                ApiResponse.Success("Package created successfully", result));
        }

        /// <summary>
        /// Updates an existing package information.
        /// </summary>
        /// <returns>The updated package details.</returns>
        /// <response code="200">Package updated successfully.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePackageDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.Fail("Invalid input data", ModelState));

            var success = await _packageService.UpdatePackageAsync(id, dto);
            if (success == null)
            {
                return NotFound(ApiResponse.Fail("Update failed. Package not found."));
            }
            return Ok(ApiResponse.Success("Package updated successfully", success));
        }

        /// <summary>
        /// Deletes a package from the system.
        /// </summary>
        /// <returns>A success message indicating deletion.</returns>
        /// <response code="200">Package deleted successfully.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _packageService.DeletePackageAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse.Fail("Delete failed. Package not found."));
            }
            return Ok(ApiResponse.Success("Package deleted successfully"));
        }
    }
}