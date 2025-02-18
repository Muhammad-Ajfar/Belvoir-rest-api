using AutoMapper;
using Belvoir.Bll.DTO.Order;
using Belvoir.DAL.Models;
using Belvoir.DAL.Models.NewFolder;
using Belvoir.DAL.Models.OrderGet;
using Belvoir.DAL.Models.TailorProduct;
using Belvoir.DAL.Models.TailorProductModels;
using Belvoir.DAL.Repositories.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Services.Admin
{
    public interface IOrderServices
    {
        public Task<Response<IEnumerable<GetTailorProductUser>>> GetAllTailorProducts(Guid user_id);
        public Task<Response<GetTailorProductId>> TailorProductsById(Guid product_id, Guid user_id);
        public Task<Response<object>> AddTailorProducts(TailorProductDTO tailorProductDTO,Guid user_id);
        public Task<Response<object>> RemoveTailorProduct(Guid product_id, Guid user_id);
        public Task<Response<object>> AddOrder(PlaceOrderDTO orderDto, Guid user_id);
        public Task<Response<int>> CheckoutRentalCartAsync(Guid userId, CheckoutRentalCartDTO checkoutDto);
        public Task<Response<IEnumerable<AdminTailorOrderGet>>> AdminGetTailorOrder(Guid? userId, string? status);
        public Task<Response<IEnumerable<AdminRentalOrderGet>>> AdminGetRentalOrder(Guid? userId, string? status);

        public Task<Response<IEnumerable<OrderUserGet>>> orderUserGets(Guid userid, string? status);
        public Task<Response<IEnumerable<OrderUserRentalGet>>> orderRentalUserGets(Guid userid, string? status);
        public Task<Response<IEnumerable<OrderDeliveryGet>>> orderDeliveryGets();
        public Task<Response<IEnumerable<OrderTailorGet>>> orderTailorGets();
        public Task<Response<SingleOrderTailoring>> SingleOrder(Guid order_id);
        public Task<Response<object>> OrderStatusUpdate(Guid order_id, string status);
    }
    public class OrderServices:IOrderServices
    {
        private readonly IOrderRepository _repo;
        private readonly IMapper _mapper;
        public OrderServices(IOrderRepository repo,IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<IEnumerable<GetTailorProductUser>>> GetAllTailorProducts(Guid user_id)
        {
            var res = await _repo.GetAllTailorProducts(user_id);
            if(res == null)
            {
                return new Response<IEnumerable<GetTailorProductUser>> { StatusCode = 200, Message = "No products to show" };
            }
            return new Response<IEnumerable<GetTailorProductUser>> { StatusCode = 200, Message = "Success", Data = res };
        }
        public async Task<Response<GetTailorProductId>> TailorProductsById(Guid product_id,Guid user_id)
        {
            var res = await _repo.TailorProductById(product_id,user_id);
            if (res == null)
            {
                return new Response<GetTailorProductId> { StatusCode = 200, Message = "No products to show" };
            }
            return new Response<GetTailorProductId> { StatusCode = 200, Message = "Success", Data = res };
        }
        public async Task<Response<object>> AddTailorProducts(TailorProductDTO tailorProductDTO, Guid user_id) {

            Guid id = Guid.NewGuid();
            var tailorProduct = _mapper.Map<TailorProductAdd>(tailorProductDTO);
            if (!await _repo.IsClothExists(tailorProduct.ClothId))
            {
                return new Response<object> { StatusCode = 404, Message = "cloth not exist" };
            }
            if (!await _repo.IsDesignExists(tailorProduct.DesignId))
            {
                return new Response<object> { StatusCode = 404, Message = "DressDesign not exist" };
            }
            if (await _repo.AddTailorProduct(tailorProduct,id, user_id))
            {
                return new Response<object> { StatusCode = 201, Message = "success" ,Data = new  {tailor_product_id = id } };
            }
            return new Response<object> { StatusCode = 500, Message = "failed" };
        }
        public async Task<Response<object>> RemoveTailorProduct(Guid product_id,Guid user_id)
        {
            var res = await _repo.TailorProductById(product_id, user_id);
            if (res == null) { return new Response<object> { StatusCode = 404, Message = "Not Found" }; }
            bool ans = await _repo.RemoveTailorProduct(product_id,user_id);
            if (ans)
            {
                return new Response<object> { StatusCode = 200, Message = "Removed Successfully" };
            }
            return new Response<object> { StatusCode = 500, Message = "internal server error" };
        }
        private string GenerateFedExTrackingNumber()
        {
            Random random = new Random();
            return string.Concat(Enumerable.Range(0, 12).Select(_ => random.Next(0, 10).ToString()));
        }
        public async Task<Response<object>> AddOrder(PlaceOrderDTO orderDto, Guid userId)
        {
            var addressCheck = await _repo.IsAddressExists(orderDto.shippingAddress);
            var order = _mapper.Map<Order>(orderDto);
            order.shippingCost = orderDto.FastShipping? 40 : 100;
            order.trackingNumber = GenerateFedExTrackingNumber(); // Generate tracking number
            order.userId = userId;
            order.totalAmount = orderDto.price + order.shippingCost;
            if (orderDto.productType == "tailor")
            {
                order.tailorProductId = orderDto.productId;
                if(orderDto.SetId == null || orderDto.SetId == Guid.Empty)
                {
                    return new Response<object>
                    {
                        StatusCode = 404,
                        Message = "A Measurement set is needed for tailor product",
                        Error = "Set id is null or empty"
                    };
                }
                order.SetId = orderDto.SetId;
            }
            else
            {
                order.rentalProductId = orderDto.productId;
            }

            if (await _repo.AddOrder(order))
            {
                return new Response<object> { StatusCode = 201, Message = "success" };
            }
            return new Response<object> { StatusCode = 500, Message = "error" };
        }
        public async Task<Response<int>> CheckoutRentalCartAsync(Guid userId, CheckoutRentalCartDTO checkoutDto)
        {
            string trackingNumber = GenerateFedExTrackingNumber();

            int totalAmount = await _repo.CheckoutRentalCartAsync(userId, checkoutDto.PaymentMethod,
                                    checkoutDto.ShippingAddress, checkoutDto.FastShipping, trackingNumber);

            if (totalAmount != -1)
            {
                return new Response<int> { StatusCode = 200, Message = "Checkout successful", Data = totalAmount };
            }
            return new Response<int> { StatusCode = 500, Message = "Checkout failed", Error = "Database operation failed" };
        }
        public async Task<Response<IEnumerable<OrderUserGet>>> orderUserGets(Guid userid, string? status)
        {
            var result = await _repo.orderUserGets(userid, status);
            if (result == null)
            {
                return new Response<IEnumerable<OrderUserGet>> { StatusCode = 200, Message = "No Orders"};
            }
            return new Response<IEnumerable<OrderUserGet>> { StatusCode = 200, Message = "success", Data = result };
        }
        public async Task<Response<IEnumerable<OrderUserRentalGet>>> orderRentalUserGets(Guid userid, string? status)
        {
            var result = await _repo.orderRentalUserGets(userid, status);
            if (result == null)
            {
                return new Response<IEnumerable<OrderUserRentalGet>> { StatusCode = 200, Message = "No Orders" };
            }
            return new Response<IEnumerable<OrderUserRentalGet>> { StatusCode = 200, Message = "success", Data = result };
        }
        public async Task<Response<IEnumerable<OrderTailorGet>>> orderTailorGets()
        {
            var result = await _repo.orderTailorGets();
            if (result == null)
            {
                return new Response<IEnumerable<OrderTailorGet>> { StatusCode = 404, Message = " no orders" };
            }
            foreach (var item in result)
            {
                item.deadline = item.order_date.AddDays(3);
            }
            return new Response<IEnumerable<OrderTailorGet>> { StatusCode = 200, Message = "success", Data = result };
        }
        public async Task<Response<IEnumerable<OrderDeliveryGet>>> orderDeliveryGets()
        {
            var result = await _repo.orderDeliveryGets(); 
            if (result == null)
            {
                return new Response<IEnumerable<OrderDeliveryGet>> { StatusCode = 404, Message = " no orders" };
            }

            return new Response<IEnumerable<OrderDeliveryGet>> { StatusCode = 200, Message = "success", Data = result };
        }

        public async Task<Response<IEnumerable<AdminTailorOrderGet>>> AdminGetTailorOrder(Guid? userId, string? status)
        {
            var result = await _repo.AdminGetTailorOrder(userId, status);
            if (result == null)
            {
                return new Response<IEnumerable<AdminTailorOrderGet>> { StatusCode = 404, Message = " no orders" };
            }
            return new Response<IEnumerable<AdminTailorOrderGet>> { StatusCode = 200, Message = "success", Data = result };
        }

        public async Task<Response<IEnumerable<AdminRentalOrderGet>>> AdminGetRentalOrder(Guid? userId, string? status)
        {
            var result = await _repo.AdminGetRentalOrder(userId, status);
            if (result == null)
            {
                return new Response<IEnumerable<AdminRentalOrderGet>> { StatusCode = 404, Message = " no orders" };
            }
            return new Response<IEnumerable<AdminRentalOrderGet>> { StatusCode = 200, Message = "success", Data = result };
        }

        public async Task<Response<SingleOrderTailoring>> SingleOrder(Guid order_id)
        {
            var result = await _repo.SingleOrder(order_id);
            if (result == null)
            {
                return new Response<SingleOrderTailoring> { StatusCode = 404, Message = "Order not found" };
            }
            return new Response<SingleOrderTailoring> { StatusCode = 200, Message = "success", Data = result };
        }
        public async Task<Response<object>> OrderStatusUpdate(Guid order_id, string status)
        {
            bool res = await _repo.UpdateStatus(order_id, status);
            if (!res)
            {
                return new Response<object> { StatusCode = 500, Message = "failed", Error = "status not updated " };
            }
            return new Response<object> { StatusCode = 200, Message = "success" };
        }
    }
}
