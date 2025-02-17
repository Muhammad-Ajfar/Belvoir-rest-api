using Belvoir.Bll.Services.Admin;
using Belvoir.Bll.DTO.Design;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Belvoir.DAL.Models.Mesurements;
using Microsoft.AspNetCore.Authorization;
using Belvoir.DAL.Models.Query;

namespace Belvoir.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignController : ControllerBase
    {
        private readonly IDesignService _designService;

        public DesignController(IDesignService designService)
        {
            _designService = designService;
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetDesigns([FromQuery] DesignQueryParameters queryParams)
        {
            var result = await _designService.GetDesignsAsync(queryParams);
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,user")]
        [HttpGet("{designId}")]
        public async Task<IActionResult> GetDesignById(Guid designId)
        {
            var response = await _designService.GetDesignByIdAsync(designId);
            return StatusCode(response.StatusCode, response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,Tailor")]
        [HttpPost("add")]
        public async Task<IActionResult> AddDesign([FromForm] AddDesignDTO designDTO)
        {
            var design = new Design
            {
                Name = designDTO.Name,
                Description = designDTO.Description,
                Category = designDTO.Category,
                Price = designDTO.Price,
                Available = designDTO.Available,
                CreatedBy = designDTO.CreatedBy
            };

            var response = await _designService.AddDesignAsync(design, designDTO.ImageFiles);

            return StatusCode(response.StatusCode, response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,Tailor")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDesign([FromForm] UpdateDesignDto dto)
        {
            var result = await _designService.UpdateDesignAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteDesign(Guid id)
        {
            var response = await _designService.SoftDeleteDesignAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,Tailor")]
        [HttpPost("add/mesurment")]
        public async Task<IActionResult> AddMesurementGuide(Mesurment_Guides mesurment_Guides)
        {
            var result = await _designService.AddMesurementGuide(mesurment_Guides);
            return StatusCode(result.StatusCode, result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,Tailor")]
        [HttpPost("add/dis-mes")]
        public async Task<IActionResult> AddDesignMesurement(List<Design_Mesurment> disign_mesurement)
        {
            var result = await _designService.AddDesignMesurement(disign_mesurement);
            return StatusCode(result.StatusCode, result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("get/mesurments/{design_id}")]
        public async Task<IActionResult> GetMesurmentList(Guid design_id)
        {
            var result = await _designService.GetDesignMesurment(design_id);
            return StatusCode(result.StatusCode, result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpPost("add/mesurment/values")]
        public async Task<IActionResult> AddMesurmentValues(MesurementSet mesurement)
        {
            Guid user_id = Guid.Parse(HttpContext.Items["UserId"]?.ToString());
            var result = await _designService.AddMesurmentValues(mesurement,user_id);
            return StatusCode(result.StatusCode, result);   
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("get/mesurments")]
        public async Task<IActionResult> GetMesurment()
        {
            var result = await _designService.GetMesurement();
            return StatusCode(result.StatusCode, result);
        }
        
    }
}
