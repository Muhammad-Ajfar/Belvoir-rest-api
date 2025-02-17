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
            var userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _tailorService.GetAllTasks(userId);
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
            var userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _tailorService.GetDashboardapi(userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Tailor")]
        [HttpGet("tailorprofile")]
        public async Task<IActionResult> GetTailorProfile()
        {
            var userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _tailorService.GetTailorprofile(userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetTailorPassword([FromBody] PasswordResetDTO data)
        {
            var userId = Guid.Parse(HttpContext.Items["UserId"].ToString()); 
            var response = await _tailorService.ResetPassword(userId, data);
            return StatusCode(response.StatusCode, response);
        }
    }
}
