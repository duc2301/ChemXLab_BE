using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.RequestDTOs.Sepay;
using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChemXLabWebAPI.Controllers
{
    /// <summary>
    /// Handles payment transactions and Webhook integration.
    /// </summary>
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Initiates a new payment transaction.
        /// </summary>
        /// <returns>Payment details including the QR code URL.</returns>
        /// <response code="200">Payment initiated successfully.</response>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromQuery] int packageId)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _paymentService.CreatePaymentAsync(Guid.Parse(userId), packageId);
            return Ok(ApiResponse.Success("Payment created successfully", result));
        }

        [Authorize]
        [HttpGet("My-Transaction")]
        public async Task<IActionResult> GetMyTransactions()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var result = await _paymentService.GetMyTransaction(Guid.Parse(userId));
            return Ok(ApiResponse.Success("Get my transactions successfully", result));
        }

        /// <summary>
        /// Retrieves the history of all payment transactions.
        /// </summary>
        /// <returns>A list of all payments.</returns>
        /// <response code="200">Request successful, returns payment history.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            return Ok(ApiResponse.Success("Get all payments successfully", result));
        }

        /// <summary>
        /// Cancels a pending payment transaction.
        /// </summary>
        /// <returns>Confirmation message of cancellation.</returns>
        /// <response code="200">Payment cancelled successfully.</response>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelPayment(Guid id)
        {
            var success = await _paymentService.CancelPaymentAsync(id);
            if (!success)
                return NotFound(ApiResponse.Fail("Payment not found"));

            return Ok(ApiResponse.Success("Payment cancelled successfully"));
        }

        /// <summary>
        /// Handles Webhook notifications from the SePay gateway.
        /// </summary>
        /// <returns>Standard acknowledgement response.</returns>
        /// <response code="200">Webhook processed successfully.</response>
        [HttpPost("sepay/webhook")]
        public async Task<IActionResult> SePayWebhook([FromBody] SePayWebhookDTO dto)
        {
            var success = await _paymentService.ConfirmPaymentAsync(dto);

            if (!success)
                return BadRequest(ApiResponse.Fail("Paid fail"));

            return Ok(ApiResponse.Success("Paid successful"));
        }
    }
}