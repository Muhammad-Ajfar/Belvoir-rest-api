using Belvoir.Bll.Services.Rentals;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Rentals
{
    [Route("api/[controller]")]
    [ApiController]
    public class FabricController : ControllerBase
    {
        private readonly IFabricService _fabricService;

        public FabricController(IFabricService fabricService)
        {
            _fabricService = fabricService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFabricCategories()
        {
            var response = await _fabricService.GetFabricCategoriesAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFabricCategory([FromBody] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new Response<string>
                {
                    StatusCode = 400,
                    Message = "Fabric category name is required",
                    Error = "Validation error"
                });
            }

            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _fabricService.AddFabricCategoryAsync(name, userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateFabricCategory([FromBody] FabricCategory fabricCategory)
        {
            if (fabricCategory == null || fabricCategory.Id == Guid.Empty || string.IsNullOrWhiteSpace(fabricCategory.Name))
            {
                return BadRequest(new Response<string>
                {
                    StatusCode = 400,
                    Message = "Invalid request data",
                    Error = "Validation error"
                });
            }

            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _fabricService.UpdateFabricCategoryAsync(fabricCategory, userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
