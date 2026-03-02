using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.DateTimeRequestDTOs;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChemXLabWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : Controller
    {
        private readonly IPaymentService _paymentService;

        public DashboardController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTransactionInformation([FromQuery]FromToDateRequestDTOs dateRequestDTOs)
        {
            var transactions = await _paymentService.GetTransactionsByDateRangeAsync(dateRequestDTOs);
            return Ok(ApiResponse.Success("Get transaction data list successful", transactions));
        }
    }
}
