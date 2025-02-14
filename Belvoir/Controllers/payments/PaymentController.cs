using Belvoir.Bll.Services.Payments;
using Belvoir.DAL.Models.Payments;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.payments
{
    [Route("api/[controller]")]
    [ApiController]
    public class paymentController : ControllerBase
    {
        private readonly IRazorpayService _razorpayService;

        public paymentController(IRazorpayService razorpayService, IConfiguration configuration)
        {
            _razorpayService = razorpayService;

            var apiSecret =  Environment.GetEnvironmentVariable("razorpaySecret") ?? string.Empty;
        }



        [HttpPost("create-order")]
        public IActionResult CreateOrder(decimal amount)
        {
            var order = _razorpayService.CreateOrder(amount);
            var razorpayLink = Environment.GetEnvironmentVariable("razorpayLink") ?? string.Empty;
            return Ok(new orderDetails
            {
                orderId = order["id"],
                amount = order["amount"],
                currency = order["currency"],
                Link = razorpayLink
            });


        }

        [HttpPost("verify-payment")]
        public IActionResult VerifyPayments([FromBody] PaymentVerificationRequest request)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var razorpaySecret = Environment.GetEnvironmentVariable("razorpaySecret") ?? string.Empty;

            bool isValid = _razorpayService.VerifyPaymentSignature(
                request.PaymentId,
                request.OrderId,
                request.Signature,
                razorpaySecret,
                userId
            );

            if (isValid)
            {
                return Ok(new { Status = "Payment verified successfully" });
            }
            else
            {
                return BadRequest(new { Status = "Verification failed", Error = "Invalid signature" });
            }
        }


    }
}
