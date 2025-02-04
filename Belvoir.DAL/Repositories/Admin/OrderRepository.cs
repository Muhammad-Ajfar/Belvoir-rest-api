﻿using Belvoir.DAL.Models;
using Belvoir.DAL.Models.NewFolder;
using Belvoir.DAL.Models.OrderGet;
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
        public Task<bool> AddTailorProduct(TailorProduct tailorProduct);
        public Task<bool> IsClothExists(Guid Id);
        public Task<bool> IsDesignExists(Guid Id);
        public Task<bool> AddOrder(Order order);
        public Task<IEnumerable<OrderTailorGet>> orderTailorGets();
        public Task<IEnumerable<OrderAdminGet>> orderAdminGets(string? status);
        public Task<IEnumerable<OrderDeliveryGet>> orderDeliveryGets();
        public Task<IEnumerable<OrderUserGet>> orderUserGets(Guid userid, string? status);
    }
    public class OrderRepository: IOrderRepository
    {
        private readonly IDbConnection _dbConnection;
        public OrderRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> AddTailorProduct(TailorProduct tailorProduct)
        {
            string query = "INSERT INTO `belvoir`.`tailor_products` (`product_id`,`customer_id`,`design_id`,`cloth_id`,`product_name`,`price`) VALUES (UUID(),'918eab05-0a0b-42a9-9ce6-2cc973c9eb3a',@designid,@clothid,@productname,@price)";
            return await _dbConnection.ExecuteAsync(query, new { clothid = tailorProduct.ClothId, userid = tailorProduct.UserId,designid = tailorProduct.DesignId,productname = tailorProduct.product_name,price = tailorProduct.price })>0;
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
        public async Task<bool> AddOrder(Order order)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_customer_id", "918eab05-0a0b-42a9-9ce6-2cc973c9eb3a");
            parameters.Add("@p_order_date", order.orderDate);
            parameters.Add("@p_total_amount", order.totalAmount);
            parameters.Add("@p_payment_method", order.paymentMethod);
            parameters.Add("@p_shipping_address", order.shippingAddress);
            parameters.Add("@p_shipping_method", order.shippingMethod);
            parameters.Add("@p_shipping_cost", order.shippingCost);
            parameters.Add("@p_tracking_number", order.trackingNumber);
            parameters.Add("@p_updated_by", order.updatedBy);
            parameters.Add("@p_product_type", order.productType);
            parameters.Add("@p_tailor_product_id", order.tailorProductId);
            parameters.Add("@p_rental_product_id", order.rentalProductId);
            parameters.Add("@p_quantity", order.quantity);
            parameters.Add("@p_price", order.price);

            return await _dbConnection.ExecuteAsync("InsertOrderWithItems", parameters, commandType: CommandType.StoredProcedure)>0;

        }
        public async Task<IEnumerable<OrderTailorGet>> orderTailorGets()
        {
            string query = "SELECT orders.order_id,User.Name as customerName,order_date,order_status,Cloths.Title as clothTitle,DressDesign.Name as DesignName FROM orders join order_items on orders.order_id = order_items.order_id join User on User.Id = orders.customer_id join tailor_products ON tailor_products.product_id = order_items.tailor_product_id join Cloths on Cloths.Id = tailor_products.cloth_id join DressDesign on DressDesign.Id = tailor_products.design_id where order_status ='pending';";
            return await _dbConnection.QueryAsync<OrderTailorGet>(query);
        }
        public async Task<IEnumerable<OrderAdminGet>> orderAdminGets(string? status)
        {
            string spname = "GetOrdersByStatus";
            var parameters = new DynamicParameters();
            parameters.Add("@p_order_status", status);
            return await _dbConnection.QueryAsync<OrderAdminGet>(spname,parameters,commandType:CommandType.StoredProcedure);
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


    }
}
