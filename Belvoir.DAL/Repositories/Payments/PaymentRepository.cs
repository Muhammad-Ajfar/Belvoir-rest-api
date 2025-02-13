using Belvoir.DAL.Models.Payments;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Payments
{
    public interface IPaymentRepository
    {
        public Task<bool> AddToPaymentTable(RazorpayPayment payment, Guid user_id);
    }
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnection _dbConnection;
        public PaymentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> AddToPaymentTable(RazorpayPayment payment,Guid user_id)
        {
            string query = @"INSERT INTO `belvoir`.`razorpay_payments`(`id`,`payment_id`,`order_id`,`user_id`,`amount`,`currency`,`status`,`method`,`description`) VALUES (UUID(),@payment_id,@order_id ,@user_id,@amount,@currency,@status,@method,@description);";
            return await _dbConnection.ExecuteAsync(query, new { payment_id = payment.PaymentId, order_id = payment.OrderId, user_id = user_id, amount = payment.Amount, currency = payment.Currency, status = payment.Status, method = payment.Method, description = payment.Description })>0;
        }
    }
}
