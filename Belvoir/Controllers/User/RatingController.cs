using Belvoir.Bll.Services.UserSer;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.UserRep;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("get")]
        public async Task<IActionResult> GetRating_(Guid entityId,string rating_to)
        {
            var data = await _ratingService.GetRating(entityId, rating_to);
            return StatusCode(data.StatusCode, data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "User")]
        [HttpPost("add")]
        public async Task<IActionResult> AddRatings(Guid entityId, [FromBody] RatingItem ratings,string rating_to)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _ratingService.AddRating(userId, entityId, ratings, rating_to);
            return StatusCode(data.StatusCode, data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "User")]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Removerating(Guid ratingid, string rating_to)
        {
            var data = await _ratingService.DeleteRating(ratingid,rating_to);
            return StatusCode(data.StatusCode, data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "User")]
        [HttpPut("Add")]
        public async Task<IActionResult> UpdateRating(Guid ratingid, [FromBody] RatingItem ratings, string rating_to)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _ratingService.UpdateRating(ratingid, ratings,userId,rating_to);
            return StatusCode(data.StatusCode, data);
        }
    }
}
