using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class Order
    {
        public Guid? Id { get; set; }
        public Guid? userId { get; set; }
        public decimal totalAmount { get; set; }
        public string paymentMethod { get; set; }
        public Guid shippingAddress { get; set; }
        public bool FastShipping { get; set; }
        public decimal shippingCost { get; set; }
        public string trackingNumber { get; set; }
        public string productType { get; set; }
        public Guid? tailorProductId { get; set; }
        public Guid? rentalProductId { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }

    }
}
