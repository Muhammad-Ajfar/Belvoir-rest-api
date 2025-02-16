using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Design
{
    public class UpdateDesignDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public List<IFormFile>? NewImages { get; set; }  // New images to be added
        public List<Guid>? RemoveImageIds { get; set; }  // Existing images to be removed
    }

}
