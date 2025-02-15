using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Order
{
    public class TailorProductDTO
    {
        [Required(ErrorMessage = "Design ID is required.")]
        public Guid DesignId { get; set; }

        [Required(ErrorMessage = "Cloth ID is required.")]
        public Guid ClothId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name can't exceed 100 characters.")]
        public string product_name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal price { get; set; }
    }
}
