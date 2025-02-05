using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Rental
{
    public interface IFabricRepository
    {
        public Task<IEnumerable<FabricCategory>> GetFabricCategories();
        public Task<int> AddFabricCategory(string name, Guid userId);
        public Task<int> UpdateFabricCategory(FabricCategory fabricCategory, Guid userId);


    }

    public class FabricRepository : IFabricRepository
    {
        private readonly IDbConnection _connection;

        public FabricRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<FabricCategory>> GetFabricCategories()
        {
            var query = "SELECT id, name FROM FabricCategory WHERE isdeleted = 0";
            return await _connection.QueryAsync<FabricCategory>(query);
        }


        public async Task<int> AddFabricCategory(string name, Guid userId)
        {
            // Check if the fabric category already exists
            int exists = await _connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM FabricCategory WHERE name = @name",
                new { name });

            if (exists > 0)
            {
                return -1; // Indicates that the category already exists
            }

            // Insert the new fabric category
            var query = @"
                    INSERT INTO FabricCategory (id, name, isdeleted, createdby, createdat) 
                    VALUES (UUID(), @name, 0, @userId, NOW())";

            return await _connection.ExecuteAsync(query, new
            {
                name,
                userId
            });
        }

        public async Task<int> UpdateFabricCategory(FabricCategory fabricCategory, Guid userId)
        {
            // Check if the category exists
            int categoryExists = await _connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM FabricCategory WHERE id = @Id AND isdeleted = 0",
                new { Id = fabricCategory.Id });

            if (categoryExists == 0)
            {
                return -1; // Category not found
            }

            // Check if the new name already exists (case-insensitive)
            int nameExists = await _connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM FabricCategory WHERE name = @Name AND id <> @Id AND isdeleted = 0",
                new { Name = fabricCategory.Name, Id = fabricCategory.Id });

            if (nameExists > 0)
            {
                return -2; // Duplicate name exists
            }

            // Update the category
            var query = @"
                    UPDATE FabricCategory 
                    SET name = @Name, updatedby = @UserId, updatedat = NOW() 
                    WHERE id = @Id AND isdeleted = 0";

            return await _connection.ExecuteAsync(query, new
            {
                Name = fabricCategory.Name,
                UserId = userId,
                Id = fabricCategory.Id
            });
        }


    }
}
