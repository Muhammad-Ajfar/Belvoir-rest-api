using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.OrderGet
{
    public class SingleOrderRentals
    {
        public Guid order_item_id { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string productImage { get; set; }
        public string productTitle { get; set; }
        public DateTime returnDate { get; set; }
        public DateTime order_date { get; set; }
        public string order_status { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
    }
}
