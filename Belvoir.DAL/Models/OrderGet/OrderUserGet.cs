using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.NewFolder
{
    public class OrderUserGet
    {
        public Guid order_id { get; set; }
        public string clothImage { get; set; }
        public string designImage { get; set; }
        public DateTime order_date { get; set; }
        public string order_status { get; set; }
        public string price { get; set; }
        public string product_name { get; set; }

    }
}
