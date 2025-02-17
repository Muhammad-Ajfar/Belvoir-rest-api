using Belvoir.DAL.Models;
using Belvoir.DAL.Models.OrderGet;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.DeliveryRep
{
    public interface IDeliveryRepository
    {
        public Task<Delivery> SingleProfile(Guid id);
        public Task<DeliveryDashboard> GetDeliveryDashboard(Guid id, string? status);

    }
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly IDbConnection _dbConnection;

        public DeliveryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<Delivery> SingleProfile(Guid userid)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<Delivery>("select * from User left join DeliveryProfile on User.id=DeliveryProfile.Userid where User.id=@id", new { id = userid });

        }
        public async Task<DeliveryDashboard> GetDeliveryDashboard(Guid id,string? status)
        {
            var query = @"SELECT count(id) * 10 as totalRevenue FROM delivery_assignments WHERE status = 'delivered' AND delivery_boy_id = @del;
              SELECT count(id) as totalOrderCount FROM delivery_assignments WHERE delivery_boy_id = @del;
              SELECT count(id) as OrdersDelivered FROM delivery_assignments WHERE status = 'delivered' AND delivery_boy_id = @del;
              SELECT count(id) as OrdersAssigned FROM delivery_assignments WHERE status = 'assigned' AND delivery_boy_id = @del;";

            using (var multi = await _dbConnection.QueryMultipleAsync(query, new { del = id }))
            {
                var response = new DeliveryDashboard();

                response.totalRevenue = await multi.ReadFirstOrDefaultAsync<int>();
                response.totalOrderCount = await multi.ReadFirstOrDefaultAsync<int>();
                response.OrdersDelivered = await multi.ReadFirstOrDefaultAsync<int>();
                response.OrdersPending = await multi.ReadFirstOrDefaultAsync<int>();

                if (response == null)
                    return null; // Avoid null reference exceptions

                var orderQuery = @"SELECT da.id as order_id, ContactName as customerName, order_date, status as order_status 
                       FROM delivery_assignments da 
                       JOIN order_items oi ON da.order_id = oi.order_item_id 
                       JOIN orders os ON os.order_id = oi.order_id 
                       JOIN Address ad ON os.shipping_address = ad.Id 
                       WHERE delivery_boy_id = @delivery_id AND (status = @status OR status IS NULL)";

                response.DeliveryOrders = (await _dbConnection.QueryAsync<OrderDeliveryGet>(orderQuery, new { delivery_id = id })).ToList();

                return response;
            }

        }

    }
}
