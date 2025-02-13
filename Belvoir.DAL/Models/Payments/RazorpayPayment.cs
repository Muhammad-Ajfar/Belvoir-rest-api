using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.Payments
{
    public class RazorpayPayment
    {
        public string PaymentId { get; set; }  // Razorpay Payment ID
        public string OrderId { get; set; }  // Razorpay Order ID
       
        public decimal Amount { get; set; }  // Payment amount
        public string Currency { get; set; } = "INR";  // Default currency
        public string Status { get; set; }  // Payment status (ENUM stored as string)
        public string Method { get; set; }  // Payment method (e.g., card, UPI)
        public string Description { get; set; }  // Payment description
    }

}
