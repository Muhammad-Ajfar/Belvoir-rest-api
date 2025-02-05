using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Rental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Services.Rentals
{
    public interface IFabricService
    {
        public Task<Response<IEnumerable<FabricCategory>>> GetFabricCategoriesAsync();
        public Task<Response<string>> AddFabricCategoryAsync(string name, Guid userId);
        public Task<Response<string>> UpdateFabricCategoryAsync(FabricCategory fabricCategory, Guid userId);


    }

    public class FabricService : IFabricService
    {
        private readonly IFabricRepository _fabricRepository;

        public FabricService(IFabricRepository fabricRepository)
        {
            _fabricRepository = fabricRepository;
        }

        public async Task<Response<IEnumerable<FabricCategory>>> GetFabricCategoriesAsync()
        {
            var categories = await _fabricRepository.GetFabricCategories();
            return new Response<IEnumerable<FabricCategory>>
            {
                StatusCode = 200,
                Message = "Fabric categories retrieved successfully",
                Data = categories
            };
        }

        public async Task<Response<string>> AddFabricCategoryAsync(string name, Guid userId)
        {
            int result = await _fabricRepository.AddFabricCategory(name, userId);

            if (result == -1)
            {
                return new Response<string>
                {
                    StatusCode = 400,
                    Message = "Fabric category with the same name already exists",
                    Error = "Duplicate entry"
                };
            }

            return new Response<string>
            {
                StatusCode = 201,
                Message = "Fabric category added successfully",
                Data = "Category created"
            };
        }

        public async Task<Response<string>> UpdateFabricCategoryAsync(FabricCategory fabricCategory, Guid userId)
        {
            int result = await _fabricRepository.UpdateFabricCategory(fabricCategory, userId);

            if (result == -1)
            {
                return new Response<string>
                {
                    StatusCode = 404,
                    Message = "Fabric category not found",
                    Error = "Invalid category ID"
                };
            }

            if (result == -2)
            {
                return new Response<string>
                {
                    StatusCode = 400,
                    Message = "A fabric category with this name already exists",
                    Error = "Duplicate name"
                };
            }

            return new Response<string>
            {
                StatusCode = 200,
                Message = "Fabric category updated successfully",
                Data = "Category updated"
            };
        }
    }

}
