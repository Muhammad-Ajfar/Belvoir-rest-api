using System.ComponentModel.DataAnnotations;

namespace Belvoir.Bll.DTO.Tailor
{
    public class PasswordResetDTO
    {
        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; }

    }
}
