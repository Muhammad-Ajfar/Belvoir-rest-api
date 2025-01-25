﻿using Belvoir.Bll.DTO;
using Belvoir.Bll.Helpers;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Admin;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace Belvoir.Bll.Services.Admin
{
    public interface IClothsServices
    {
        public Task<Response<object>> GetAllCloths(ProductQuery pquery);
        public Task<Response<object>> GetClothById(Guid id);
        public Task<Response<Object>> UpdateCloths(Guid id,IFormFile file, ClothDTO cloth);
        public Task<Response<Object>> DeleteCloths(Guid id);
        public Task<Response<Object>> AddCloths(IFormFile file,ClothDTO cloth);

        public Task<Response<object>> AddWishlist(Guid userId, Guid productId);

        public Task<Response<IEnumerable<WhishList>>> GetWishlist(Guid userId);


    }
    public class ClothsServices : IClothsServices
    {
        private readonly IDbConnection _connection;
        private readonly ICloudinaryService _cloudinary;
        private readonly IClothesRepository _repo;

        public ClothsServices(IDbConnection connection, ICloudinaryService cloudinary,IClothesRepository clothesRepository)
        {
            _connection = connection;
            _cloudinary = cloudinary;
            _repo = clothesRepository;
        }

        public async Task<Response<object>> GetAllCloths(ProductQuery pquery)
        {
            try
            {
                var clothes = await _repo.GetClothes(pquery);
                return new Response<object> { data = clothes, statuscode = 200, message = "success" };
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    error = ex.Message,
                    statuscode = 500
                };
            }
        }
        public async Task<Response<Object>> GetClothById(Guid id)
        {
            try
            {
                var user = await _connection.QueryFirstOrDefaultAsync<Cloth>("SELECT * FROM Cloths WHERE Id = @Id", new { Id = id });
                return new Response<object> { data = user, statuscode = 200, message = "success" };

            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    error = ex.Message,
                    statuscode = 500
                };
            }

        }
      
        public async Task<Response<Object>> AddCloths(IFormFile file , ClothDTO cloth)
        {
            try
            {
                Guid id = Guid.NewGuid();
                string imageurl = await _cloudinary.UploadImageAsync(file);

                await _connection.ExecuteAsync(
                "INSERT INTO Cloths (Description, DesignPattern, Id, Material, Title, ImageUrl,CreatedBy) VALUES (@Description, @Design, UUID(), @Material, @Title, @ImageUrl,'e2c7d233 - 3fd0 - 4527 - a79a - bfb45a762f1b')",
                new
                {
                    Description = cloth.Description,
                    Design = cloth.DesignPattern,
                    Material = cloth.Material,
                    Title = cloth.Title,
                    ImageUrl = imageurl
                }); 
                return new Response<object> {  statuscode = 201, message = "success" };
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    error = ex.Message,
                    statuscode = 500
                };
            }
        }
        public async Task<Response<Object>> UpdateCloths(Guid Id,IFormFile file, ClothDTO cloth)
        {
            try
            {
                await _connection.ExecuteAsync(
                    "UPDATE Cloths SET Description = @Description, DesignPattern = @Design, Material = @Materia, Title = @Title WHERE Id = @ClothId",
                    new
                    {
                        Description = cloth.Description,
                        Design = cloth.DesignPattern,
                        Materia = cloth.Material,
                        Title = cloth.Title,
                        ClothId = Id
                    });
                return new Response<object> { statuscode = 200, message = "success" };
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    error = ex.Message,
                    statuscode = 500
                };
            }
        }
        public async Task<Response<Object>> DeleteCloths(Guid id)
        {
            try
            {
                await _connection.ExecuteAsync("DELETE FROM Cloths WHERE Id = @Id", new { Id = id });
                return new Response<object> { statuscode = 200, message = "success" };
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    error = ex.Message,
                    statuscode = 500
                };
            }
        }

        public async Task<Response<object>> AddWishlist(Guid userId, Guid productId)
        {
            var itemexist = await _repo.ExistItem(userId, productId);
            if (itemexist > 0)
            {
                return new Response<object>
                {
                    message = "item already exist",
                    statuscode = 409
                };
            }
            await _repo.AddWhishlist(userId, productId);
            return new Response<object>
            {
                message = "item added success",
                statuscode = 200
            };
        }
        public async Task<Response<IEnumerable<WhishList>>> GetWishlist(Guid userId)
        {
            var response = await _repo.GetWishlist(userId);
            return new Response<IEnumerable<WhishList>>
            {
                data = response,
                statuscode = 200,
                message = "Wishlist retrieved successfully."
            };

        }
    }
}
