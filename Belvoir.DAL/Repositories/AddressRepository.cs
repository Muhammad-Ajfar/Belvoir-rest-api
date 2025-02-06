using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories
{
    public interface IAddressRepository
    {
        public Task<List<Address>> GetAddressByUser(Guid userId);
        public Task<int> AddAddress(Address address);
        public Task<int> UpdateAddress(Address address);
        public Task<int> SoftDeleteAddress(Guid id);


    }

    public class AddressRepository : IAddressRepository
    {
        private readonly IDbConnection _connection;

        public AddressRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Address>> GetAddressByUser(Guid userId)
        {
            string query = @"SELECT Id, BuildingName, Street, City, State, PostalCode, ContactName, ContactNumber
                            FROM Address 
                            WHERE UserId = @UserId and IsDeleted = FALSE";

            var addresses = await _connection.QueryAsync<Address>(query, new { UserId = userId });
            return addresses.ToList();
        }

        public async Task<int> AddAddress(Address address)
        {
            string query = @"INSERT INTO Address 
                            (Id, UserId, BuildingName, Street, City, State, PostalCode, 
                             ContactName, ContactNumber) 
                            VALUES 
                            (@Id, @UserId, @BuildingName, @Street, @City, @State, @PostalCode, 
                             @ContactName, @ContactNumber)";

            return await _connection.ExecuteAsync(query, address);
        }


        public async Task<int> UpdateAddress(Address address)
        {
            string query = @"UPDATE Address 
                            SET BuildingName = @BuildingName, 
                                Street = @Street, 
                                City = @City, 
                                State = @State, 
                                PostalCode = @PostalCode, 
                                ContactName = @ContactName, 
                                ContactNumber = @ContactNumber
                            WHERE Id = @Id";

            return await _connection.ExecuteAsync(query, address);
        }

        public async Task<int> SoftDeleteAddress(Guid id)
        {
            string query = @"UPDATE Address 
                            SET IsDeleted = TRUE, 
                                UpdatedAt = NOW()
                            WHERE Id = @Id";

            return await _connection.ExecuteAsync(query, new { Id = id });
        }



    }
}
