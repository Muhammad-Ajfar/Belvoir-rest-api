
using Belvoir.Bll.DTO.Tailor;
using Belvoir.Bll.DTO.User;
using Belvoir.Bll.Services.Admin;
using CloudinaryDotNet.Actions;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Belvoir.Bll.DTO.Delivery;
using Microsoft.AspNetCore.Authorization;
using Belvoir.DAL.Models.Query;

namespace Belvoir.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminServices _myService;

        public AdminController(IAdminServices myService)
        {
            _myService = myService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users/{role}")]
        public async Task<IActionResult> GetUsers(string role,[FromQuery] UserQuery userQuery)
        {
            var data = await _myService.GetAllUsers(role,userQuery);
            return Ok(data);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("user/id/{id}")]
        public async Task<IActionResult> GetUsersById(Guid id)
        {
            var data = await _myService.GetUserById(id);
            return Ok(data);
        }



        // PATCH Endpoint for Blocking/Unblocking a User
        [Authorize(Roles = "Admin")]
        [HttpPatch("{role}/block-unblock/{id}")]
        public async Task<IActionResult> BlockOrUnblockUser(Guid id,string role)
        {
            var response = await _myService.BlockOrUnblock(id,role);
            if (response.StatusCode == 400)
            {
                return BadRequest(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add/tailor")]
        public async Task<IActionResult> AddTailor([FromBody] TailorDTO tailorDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _myService.AddTailor(tailorDTO);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{role}")]
        public async Task<IActionResult> DeleteTailor(Guid id,string role)
        {
            var response = await _myService.DeleteTailor(id,role);
            return StatusCode(response.StatusCode, response);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("add/Delivery")]
        public async Task<IActionResult> AddDelivery(DeliveryDTO deliveryDTO)
        {
            var response = await _myService.AddDelivery(deliveryDTO);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add/laundry")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _myService.AddLaundry(registerDTO);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("sales-report")]
        public async Task<IActionResult> SalesReport()
        {
            var respose =await _myService.GetSalesReport();
            return StatusCode(respose.StatusCode, respose);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboarddata()
        {
            var respose = await _myService.GetDasboard();
            return StatusCode(respose.StatusCode, respose);
        }

        [HttpPost("Add/Task/delivery")]
        public async Task<IActionResult> AssaignOrdersByPinCode(string pin,Guid delivery_id,DateTime deadline)
        {
            var response = await _myService.AssaignOrdersByPinCode(pin, delivery_id,deadline);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("Add/Task/delivery{order_id}")]
        public async Task<IActionResult> AssaignOrdersByOrderId(Guid order_id, Guid delivery_id, DateTime deadline)
        {
            var response = await _myService.AssaignOrderByOrderId(order_id, delivery_id, deadline);
            return StatusCode(response.StatusCode, response);
        }
    }
}
