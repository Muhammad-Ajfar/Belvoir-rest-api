using Belvoir.DAL.Models;
using Belvoir.DAL.Models.NewFolder;
using Belvoir.DAL.Models.OrderGet;
using Belvoir.DAL.Models.TailorProduct;
using Belvoir.DAL.Models.TailorProductModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Admin
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<GetTailorProductUser>> GetAllTailorProducts(Guid user_id);
        public Task<bool> AddTailorProduct(TailorProductAdd tailorProduct,Guid id,Guid user_id);
        public Task<GetTailorProductId> TailorProductById(Guid product_id, Guid user_id);
        public Task<bool> RemoveTailorProduct(Guid product_id, Guid user_id );
        public Task<bool> IsClothExists(Guid Id);   
        public Task<bool> IsDesignExists(Guid Id);
        public Task<bool> IsAddressExists(Guid Id);
        public Task<bool> AddOrder(Order order );
        public Task<int> CheckoutRentalCartAsync(Guid userId, string paymentMethod, Guid shippingAddress, bool fastShipping, string trackingNumber);

        public Task<IEnumerable<OrderTailorGet>> orderTailorGets();
        public Task<IEnumerable<AdminTailorOrderGet>> AdminGetTailorOrder(string? status);
        public Task<IEnumerable<OrderDeliveryGet>> orderDeliveryGets();
        public Task<IEnumerable<OrderUserGet>> orderUserGets(Guid userid, string? status);
        public Task<IEnumerable<OrderUserRentalGet>> orderRentalUserGets(Guid userid, string? status);
        public Task<SingleOrderTailoring> SingleOrder(Guid order_id);
        public Task<bool> UpdateStatus(Guid order_id, string status);
    }
    public class OrderRepository: IOrderRepository
    {
        private readonly IDbConnection _dbConnection;
        public OrderRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<GetTailorProductUser>> GetAllTailorProducts(Guid user_id)
        {
            string query = @"SELECT 
                                tailor_products.product_id,
	                            Name as DesignName,
                                Title as ClothName,
                                product_name,
                                tailor_products.price
                            FROM tailor_products 
                            JOIN Cloths ON tailor_products.cloth_id = Cloths.Id
                            JOIN DressDesign ON tailor_products.design_id = DressDesign.Id
                            WHERE customer_id = @id AND tailor_products.IsDeleted = false";
            return await _dbConnection.QueryAsync<GetTailorProductUser>(query, new { id = user_id });
        }
        public async Task<bool> AddTailorProduct(TailorProductAdd tailorProduct,Guid id, Guid user_id)
        {

            string query = "INSERT INTO `belvoir`.`tailor_products` (`product_id`,`customer_id`,`design_id`,`cloth_id`,`product_name`,`price`) VALUES (@id,@user_id,@designid,@clothid,@productname,@price)";
            return await _dbConnection.ExecuteAsync(query, new {id = id, clothid = tailorProduct.ClothId, user_id = user_id,designid = tailorProduct.DesignId,productname = tailorProduct.product_name,price = tailorProduct.price })>0;

        }
        public async Task<bool> RemoveTailorProduct(Guid product_id,Guid user_id)
        {
            string query = @"UPDATE tailor_products SET IsDeleted = true WHERE product_id = @id AND IsDeleted = false AND customer_id = @user_id";
            return await _dbConnection.ExecuteAsync(query, new { id = product_id , user_id = user_id})>0;
        }
        public async Task<GetTailorProductId> TailorProductById(Guid product_id,Guid user_id)
        {
            string query = @"SELECT 
                                tailor_products.product_id,
	                            Name as DesignName,
                                Cloths.ImageUrl as ClothImage,
                                DesignImages.ImageUrl as DesignImage,
                                Title as ClothName,
                                product_name,
                                tailor_products.price
                            FROM tailor_products 
                            JOIN Cloths ON tailor_products.cloth_id = Cloths.Id
                            JOIN DressDesign ON tailor_products.design_id = DressDesign.Id
                            JOIN DesignImages ON DesignImages.DesignId = DressDesign.Id
                            WHERE product_id = @id AND IsDeleted = false AND customer_id = @user_id";
            return await _dbConnection.QueryFirstOrDefaultAsync<GetTailorProductId>(query, new { id = product_id ,user_id  =user_id}) ;
        }
        public async Task<bool> IsClothExists(Guid Id)
        {
            string query = "SELECT Count(*) FROM Cloths WHERE Id = @Id";
            return await _dbConnection.ExecuteScalarAsync<int>(query, new { Id }) > 0;
        }
        public async Task<bool> IsDesignExists(Guid Id)
        {
            string query = "SELECT Count(*) FROM DressDesign WHERE Id = @Id";
            return await _dbConnection.ExecuteScalarAsync<int>(query, new { Id }) > 0;
        }
        public async Task<bool> IsAddressExists(Guid Id)
        {
            string query = "SELECT Count(*) FROM Address WHERE Id = @Id";
            return await _dbConnection.ExecuteScalarAsync<int>(query, new { Id }) > 0;
        }
        public async Task<bool> AddOrder(Order order)
        {
                var parameters = new DynamicParameters();
                parameters.Add("@p_customer_id", order.userId);
                parameters.Add("@p_total_amount", order.totalAmount);
                parameters.Add("@p_payment_method", order.paymentMethod);
                parameters.Add("@p_shipping_address", order.shippingAddress);
                parameters.Add("@p_fast_shipping", order.FastShipping);
                parameters.Add("@p_shipping_cost", order.shippingCost);
                parameters.Add("@p_tracking_number", order.trackingNumber);
                parameters.Add("@p_product_type", order.productType);
                parameters.Add("@p_tailor_product_id", order.tailorProductId);
                parameters.Add("@p_rental_product_id", order.rentalProductId);
                parameters.Add("@p_quantity", order.quantity);
                parameters.Add("@p_price", order.price);
                parameters.Add("@p_success", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync("InsertOrderWithItems", parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@p_success") == 1;
        }

        public async Task<int> CheckoutRentalCartAsync(Guid userId, string paymentMethod, Guid shippingAddress, bool fastShipping, string trackingNumber)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_user_id", userId);
                parameters.Add("@p_payment_method", paymentMethod);
                parameters.Add("@p_shipping_address", shippingAddress);
                parameters.Add("@p_fast_shipping", fastShipping);
                parameters.Add("@p_tracking_number", trackingNumber);
                parameters.Add("@p_total_amount", dbType: DbType.Int32, direction: ParameterDirection.Output);


                await _dbConnection.ExecuteAsync("CheckoutRentalCart", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("@p_total_amount");

            }
            catch (Exception ex)
            {
                throw new Exception("Error processing rental cart checkout", ex);
            }
        }


        public async Task<IEnumerable<OrderTailorGet>> orderTailorGets()
        {
            string query = "SELECT orders.order_id,User.Name as customerName,order_date,order_status,Cloths.Title as clothTitle,DressDesign.Name as DesignName FROM orders join order_items on orders.order_id = order_items.order_id join User on User.Id = orders.customer_id join tailor_products ON tailor_products.product_id = order_items.tailor_product_id join Cloths on Cloths.Id = tailor_products.cloth_id join DressDesign on DressDesign.Id = tailor_products.design_id where order_status ='pending';";
            return await _dbConnection.QueryAsync<OrderTailorGet>(query);
        }
        public async Task<IEnumerable<AdminTailorOrderGet>> AdminGetTailorOrder(string? status)
        {
            string spname = "GetTailorOrdersByStatus";
            var parameters = new DynamicParameters();
            parameters.Add("@p_order_status", status);
            return await _dbConnection.QueryAsync<AdminTailorOrderGet>(spname,parameters,commandType:CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<OrderDeliveryGet>> orderDeliveryGets()
        {
            string query = "SELECT orders.order_id,Name as customerName,order_date,order_status FROM orders join order_items on orders.order_id = order_items.order_id join User on User.Id = orders.customer_id  AND order_status ='out for delivery';";
            return await _dbConnection.QueryAsync<OrderDeliveryGet>(query);
        }
        public async Task<IEnumerable<OrderUserGet>> orderUserGets(Guid userid,string? status)
        {
            string spName = "orderForUser";
            var parameters = new DynamicParameters();
            parameters.Add("@p_order_status", status);
            parameters.Add("@p_user_id", userid);
            return await _dbConnection.QueryAsync<OrderUserGet>(spName,parameters,commandType:CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<OrderUserRentalGet>> orderRentalUserGets(Guid userid, string? status)
        {
            string query = @"SELECT 
	                            orders.order_id,
	                            RentalImage.Imagepath as RentalImage,
	                            orders.order_date,
	                            order_status,
	                            order_items.price,
	                            RentalProduct.Title,
	                            order_items.quantity
                            FROM orders
                            JOIN order_items ON orders.order_id = order_items.order_id
                            JOIN RentalProduct ON RentalProduct.Id = order_items.rental_product_id
                            JOIN RentalImage ON RentalImage.productid = RentalProduct.Id
                            WHERE orders.customer_id =  '2be0b7f6-6bcc-4310-8db6-f5e8f4183690'
                            AND order_items.product_type = 'rental'
                            AND RentalImage.IsPrimary = true 
                            AND ( @order_status IS NULL OR order_items.order_status = @order_status)
                            ORDER BY orders.order_date DESC;";
            
            return await _dbConnection.QueryAsync<OrderUserRentalGet>(query,new {user_id = userid,order_status = status});
        }
        public async Task<SingleOrderTailoring> SingleOrder(Guid order_id)
        {
            string query = @"
                            SELECT 
                                order_items.order_item_id,
                                Address.ContactName,
                                Address.ContactNumber,
                                CONCAT(Address.BuildingName,', ',Address.Street,', ',Address.City,', ',Address.State,', ',Address.PostalCode) as address,
                                Cloths.ImageUrl as clothImage,
                                Cloths.Title as clothTitle,
                                DesignImages.ImageUrl as designImage,
                                DressDesign.Name as designTitle,
                                orders.order_date,
                                order_status,
                                order_items.price,
                                tailor_products.product_name,
                                order_items.quantity
                            FROM orders
                            JOIN order_items ON orders.order_id = order_items.order_id
                            JOIN tailor_products ON tailor_products.product_id = order_items.tailor_product_id
                            JOIN DressDesign ON DressDesign.Id = tailor_products.design_id
                            JOIN DesignImages ON DressDesign.Id = DesignImages.DesignId
                            JOIN Cloths ON  Cloths.Id = tailor_products.cloth_id
                            JOIN Address ON orders.shipping_address = Address.Id
                            WHERE product_type = 'tailor'
                            AND DesignImages.IsPrimary = true 
                            AND order_items.order_item_id = @order_id;";
            return await _dbConnection.QueryFirstOrDefaultAsync<SingleOrderTailoring>(query, new { order_id = order_id });
        }

        
        public async Task<bool> UpdateStatus(Guid order_id,string status)
        {
            string query = @"UPDATE order_items SET order_status = @NewStatus WHERE order_item_id = @OrderId;";
            return await _dbConnection.ExecuteAsync(query, new { OrderId = order_id, NewStatus = status }) > 0;
        }
        
    }
}
