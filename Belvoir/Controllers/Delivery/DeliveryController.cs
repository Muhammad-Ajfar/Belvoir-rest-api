using Belvoir.Bll.Services.DeliverySer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Delivery
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private IDeliveryServices _service;
        public DeliveryController(IDeliveryServices services)
        {
            _service = services;
        }

        [HttpGet("profile-delivery")]
        public async Task<IActionResult> GetDeliveryProfile()
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _service.GetDeliveryProfile(userId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("dashboard")]
        public async  Task<IActionResult> GetDeliveryDashboard() {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _service.GetDeliveryDashboard(userId);
            return StatusCode(response.StatusCode, response);
        }

    }
}
