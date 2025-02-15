using Belvoir.DAL.Models;
using Belvoir.DAL.Models.Payments;
using Belvoir.DAL.Repositories.Payments;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Services.Payments
{
    public interface IRazorpayService
    {
        public Razorpay.Api.Order CreateOrder(decimal amount);
        public Payment FetchPayment(string paymentId);
        public bool VerifyPaymentSignature(string paymentId, string orderId, string signature, string apiSecret,Guid user_id);
    }



    public class RazorpayService : IRazorpayService
    {
        private readonly RazorpayClient _client;
        private readonly IPaymentRepository _paymentRepository;

        public RazorpayService(IConfiguration configuration,IPaymentRepository paymentRepository)
        {
            var razorpayKey = Environment.GetEnvironmentVariable("razorpayKey") ?? string.Empty;
            var razorpaySecret = Environment.GetEnvironmentVariable("razorpaySecret") ?? string.Empty;

            if (string.IsNullOrEmpty(razorpayKey) || string.IsNullOrEmpty(razorpaySecret))
            {
                throw new ArgumentException("Razorpay API credentials are missing.");
            }

            _client = new RazorpayClient(razorpayKey, razorpaySecret);
            _paymentRepository = paymentRepository; 
        }


        public Razorpay.Api.Order CreateOrder(decimal amount )
        {
            string receipt = GenerateReceipt();

            var options = new Dictionary<string, object>
        {
            { "amount", amount * 100 }, // Amount in paise (1 INR = 100 paise)
            { "currency", "INR" },
            { "receipt", receipt },
            { "payment_capture", 1 } // Auto-capture payment
        };

            return _client.Order.Create(options);
        }

        public Payment FetchPayment(string paymentId)
        {
            return _client.Payment.Fetch(paymentId);
        }

        public bool VerifyPaymentSignature(string paymentId, string orderId, string signature, string apiSecret,Guid user_id)
        {
            //var razpay = new RazorpayPayment() {PaymentId = paymentId,OrderId = orderId };
            //bool ans = _paymentRepository.AddToPaymentTable()


            string payload = $"{orderId}|{paymentId}";

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                var generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                return generatedSignature == signature;
            }
        }

        public string GenerateReceipt()
        {
            string prefix = "BELVOIR"; 
            string datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            string uniqueId = new Random().Next(100000, 999999).ToString(); // 6-digit random number

            return $"{prefix}-{datePart}-{uniqueId}";
        }


    }
}
