using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.OrderGet
{
    public class OrderDeliveryGet
    {
        public Guid order_id { get; set; }
        public string customerName { get; set; }
        public DateTime order_date { get; set; }
        public string order_status { get; set; }
        public DateTime deadline { get; set; }
    }
}
