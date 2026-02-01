using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.Package;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ChemXLabWebAPI.Controllers
{
    [Route("api/packages")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var packages = await _packageService.GetAllPackagesAsync();
            return Ok(ApiResponse.Success("Get all packages successfully", packages));
        }

        [HttpGet("{id}", Name = "GetPackageById")]
        public async Task<IActionResult> GetById(int id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
            {
                return NotFound(ApiResponse.Fail($"Package with ID = {id} not found"));
            }
            return Ok(ApiResponse.Success("Get package details successfully", package));
        }

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePackageDTO dto)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
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