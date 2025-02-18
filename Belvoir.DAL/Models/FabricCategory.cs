using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Belvoir.DAL.Models
{

    public class FabricCategory
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Fabric category name is required.")]
        [StringLength(100, ErrorMessage = "Fabric category name can't exceed 100 characters.")]
        public string Name { get; set; }
    }

}
