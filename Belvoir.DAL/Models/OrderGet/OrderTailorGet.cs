using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.NewFolder
{
    public class OrderTailorGet
    {
        public Guid order_id { get; set; }
        public string customerName { get; set; }
        public DateTime order_date { get; set; }
        public string  clothTitle { get; set; }
        public string  DesignName { get; set; }
        public string order_status { get; set; }
        public DateTime deadline { get; set; } 

    }
}
