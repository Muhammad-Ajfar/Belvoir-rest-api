using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Order
{
    public class PlaceOrderDTO
    {
        [Required(ErrorMessage = "Payment method is required.")]
        [RegularExpression("^(RazorPay|CashOnDelivery)$", ErrorMessage = "Invalid payment method.")]
        public string paymentMethod { get; set; }

        [Required(ErrorMessage = "Shipping address is required.")]
        public Guid shippingAddress { get; set; }

        public bool FastShipping { get; set; }

        [Required(ErrorMessage = "Product type is required.")]
        [RegularExpression("^(tailor|rental)$", ErrorMessage = "Invalid product type.")]
        public string productType { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        public Guid productId { get; set; }

        public Guid? SetId { get; set; }

        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10.")]
        public int quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal price { get; set; }
    }
}
