using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Belvoir.DAL.Models
{


    public class User
    {

        public Guid Id { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public string Name { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string PasswordHash { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }
        public string Role { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }

}
