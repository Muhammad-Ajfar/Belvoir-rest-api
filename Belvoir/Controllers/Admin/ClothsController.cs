﻿using Belvoir.Bll.DTO;
using Belvoir.Bll.Services.Admin;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClothesController : ControllerBase
    {
        private readonly IClothsServices _myService;

        public ClothesController(IClothsServices myService)
        {
            _myService = myService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetClothes([FromQuery] ProductQuery pquery)
        {
            var data = await _myService.GetAllCloths(pquery);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClothesById(Guid id)
        {
            var data = await _myService.GetClothById(id);
            return Ok(data);
        }



        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCloths(Guid id,IFormFile file, [FromForm]ClothDTO cloth)
        {
            var data = await _myService.UpdateCloths(id,file,cloth);
            return Ok(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<IActionResult> AddCloths(IFormFile file,[FromForm] ClothDTO cloth)
        {
            var data = await _myService.AddCloths(file,cloth);
            return Ok(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCloths(Guid id)
        {
            var data = await _myService.DeleteCloths(id);
            return Ok(data);
        }


        [Authorize(Roles = "User")]
        [HttpPost("whishlist")]
        public async Task<IActionResult> AddToWhisList(Guid productid)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _myService.AddWishlist(userId, productid);
            return StatusCode(data.StatusCode,data.Message);
        }

        [Authorize(Roles = "User")]
        [HttpGet("whishlist")]
        public async Task<IActionResult> GetWhistList()
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _myService.GetWishlist(userId);
            return StatusCode(data.StatusCode, data);
        }

        [HttpGet("cloth-rating")]
        public async Task<IActionResult> GetRating_(Guid productid)
        {
            var data = await _myService.GetRating(productid);
            return StatusCode(data.StatusCode, data);
        }

        [Authorize(Roles = "User")]
        [HttpPost("cloth-rating")]
        public async Task<IActionResult> AddRatings(Guid clothid, [FromBody] RatingItem ratings)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _myService.AddRating(userId,clothid, ratings);
            return StatusCode(data.StatusCode, data);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("cloth-rating")]
        public async Task<IActionResult> Removerating(Guid ratingid)
        {
            var data = await _myService.DeleteRating(ratingid);
            return StatusCode(data.StatusCode, data);
        }

        [Authorize(Roles = "User")]
        [HttpPut("cloth-rating")]
        public async Task<IActionResult> UpdateRating(Guid raingid, [FromBody] RatingItem ratings)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var data = await _myService.UpdateRating(raingid, ratings);
            return StatusCode(data.StatusCode, data);
        }

        [HttpGet("cloth-category")]
        public async Task<IActionResult> GetClothCategory()
        {
            var data = await _myService.GetClothCategory();
            return StatusCode(data.StatusCode, data);
        }

    }
}
