using System.ComponentModel.DataAnnotations;

namespace Belvoir.Bll.DTO.User
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name can't exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must be a 10-digit Indian number starting with 6-9.")]
        public string Phone { get; set; }
    }
}
