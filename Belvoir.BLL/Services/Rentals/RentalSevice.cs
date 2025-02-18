using AutoMapper;
using Belvoir.Bll.DTO.Rental;
using Belvoir.Bll.Helpers;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Rental;
using CloudinaryDotNet;
using Dapper;
using Microsoft.AspNetCore.Http;
//using Org.BouncyCastle.Bcpg;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using CloudinaryDotNet;
using Belvoir.DAL.Models.Query;

namespace Belvoir.Bll.Services.Rentals
{
    public interface IRentalService
    {
        public Task<Response<object>> AddRental(IFormFile[] files, RentalSetDTO Data, Guid userid);

        public Task<Response<RentalViewDTO>> GetRentalById(Guid id);

        public Task<Response<IEnumerable<RentalViewDTO>>> PaginatedRentalProduct(RentalQuery query);

        public Task<Response<object>> DeleteRental(Guid rentalId, Guid userid);

        public Task<Response<object>> UpdateRental(Guid rentalId, IFormFile[] files, RentalSetDTO Data, Guid userid);


        public Task<Response<object>> AddWishlist(Guid userId, Guid productId);
        public Task<Response<IEnumerable<RentalWhishListviewDTO>>> GetWishlist(Guid userId);


    }

    public class RentalSevice : IRentalService
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRentalRepository _repo;
        public RentalSevice(IDbConnection connection, IMapper mapper, ICloudinaryService cloudinary, IRentalRepository repo)
        {
            _connection = connection;
            _mapper = mapper;
            _cloudinaryService = cloudinary;
            _repo = repo;
        }

        public async Task<Response<object>> AddRental(IFormFile[] files, RentalSetDTO Data, Guid userId)
        {
            if (files.Length > 3)
            {
                return new Response<object>
                {
                    StatusCode = 400,
                    Error = "You can only upload a maximum of 3 images."
                };
            }

            var categoryExists = await _repo.CetegoryExist(Data.fabrictype);
            if (categoryExists == 0)
            {
                return new Response<object>
                {
                    StatusCode = 404,
                    Error = "Category does not exist."
                };
            }

            var rentalProduct = _mapper.Map<RentalProduct>(Data);
            rentalProduct.CreatedBy = userId;
            rentalProduct.Id = Guid.NewGuid();
            rentalProduct.CreatedAt = DateTime.UtcNow;
            rentalProduct.IsDeleted = false;
            var result = await _repo.AddRentalProductAsync(rentalProduct, userId);

            for (int i = 0; i < files.Length; i++)
            {
                var url = await _cloudinaryService.UploadImageAsync(files[i]);

                bool isPrimary = i == 0;
                await _repo.AddRentalImage(rentalProduct.Id, url, isPrimary);


            }


            return new Response<object>
            {
                StatusCode = 200,
                Message = "Rental item added successfully."
            };
        }


        public async Task<Response<RentalViewDTO>> GetRentalById(Guid id)
        {
            var rental = await _repo.GetRentalProductById(id);

            if (rental == null)
            {
                return new Response<RentalViewDTO>
                {
                    StatusCode = 404,
                    Error = "Rental item not found"
                };
            }

            var imagesresponse = await _repo.GetRentalImagesByProductId(id);

            var mapped = _mapper.Map<RentalViewDTO>(rental);
            mapped.images = imagesresponse.ToList();

            return new Response<RentalViewDTO>
            {
                Message = "Rental item retrieved successfully",
                StatusCode = 200,
                Data = mapped
            };
        }

        public async Task<Response<IEnumerable<RentalViewDTO>>> PaginatedRentalProduct(RentalQuery query)
        {
            var rawData = await _repo.GetRentalProductsAsync(query);

            var resultDict = new Dictionary<string, RentalViewDTO>();

            foreach (var (rentalProduct, rentalImage) in rawData)
            {
                var mapped = _mapper.Map<RentalViewDTO>(rentalProduct);
                if (!resultDict.ContainsKey(rentalProduct.Id.ToString()))
                {
                    mapped.images = new List<RentalImage>();
                    resultDict[rentalProduct.Id.ToString()] = mapped;
                }
                resultDict[rentalProduct.Id.ToString()].images.Add(rentalImage);
            }
            var result = resultDict.Values.ToList();
            return new Response<IEnumerable<RentalViewDTO>>
            {
                StatusCode = 200,
                Data = result,
                Message = "success"
            };
        }

