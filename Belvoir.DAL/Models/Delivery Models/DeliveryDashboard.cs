using Belvoir.DAL.Models.OrderGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class DeliveryDashboard
    {
        public decimal totalRevenue {  get; set; }
        public int totalOrderCount {  get; set; }
        public int OrdersDelivered {  get; set; }
        public int OrdersPending {  get; set; }
        public IEnumerable<OrderDeliveryGet>? DeliveryOrders { get; set; }
    }
}
