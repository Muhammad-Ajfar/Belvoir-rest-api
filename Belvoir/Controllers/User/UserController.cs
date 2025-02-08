using Belvoir.Bll.Services.UserSer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IuserService _userService;
        public UserController(IuserService userService)
        {
            _userService = userService;   
        }

        [Authorize]
        [HttpGet("profile-User")]
        public async Task<IActionResult> GetDeliveryProfile()
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _userService.GetUserProfile(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