        public async Task<Response<object>> DeleteRental(Guid rentalId, Guid userid)
        {

            var rentalProduct = await _repo.GetRentalProductById(rentalId);

            if (rentalProduct == null)
            {
                return new Response<object>
                {
                    StatusCode = 404,
                    Error = "Rental product not found"
                };
            }

            await _repo.RentalProductAsDeleted(rentalId, userid);

            return new Response<object>
            {
                Message = "Rental item deleted successfully",
                StatusCode = 200
            };
        }


        public async Task<Response<object>> UpdateRental(Guid rentalId, IFormFile[] files, RentalSetDTO Data, Guid userId)
        {
            // Check if rental product exists
            var rentalProduct = await _repo.GetRentalProductById(rentalId);
            if (rentalProduct == null)
            {
                return new Response<object>
                {
                    StatusCode = 404,
                    Error = "Rental product not found"
                };
            }

            var fabric = await _repo.CetegoryExist(Data.fabrictype);
            if (fabric == null)
            {
                return new Response<object>
                {
                    StatusCode = 404,
                    Error = "Fabric category does not exist"
                };
            }

            var mappedProduct = _mapper.Map<RentalProduct>(Data);
            mappedProduct.Id = rentalId;
            mappedProduct.UpdatedBy = userId;
            mappedProduct.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateRentalProduct(mappedProduct);

            if (files != null && files.Length > 0)
            {
                await _repo.DeleteRentalImages(rentalId);

                for (int i = 0; i < files.Length; i++)
                {
                    var imagePath = await _cloudinaryService.UploadImageAsync(files[i]);
                    await _repo.AddRentalImage(rentalId, imagePath, i == 0);

                }
            }

            return new Response<object>
            {
                Message = "Rental item updated successfully",
                StatusCode = 200
            };
        }





        public async Task<Response<object>> AddWishlist(Guid userId, Guid productId)
        {
            // Toggle the wishlist item and get the number of rows affected
            var res = await _repo.ToggleWishlist(userId, productId);

            // Determine the response based on the number of rows affected
            if (res == 1)
            {
                return new Response<object>
                {
                    Message = "item added to wishlist",
                    StatusCode = 200
                };
            }
            else if (res == -1)
            {
                return new Response<object>
                {
                    Message = "item removed from wishlist",
                    StatusCode = 200
                };
            }
            else if (res == -2)
            {
                return new Response<object>
                {
                    Message = "Product not found",
                    StatusCode = 404
                };
            }
            else
            {
                return new Response<object>
                {
                    Message = "unexpected result",
                    StatusCode = 500
                };
            }
        }


        public async Task<Response<IEnumerable<RentalWhishListviewDTO>>> GetWishlist(Guid userId)
        {
            var rawData = await _repo.GetWishlist(userId);

            var resultDict = new Dictionary<string, RentalWhishListviewDTO>();

            foreach (var (rentalProduct, rentalImage) in rawData)
            {
                if (!resultDict.ContainsKey(rentalProduct.ProductId.ToString()))
                {
                    var mapped = _mapper.Map<RentalWhishListviewDTO>(rentalProduct);
                    mapped.images = new List<RentalImage>();
                    resultDict[rentalProduct.ProductId.ToString()] = mapped;
                }

                if (rentalImage != null)
                {
                    resultDict[rentalProduct.ProductId.ToString()].images.Add(rentalImage);
                }
            }

            var rentals = resultDict.Values.ToList();

            return new Response<IEnumerable<RentalWhishListviewDTO>>
            {
                Data = rentals,
                StatusCode = 200,
                Message = "Wishlist retrieved successfully."
            };

        }

    }
}
