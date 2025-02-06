using AutoMapper;
using Belvoir.Bll.DTO.Address;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Services
{
    public interface IAddressService
    {
        public Task<Response<List<AddressGetDTO>>> GetAddressesByUser(Guid userId);
        public Task<Response<string>> AddAddress(Guid userId, AddressAddDTO addressAddDto);
        public Task<Response<string>> UpdateAddress(Address address);
        public Task<Response<string>> SoftDeleteAddress(Guid id);

    }
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<List<AddressGetDTO>>> GetAddressesByUser(Guid userId)
        {
            var addresses = await _repository.GetAddressByUser(userId);
            if (addresses == null || !addresses.Any())
            {
                return new Response<List<AddressGetDTO>>
                {
                    StatusCode = 404,
                    Message = "No addresses found for this user.",
                    Data = null
                };
            }
            var addressesDto = _mapper.Map<List<AddressGetDTO>>(addresses);

            return new Response<List<AddressGetDTO>>
            {
                StatusCode = 200,
                Message = "Addresses retrieved successfully.",
                Data = addressesDto
            };
        }

        public async Task<Response<string>> AddAddress(Guid userId,AddressAddDTO addressAddDto)
        {
            Address address = _mapper.Map<Address>(addressAddDto);
            address.UserId = userId;
            address.Id = Guid.NewGuid();
            int rowsAffected = await _repository.AddAddress(address);
            if (rowsAffected > 0)
            {
                return new Response<string>
                {
                    StatusCode = 201,
                    Message = "Address added successfully.",
                    Data = address.Id.ToString()
                };
            }

            return new Response<string>
            {
                StatusCode = 400,
                Message = "Failed to add address.",
                Error = "Database operation failed."
            };
        }

        public async Task<Response<string>> UpdateAddress(Address address)
        {
            int rowsAffected = await _repository.UpdateAddress(address);
            if (rowsAffected > 0)
            {
                return new Response<string>
                {
                    StatusCode = 200,
                    Message = "Address updated successfully.",
                    Data = address.Id.ToString()
                };
            }

            return new Response<string>
            {
                StatusCode = 400,
                Message = "Failed to update address.",
                Error = "Database operation failed."
            };
        }

        public async Task<Response<string>> SoftDeleteAddress(Guid id)
        {
            int rowsAffected = await _repository.SoftDeleteAddress(id);
            if (rowsAffected > 0)
            {
                return new Response<string>
                {
                    StatusCode = 200,
                    Message = "Address soft deleted successfully.",
                    Data = id.ToString()
                };
            }

            return new Response<string>
            {
                StatusCode = 404,
                Message = "Address not found or already deleted.",
                Error = "Invalid address ID."
            };
        }
    }

}
