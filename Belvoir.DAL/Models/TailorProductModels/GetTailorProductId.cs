using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.TailorProductModels
{
    public class GetTailorProductId
    {
        public Guid Id { get; set; }
        public string DesignName { get; set; }
        public string DesignImage { get; set; }
        public string ClothImage { get; set; }
        public string ClothName { get; set; }
        public string product_name { get; set; }
        public decimal price { get; set; }
    }
}
