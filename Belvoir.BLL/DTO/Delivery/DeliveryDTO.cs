using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Belvoir.Bll.DTO.Delivery
{

    public class DeliveryDTO
    {

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name can't exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must be a 10-digit Indian number starting with 6-9.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Licence number is required.")]
        [StringLength(20, ErrorMessage = "Licence number can't exceed 20 characters.")]
        public string LicenceNo { get; set; }

        [Required(ErrorMessage = "Vehicle number is required.")]
        [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z]{1,2}\d{4}$", ErrorMessage = "Invalid Indian vehicle number format (e.g., MH12AB1234 or DL04Z5678).")]
        public string VehicleNo { get; set; }
    }

}
