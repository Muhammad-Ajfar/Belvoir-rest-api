using Belvoir.Bll.Services.Payments;
using Belvoir.DAL.Models.Payments;
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

            var apiSecret = configuration["Razorpay:ApiSecret"];
        }



        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] decimal amount)
        {
            var order = _razorpayService.CreateOrder(amount);

            return Ok(new orderDetails
            {
                orderId = order["id"],
                amount = order["amount"],
                currency = order["currency"],
                Link = "file:///C:/Users/nabee/OneDrive/Desktop/index.html"
            });


        }

        [HttpPost("verify-payment")]
        public IActionResult VerifyPayments([FromBody] PaymentVerificationRequest request)
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());


            bool isValid = _razorpayService.VerifyPaymentSignature(
                request.PaymentId,
                request.OrderId,
                request.Signature,
                "7h2qnyXl8f91qZN5cAZ2G4cr",
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
