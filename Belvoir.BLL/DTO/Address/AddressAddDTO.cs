using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Belvoir.Bll.DTO.Address
{


    public class AddressAddDTO
    {
        [Required(ErrorMessage = "Street is required.")]
        [StringLength(100, ErrorMessage = "Street name can't exceed 100 characters.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City name can't exceed 50 characters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, ErrorMessage = "State name can't exceed 50 characters.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal Code is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Postal Code must be exactly 6 digits.")]
        public string PostalCode { get; set; }

        [StringLength(100, ErrorMessage = "Building name can't exceed 100 characters.")]
        public string BuildingName { get; set; }

        [Required(ErrorMessage = "Contact Name is required.")]
        [StringLength(50, ErrorMessage = "Contact name can't exceed 50 characters.")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must be a 10-digit Indian number starting with 6-9.")]
        public string ContactNumber { get; set; }
    }

}
