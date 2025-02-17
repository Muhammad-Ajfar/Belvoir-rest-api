﻿using Belvoir.Bll.Services.DeliverySer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Delivery
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private IDeliveryServices _service;
        public DeliveryController(IDeliveryServices services)
        {
            _service = services;
        }

        [Authorize(Roles = "Delivery")]
        [HttpGet("profile-delivery")]
        public async Task<IActionResult> GetDeliveryProfile()
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _service.GetDeliveryProfile(userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Delivery")]
        [HttpGet("dashboard")]
        public async  Task<IActionResult> GetDeliveryDashboard(string? status) {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _service.GetDeliveryDashboard(userId, status);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Delivery")]
        [HttpPatch("change/status")]
        public async Task<IActionResult> ChangeStatus(Guid id,string status)
        {
            var response = await _service.ChangeStatus(id, status);
            return StatusCode(response.StatusCode, response);
        }
    }
}
