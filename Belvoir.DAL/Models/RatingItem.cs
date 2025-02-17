using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class RatingItem
    {
        [StringLength(500, ErrorMessage = "Message can't exceed 500 characters.")]
        public string message { get; set; }

        [Range(1, 5, ErrorMessage = "Rating value must be between 1 and 5.")]
        public decimal ratingvalue { get; set; }
    }
}
