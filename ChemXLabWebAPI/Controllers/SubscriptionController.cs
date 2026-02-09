using Application.DTOs.ApiResponseDTO;
using Application.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChemXLabWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _service;

        public SubscriptionController(ISubscriptionService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet("my-subscription")]
        public async Task<IActionResult> MySubcription()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            var subscriptions = await _service.MySubscription(Guid.Parse(userId.Value));
            return Ok(ApiResponse.Success("get subscription successful", subscriptions));
        }
    }
}
