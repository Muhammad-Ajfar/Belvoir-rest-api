﻿using Belvoir.Bll.DTO.User;
using Belvoir.Bll.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authServices.RegisterUserAsync(registerDTO);
            return StatusCode(response.StatusCode, response);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authServices.LoginAsync(loginDTO);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> GetRefreshToken(string refreshtoken)
        {
            var response = await _authServices.RefreshTokenAsync(refreshtoken);
            return StatusCode(response.StatusCode, response);
        }
    }
}