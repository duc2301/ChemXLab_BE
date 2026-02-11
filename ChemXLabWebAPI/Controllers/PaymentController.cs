using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.Payment;
using Application.Interfaces.IServices;
using ChemXLabWebAPI.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PaymentHub> _hubContext;        

        public PaymentController(IPaymentService paymentService, IConfiguration configuration, IHubContext<PaymentHub> hubContext)
        {
            _paymentService = paymentService;
            _configuration = configuration;
            _hubContext = hubContext;
        }



        /// <summary>
        /// Initiates a new payment transaction.
        /// </summary>
        /// <returns>Payment details including the QR code URL.</returns>
        /// <response code="200">Payment initiated successfully.</response>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromQuery] Guid packageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            var result = await _paymentService.CreatePaymentAsync(Guid.Parse(userId.Value), packageId);
            return Ok(ApiResponse.Success("Payment created successfully", result));
        }

        [Authorize]
        [HttpGet("My-Transaction")]
        public async Task<IActionResult> GetMyTransactions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            var result = await _paymentService.GetMyTransaction(Guid.Parse(userId.Value));
            return Ok(ApiResponse.Success("Get my transactions successfully", result));
        }

        /// <summary>
        /// Retrieves the history of all payment transactions.
        /// </summary>
        /// <returns>A list of all payments.</returns>
        /// <response code="200">Request successful, returns payment history.</response>
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            return Ok(ApiResponse.Success("Get all payments successfully", result));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse.Fail("Payment not found"));
            return Ok(ApiResponse.Success("Get payment successfully", result));
        }

        /// <summary>
        /// Cancels a pending payment transaction.
        /// </summary>
        /// <returns>Confirmation message of cancellation.</returns>
        /// <response code="200">Payment cancelled successfully.</response>
        
        [HttpPut("{id}/cancel")]
        private async Task<IActionResult> CancelPayment(Guid id)
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
            string authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Apikey "))
            {
                return Unauthorized(ApiResponse.Fail("Authorization header is missing or invalid format"));
            }

            var receivedKey = authHeader.Substring(7).Trim();

            var mySecretKey = _configuration["SePay:ApiKey"];

            if (!string.Equals(receivedKey, mySecretKey, StringComparison.Ordinal))
            {
                return Unauthorized(ApiResponse.Fail("Apikey is invalid"));
            }

            Guid? userId = await _paymentService.ConfirmPaymentAsync(dto);

            if (userId.ToString() == null)
                return BadRequest(ApiResponse.Fail("Paid fail"));

            await _hubContext.Clients.User(userId.ToString()).SendAsync("PaymentSuccess", true);

            return Ok(ApiResponse.Success("Paid successful"));
        }
    }
}