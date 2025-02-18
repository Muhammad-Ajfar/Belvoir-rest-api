using AutoMapper;
using Belvoir.Bll.DTO.Design;
using Belvoir.Bll.Helpers;
using Belvoir.DAL.Models;
using Belvoir.DAL.Models.Mesurements;
using Belvoir.DAL.Models.Query;
using Belvoir.DAL.Repositories.Admin;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Belvoir.Bll.Services.Admin
{
    public interface IDesignService
    {
        Task<Response<List<DesignDTO>>> GetDesignsAsync(DesignQueryParameters queryParams);
        Task<Response<DesignDTO>> GetDesignByIdAsync(Guid designId);
        Task<Response<string>> AddDesignAsync(Design design, List<IFormFile> imageFiles);
        Task<Response<string>> UpdateDesignAsync(UpdateDesignDto dto);
        public Task<Response<string>> SoftDeleteDesignAsync(Guid designId);
        Task<Response<object>> AddMesurementGuide(Mesurment_Guides design_Mesurments);
        Task<Response<object>> AddDesignMesurement(List<Design_Mesurment> mesurement);
        Task<Response<IEnumerable<MesurementListGet>>> GetDesignMesurment(Guid design_id);
        Task<Response<IEnumerable<MesurmentResponse>>> AddMesurmentValues(MesurementSet mesurment, Guid user_id);
        public Task<Response<IEnumerable<MesurmentGuidGet>>> GetMesurement();
    }

    public class DesignService : IDesignService
    {
        public readonly IDesignRepository _designRepository;
        public readonly ICloudinaryService _cloudinaryService;
        public readonly IMapper _mapper;

        public DesignService(IDesignRepository designRepository, ICloudinaryService cloudinaryService, IMapper mapper)
        {
            _designRepository = designRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        public async Task<Response<List<DesignDTO>>> GetDesignsAsync(DesignQueryParameters queryParams)
        {
            var designs = await _designRepository.GetDesignsAsync(queryParams);

            if (designs == null || designs.Count == 0)
            {
                return new Response<List<DesignDTO>>
                {
                    StatusCode = 404,
                    Message = "No designs found"
                };
            }
            var designDtos = _mapper.Map<List<DesignDTO>>(designs);

            return new Response<List<DesignDTO>>
            {
                StatusCode = 200,
                Message = "Designs retrieved successfully",
                Data = designDtos
            };
        }

        public async Task<Response<DesignDTO>> GetDesignByIdAsync(Guid designId)
        {
            var design = await _designRepository.GetDesignById(designId);

            if (design == null)
            {
                return new Response<DesignDTO>
                {
                    StatusCode = 404,
                    Message = "Design not found",
                    Data = null
                };
            }

            var designDto = _mapper.Map<DesignDTO>(design);

            return new Response<DesignDTO>
            {
                StatusCode = 200,
                Message = "Design retrieved successfully",
                Data = designDto
            };
        }



        public async Task<Response<string>> AddDesignAsync(Design design, List<IFormFile> imageFiles)
        {
            if (imageFiles == null || imageFiles.Count < 3)
            {
                return new Response<string>
                {
                    StatusCode = 400,
                    Message = "At least 3 images are required.",
                    Error = "Invalid number of images."
                };
            }

            design.Id = Guid.NewGuid();
            design.CreatedAt = DateTime.UtcNow;

            var imageList = new List<Image>();

            // Upload images to Cloudinary
            for (int i = 0; i < imageFiles.Count; i++)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(imageFiles[i]);
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return new Response<string>
                    {
                        StatusCode = 500,
                        Message = "Image upload failed.",
                        Error = "Cloudinary service error."
                    };
                }

                imageList.Add(new Image
                {
                    Id = Guid.NewGuid(),
                    EntityId = design.Id,
                    ImageUrl = imageUrl,
                    IsPrimary = i == 0  // First image as primary
                });
            }

            design.Images = imageList;

            // Call repository to save design and images in a single transaction
            var result = await _designRepository.AddDesignWithImagesAsync(design);

            if (result > 0)
            {
                return new Response<string>
                {
                    StatusCode = 201,
                    Message = "Design added successfully.",
                    Data = design.Id.ToString()
                };
            }

            return new Response<string>
            {
                StatusCode = 500,
                Message = "Failed to add design.",
                Error = "Database insert error."
            };
        }

        public async Task<Response<string>> UpdateDesignAsync(UpdateDesignDto dto)
        {
            // Retrieve existing images
            var existingImages = await _designRepository.GetImagesByDesignIdAsync(dto.Id);
            int currentImageCount = existingImages.Count();

            // Remove images if requested
            if (dto.RemoveImageIds != null && dto.RemoveImageIds.Any())
            {
                currentImageCount -= dto.RemoveImageIds.Count;
            }

            // Upload new images if provided
            var uploadedImages = new List<Image>();
            if (dto.NewImages != null && dto.NewImages.Any())
            {
                foreach (var image in dto.NewImages)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(image);
                    uploadedImages.Add(new Image
                    {
                        Id = Guid.NewGuid(),
                        EntityId = dto.Id,
                        ImageUrl = imageUrl,
                        IsPrimary = false // Default to false; logic can be added for primary flag
                    });
                }
                currentImageCount += uploadedImages.Count;
            }

            // Ensure at least 3 images exist
            if (currentImageCount < 3)
            {
                return new Response<string>
                {
                    StatusCode = 400,
                    Message = "At least 3 images are required.",
                    Error = "Image count is less than 3",
                    Data = null
                };
            }

            // Update design and images in a transaction
            var design = new Design
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Price = dto.Price,
                Available = dto.Available
            };

            var success = await _designRepository.UpdateDesignAsync(design, dto.RemoveImageIds, uploadedImages);

            if (success)
            {
                return new Response<string>
                {
                    StatusCode = 200,
                    Message = "Design updated successfully",
                    Error = null,
                    Data = "Success"
                };
            }
            else
            {
                return new Response<string>
                {
                    StatusCode = 500,
                    Message = "Failed to update design",
                    Error = "Database error",
                    Data = null
                };
            }
        }

        public async Task<Response<string>> SoftDeleteDesignAsync(Guid designId)
        {
            var existingDesign = await _designRepository.GetDesignById(designId);

            if (existingDesign == null || existingDesign.IsDeleted)
            {
                return new Response<string>
                {
                    StatusCode = 404,
                    Message = "Design not found or already deleted.",
                    Error = "Invalid DesignId",
                    Data = null
                };
            }

            var rowsAffected = await _designRepository.SoftDeleteDesignAsync(designId);

            if (rowsAffected > 0)
            {
                return new Response<string>
                {
                    StatusCode = 200,
                    Message = "Design soft deleted successfully.",
                    Error = null,
                    Data = null
                };
            }

            return new Response<string>
            {
                StatusCode = 500,
                Message = "Failed to soft delete design.",
                Error = "Database error",
                Data = null
            };
        }


        public async Task<Response<object>> AddMesurementGuide(Mesurment_Guides mesurment_Guides)
        {
            bool result = await _designRepository.AddMesurementGuide(mesurment_Guides);
            if (result)
            {
                return new Response<object> { StatusCode = 200, Message = "added successfully" };
            }
            return new Response<object> { StatusCode = 200, Message = "Failed" , Error = "server error" };
        }
        public async Task<Response<object>> AddDesignMesurement(List<Design_Mesurment> design_Mesurments)
        {
            bool result = await _designRepository.AddDesignMesurment(design_Mesurments);
            if (result)
            {
                return new Response<object> { StatusCode = 200, Message = "added successfully" };
            }
            return new Response<object> { StatusCode = 200, Message = "Failed", Error = "server error" };
        }
        public async Task<Response<IEnumerable<MesurementListGet>>> GetDesignMesurment(Guid design_id)
        {
            var data = await _designRepository.GetDesignMesurment(design_id);
            if (data == null)
            {
                return new Response<IEnumerable<MesurementListGet>> { StatusCode = 200, Message = "no data" };
            }
            else
            {
                return new Response<IEnumerable<MesurementListGet>> { StatusCode = 200, Message = "success", Data = data };
            }
        }

        public async Task<Response<IEnumerable<MesurmentResponse>>> AddMesurmentValues(MesurementSet mesurment, Guid user_id)
        {
            if (mesurment == null ) {
                return new Response<IEnumerable<MesurmentResponse>> { StatusCode = 404, Error = "can't be null" };
            }
            if (mesurment.values == null)
            {
                return new Response<IEnumerable<MesurmentResponse>> { StatusCode = 404, Error = "mesurment can't be null" };
            }
            var result = await _designRepository.AddMesurmentValues(mesurment, user_id);
            
            return new Response<IEnumerable<MesurmentResponse>> { StatusCode = 200, Message = "success"  ,Data = result};
        }
        public async Task<Response<IEnumerable<MesurmentGuidGet>>> GetMesurement()
        {
            
            var result = await _designRepository.GetMesurement();
            if (result == null)
            {
                return new Response<IEnumerable<MesurmentGuidGet>> { StatusCode = 500, Error = "server error" };
            }
            return new Response<IEnumerable<MesurmentGuidGet>> { StatusCode = 200, Message = "success" , Data = result};
        }
       
    }
}
