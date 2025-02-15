using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Order
{
    public class CheckoutRentalCartDTO
    {
        public string PaymentMethod { get; set; } // "Credit Card", "PayPal", "Cash"
        public Guid ShippingAddress { get; set; }
        public bool FastShipping { get; set; }
    }

}
