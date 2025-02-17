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
        [Authorize]
        [HttpPost("add/tailorProduct")]
        public  async Task<IActionResult> CreateTailorProduct(TailorProductDTO tailorProductDTO)
        {
            var user_id = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var response = await _orderServices.AddTailorProducts(tailorProductDTO,user_id);
            return StatusCode(statusCode: response.StatusCode, response);
        }
        [Authorize]
        [HttpGet("TailorProducts/All")]
        public async Task<IActionResult> GetTailorproduct()
        {
            var user_id = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var response = await _orderServices.GetAllTailorProducts( user_id);
            return StatusCode(statusCode: response.StatusCode, response);
        }
        [Authorize]
        [HttpGet("TailorProducts/{product_id}")]
        public async Task<IActionResult> GetTailorproductById(Guid product_id)
        {
            var user_id = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var response = await _orderServices.TailorProductsById(product_id,user_id);
            return StatusCode(statusCode: response.StatusCode, response);
        }
        [Authorize]
        [HttpDelete("remove/tailorproduct")]
        public async Task<IActionResult> RemoveTailorProduct(Guid product_id)
        {
            var user_id = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var response = await _orderServices.RemoveTailorProduct(product_id,user_id);
            return StatusCode(statusCode: response.StatusCode,response);
        }

        [Authorize]
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> AddOrder(PlaceOrderDTO orderDto)
        {
            var user_id = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var response = await _orderServices.AddOrder(orderDto, user_id );
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("checkout/rental")]
        public async Task<IActionResult> CheckoutRentalCart([FromBody] CheckoutRentalCartDTO checkoutDto)
        {
            var userId = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var response = await _orderServices.CheckoutRentalCartAsync(userId, checkoutDto);

            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("user/tailor")]
        public async Task<IActionResult> getUserOrder(string? status)
        {
            var user_id = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var response = await _orderServices.orderUserGets(user_id, status);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("user/rental")]
        public async Task<IActionResult> getRentalUserOrder(string? status)
        {
            var user_id = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var response = await _orderServices.orderRentalUserGets(user_id, status);
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
        [HttpGet("{order_id}")]
        public async Task<IActionResult> SingleOrder(Guid order_id)
        {
            var response = await _orderServices.SingleOrder(order_id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("update/status/{order_id}")]
        public async Task<IActionResult> OrderStatusUpdate(Guid order_id,string status)
        {
            var response = await _orderServices.SingleOrder(order_id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
