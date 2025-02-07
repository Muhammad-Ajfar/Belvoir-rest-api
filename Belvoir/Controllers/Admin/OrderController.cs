using Belvoir.Bll.DTO.Order;
using Belvoir.Bll.Services.Admin;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Belvoir.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _orderServices;
        public OrderController(IOrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        [HttpPost("add/tailorProduct")]
        public  async Task<IActionResult> CreateTailorProduct(TailorProductDTO tailorProductDTO)
        {
            //string user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).ToString();
            //Guid userId = Guid.TryParse(user, out Guid parsedGuid) ? parsedGuid : Guid.Empty;
            var response = await _orderServices.AddTailorProducts(tailorProductDTO);
            return StatusCode(statusCode: response.StatusCode, response);
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> AddOrder(Order order)
        {
            var user_id = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var response = await _orderServices.AddOrder(order, user_id );
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> getUserOrder(string? status)
        {
            var user_id = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var response = await _orderServices.orderUserGets(user_id, status);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> getAdminOrder(string? status)
        {
            var response = await _orderServices.orderAdminGets(status);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("delivery")]
        public async Task<IActionResult> getDeliveryOrder()
        {
            var response = await _orderServices.orderDeliveryGets();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("tailor")]
        public async Task<IActionResult> getTailorOrder()
        {
            var response = await _orderServices.orderTailorGets();
            return StatusCode(response.StatusCode, response);
        }
    }
}
