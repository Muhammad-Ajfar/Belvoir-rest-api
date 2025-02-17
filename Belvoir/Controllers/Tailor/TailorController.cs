using Belvoir.Bll.DTO.Tailor;
using Belvoir.DAL.Models;
using Belvoir.Bll.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Belvoir.Controllers.Tailor
{
    [Route("api/[controller]")]
    [ApiController]
    public class TailorController : ControllerBase
    {
        private readonly ITailorservice _tailorService;

        public TailorController(ITailorservice tailorService)
        {
            _tailorService = tailorService;
        }

        [Authorize(Roles = "Tailor")]
        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var user =User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier);

            if (user== null)
            {
                return Unauthorized("Please login"); 
            }

            var response = await _tailorService.GET_ALL_TASK(Guid.Parse(user.Value));
            if (response.StatusCode == 200)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }


        [Authorize(Roles = "Tailor")]
        [HttpPut("tasks/{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromBody] string status)
        {
            

            var response = await _tailorService.UpdateStatus(taskId, status);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Tailor")]
        [HttpGet("tailordashboard")]
        public async Task<IActionResult> GetDashboard()
        {

            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (user == null)
            {
                return Unauthorized("Please login");
            }
            var response = await _tailorService.GetDashboardapi(Guid.Parse(user.Value));
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Tailor")]
        [HttpGet("tailorprofile")]
        public async Task<IActionResult> GetTailorProfile()
        {
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);


            var response = await _tailorService.GetTailorprofile(Guid.Parse(user.Value));
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetTailorPassword([FromBody] PasswordResetDTO data)
        {
            var userId = Guid.Parse(HttpContext.Items["UserId"].ToString()); // Fetch userId from HttpContext

            var response = await _tailorService.ResetPassword(userId, data);
            return StatusCode(response.StatusCode, response);
        }
    }
}
