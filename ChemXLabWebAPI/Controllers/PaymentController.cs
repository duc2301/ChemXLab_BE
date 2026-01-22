using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.Payment;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ChemXLabWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayment()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            return Ok(ApiResponse.Success("Get all payment successful", result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            return Ok(ApiResponse.Success("Get payment by id successful", result));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDTO requestDTO)
        {
            var result = await _paymentService.CreatePaymentAsync(requestDTO);
            return Ok(ApiResponse.Success("Payment created successfully", result));
        }
    }
}
