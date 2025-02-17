using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.TailorProduct
{
    public class GetTailorProductUser
    {
        public Guid Id { get; set; }
        public string DesignName { get; set; }
        public string ClothName { get; set; }
        public string product_name { get; set; }
        public decimal price { get; set; }
    }
}
