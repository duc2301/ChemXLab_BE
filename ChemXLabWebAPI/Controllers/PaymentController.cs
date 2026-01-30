using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.RequestDTOs.Sepay;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ChemXLabWebAPI.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDTO request)
        {
            var result = await _paymentService.CreatePaymentAsync(request);
            return Ok(ApiResponse.Success("Payment created successfully", result));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            return Ok(ApiResponse.Success("Get all payments successfully", result));
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelPayment(Guid id)
        {
            var success = await _paymentService.CancelPaymentAsync(id);
            if (!success)
                return NotFound(ApiResponse.Fail("Payment not found"));

            return Ok(ApiResponse.Success("Payment cancelled successfully"));
        }


        [HttpPost("sepay/webhook")]
        public async Task<IActionResult> SePayWebhook([FromBody] SePayWebhookDTO dto)
        {
            var success = await _paymentService.ConfirmPaymentAsync(dto);

            if (!success)
                return BadRequest("Payment confirmation failed");

            return Ok("OK");
        }
    }
}
