using Belvoir.Bll.DTO.Rental;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Rental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Services.Rentals
{
    public interface IRentalCartService
    {
        Task<Response<RentalCart>> GetCartByUserId(Guid userId);

        Task<Response<string>> AddToCartAsync(Guid userId, AddToCartDTO cartDTO);
    }

    public class RentalCartService : IRentalCartService
    {
        private readonly IRentalCartRepository _repository;

        public RentalCartService(IRentalCartRepository repository)
        {
            _repository = repository;
        }

        public async Task<Response<RentalCart>> GetCartByUserId(Guid userId)
        {
            var cart = await _repository.GetCartByUserId(userId);

            if (cart == null)
            {
                return new Response<RentalCart>
                {
                    StatusCode = 404,
                    Message = "Cart not found"
                };
            }

            return new Response<RentalCart>
            {
                StatusCode = 200,
                Message = "Cart retrieved successfully",
                Data = cart
            };
        }

        public async Task<Response<string>> AddToCartAsync(Guid userId, AddToCartDTO cartDTO)
        {
            var currentCart = await _repository.GetCartByUserId(userId) ?? new RentalCart();
            currentCart.Items ??= new List<RentalCartItem>();


            if (currentCart.Items.Count >= 20)
            {
                return new Response<string> { StatusCode = 400, Message = "You can only have a maximum of 20 items in your cart.", Error = "Cart limit reached.", Data = null };

            }

            var existingCartItem = currentCart.Items.FirstOrDefault(item => item.ProductId == cartDTO.ProductId);

            if (existingCartItem != null)
            {
                return await UpdateCartItemQuantityAsync(existingCartItem.ItemId, existingCartItem.Quantity + cartDTO.Quantity);
            }

            if (cartDTO.Quantity > 10)
            {
                return new Response<string> { StatusCode = 400, Message = "You can only add a maximum of 10 of the same product to your cart.", Error = "Quantity limit exceeded.", Data = null };

            }

            await _repository.AddToCartAsync(userId, cartDTO.ProductId, cartDTO.Quantity);

            return new Response<string> { StatusCode = 200, Message = "Product added to cart successfully.", Data = null };
        }

        public async Task<Response<string>> UpdateCartItemQuantityAsync(Guid cartItemId, int newQuantity)
        {
            if (newQuantity < 1 || newQuantity > 10)
            {
                return new Response<string> { StatusCode = 400, Message = "Quantity must be between 1 and 10.", Error = "Invalid quantity.", Data = null };
            }

            await _repository.UpdateCartItemQuantityAsync(cartItemId, newQuantity);
            return new Response<string> { StatusCode = 200, Message = "Cart item quantity updated successfully.", Data = null };
        }

        public async Task<Response<string>> RemoveCartItemAsync(Guid cartItemId)
        {
            await _repository.RemoveCartItemAsync(cartItemId);
            return new Response<string> { StatusCode = 200, Message = "Cart item removed successfully.", Data = null };
        }


    }

}
