using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.OrderGet
{
    public class OrderUserRentalGet
    {
        public Guid order_id { get; set; }
        public string RentalImage { get; set; }
        public DateTime order_date { get; set; }
        public string order_status { get; set; }
        public string price { get; set; }
        public string Title { get; set; }
    }
}
