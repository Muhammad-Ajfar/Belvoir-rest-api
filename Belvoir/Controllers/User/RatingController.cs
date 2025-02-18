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
        [HttpGet("rating/get")]
        public async Task<IActionResult> GetRating_(Guid productid,string rating_to)
        {
            var data = await _ratingService.GetRating(productid, rating_to);
            return StatusCode(data.StatusCode, data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "User")]
        [HttpPost("rating/add")]
        public async Task<IActionResult> AddRatings(Guid clothid, [FromBody] RatingItem ratings,string rating_to)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _ratingService.AddRating(userId, clothid, ratings, rating_to);
            return StatusCode(data.StatusCode, data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "User")]
        [HttpDelete("rating/Delete")]
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
        [HttpPut("rating/Add")]
        public async Task<IActionResult> UpdateRating(Guid raingid, [FromBody] RatingItem ratings, string rating_to)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _ratingService.UpdateRating(raingid, ratings,userId,rating_to);
            return StatusCode(data.StatusCode, data);
        }
    }
}
