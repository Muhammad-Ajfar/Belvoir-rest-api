using Belvoir.Bll.DTO.Rental;
using Belvoir.Bll.DTO;
using Belvoir.Bll.Services.Rentals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Belvoir.DAL.Models.Query;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Authorization;

namespace Belvoir.Controllers.Rentals
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _service;
        public RentalController(IRentalService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddRental(IFormFile[] files, [FromForm] RentalSetDTO rentalData)
        {
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var response = await _service.AddRental(files, rentalData, Guid.Parse(user.Value));
            return StatusCode(response.StatusCode, response);

        }

        [HttpGet("id")]
        public async Task<IActionResult> SearchRentalid(Guid id)
        {

            var response = await _service.GetRentalById(id);
            return StatusCode(response.StatusCode, response);

        }


        
        [HttpGet("products")]
        public async Task<IActionResult> Paginated([FromQuery] RentalQuery query)
        {

            var response = await _service.PaginatedRentalProduct(query);
            return StatusCode(response.StatusCode, response);

        }

        [HttpDelete("")]
        public async Task<IActionResult> DeleteRental(Guid id)
        {
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var response = await _service.DeleteRental(id, Guid.Parse(user.Value));
            return StatusCode(response.StatusCode, response);

        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateRental(Guid rentalId, IFormFile[] files, [FromForm] RentalSetDTO rentalData)
        {
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var response = await _service.UpdateRental(rentalId, files, rentalData, Guid.Parse(user.Value));
            return StatusCode(response.StatusCode, response);

        }

        [Authorize]
        [HttpPost("whishlist")]
        public async Task<IActionResult> AddToWhisList(Guid productid)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _service.AddWishlist(userId, productid);
            return StatusCode(data.StatusCode, data);
        }

        [HttpGet("whishlist")]
        public async Task<IActionResult> GetWhistList()
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _service.GetWishlist(userId);
            return StatusCode(data.StatusCode, data);
        }

        [HttpGet("rental-rating")]
        public async Task<IActionResult> GetRating_(Guid productid)
        {
            var data = await _service.GetRating(productid);
            return StatusCode(data.StatusCode, data);
        }

        [Authorize]
        [HttpPost("rental-rating")]
        public async Task<IActionResult> AddRatings(Guid productid, [FromBody] RatingItem ratings)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _service.AddRating(userId, productid, ratings);
            return StatusCode(data.StatusCode, data);
        }

        [Authorize]
        [HttpDelete("rental-rating")]
        public async Task<IActionResult> Removerating(Guid ratingid)
        {
            var data = await _service.DeleteRating(ratingid);
            return StatusCode(data.StatusCode, data);
        }

        [Authorize]
        [HttpPut("rental-rating")]
        public async Task<IActionResult> UpdateRating(Guid raitngid, [FromBody] RatingItem ratings)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _service.UpdateRating(raitngid, ratings);
            return StatusCode(data.StatusCode, data);
        }
    }
}
