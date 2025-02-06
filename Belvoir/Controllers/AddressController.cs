using AutoMapper;
using Belvoir.Bll.DTO.Address;
using Belvoir.Bll.Services;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly IMapper _mapper;

        public AddressController(IAddressService addressService, IMapper mapper)
        {
            _addressService = addressService;
            _mapper = mapper;
        }

        // Get Addresses by User
        [HttpGet("user")]
        public async Task<IActionResult> GetAddressesByUser()
        {
            var userId = Guid.Parse(HttpContext.Items["UserId"].ToString()); // Fetch userId from HttpContext
            var response = await _addressService.GetAddressesByUser(userId);
            if (response.StatusCode == 404)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        // Add Address
        [HttpPost("Add")]
        public async Task<IActionResult> AddAddress([FromBody] AddressAddDTO addressAddDto)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString()); // Fetch userId from HttpContext

            var response = await _addressService.AddAddress(userId, addressAddDto);
            if (response.StatusCode == 201)
            {
                return CreatedAtAction(nameof(GetAddressesByUser), new { userId = userId }, response);
            }

            return BadRequest(response);
        }

        // Update Address
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] AddressAddDTO addressDto)
        {
            var address = _mapper.Map<Address>(addressDto);
            address.Id = id;


            var response = await _addressService.UpdateAddress(address);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Soft Delete Address
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAddress(Guid id)
        {
            var response = await _addressService.SoftDeleteAddress(id);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }

            return NotFound(response);
        }
    }

}
