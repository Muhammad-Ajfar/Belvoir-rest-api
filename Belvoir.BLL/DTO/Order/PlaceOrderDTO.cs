using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Order
{
    public class PlaceOrderDTO
    {
        public string paymentMethod { get; set; }
        public Guid shippingAddress { get; set; }
        public bool fastShipping { get; set; }
        public string productType { get; set; }
        public Guid productId { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
    }
}
