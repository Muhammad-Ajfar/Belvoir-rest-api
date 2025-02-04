using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.Query
{
    public class RentalQuery
    {
        public string? searchName {  get; set; }
        public Guid? fabric_type { get; set; }
        public string? gender {  get; set; }
        public string? garmentType {  get; set; }
        public int? minPrice { get; set; }
        public int? maxPrice { get; set; }
        public int? pageSize { get; set; }
        public int? pageNo { get; set; }

    }
}
