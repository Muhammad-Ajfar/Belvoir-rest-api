﻿using Belvoir.DAL.Models;
using Belvoir.DAL.Models.Query;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Admin
{
    public interface IClothesRepository
    {
        public Task<IEnumerable<Cloth>> GetClothes(ProductQuery query);
        public Task<int> AddWhishlist(Guid userid, Guid productid);
        public Task<IEnumerable<WhishList>> GetWishlist(Guid userId);
        public Task<int> ExistItem(Guid userid, Guid productid);
        
        public Task<ClothCategory> GetCategory();


    }
    public class ClothesRepository : IClothesRepository
    {
        private readonly IDbConnection _dbConnection;
        public ClothesRepository(IDbConnection dbConnection) {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<Cloth>> GetClothes(ProductQuery query)
        {
            var parameters = new DynamicParameters();
            parameters.Add("pTitle", query.SearchTerm, DbType.String);
            parameters.Add("pMaterial", query.Materials?.Where(m => !string.IsNullOrWhiteSpace(m)).Any() == true? string.Join(",", query.Materials.Where(m => !string.IsNullOrWhiteSpace(m))): null);
            parameters.Add("pColors", query.Colors?.Where(m => !string.IsNullOrWhiteSpace(m)).Any() == true ? string.Join(",", query.Colors.Where(m => !string.IsNullOrWhiteSpace(m))) : null);
            parameters.Add("pDesignPattern", query.DesignPatterns?.Where(m => !string.IsNullOrWhiteSpace(m)).Any() == true ? string.Join(",", query.DesignPatterns.Where(m => !string.IsNullOrWhiteSpace(m))) : null);
            parameters.Add("pMinPrice", query.MinPrice, DbType.Decimal);
            parameters.Add("pMaxPrice", query.MaxPrice, DbType.Decimal);
            parameters.Add("pSortBy", query.SortBy, DbType.String);
            parameters.Add("pIsDescending", query.IsDescending, DbType.Boolean);
            parameters.Add("pPageSize", query.PageSize, DbType.Int32);
            parameters.Add("pPageNo", query.PageNo, DbType.Int32);
            var clothes = await _dbConnection.QueryAsync<Cloth>("GetCloths", parameters, commandType: CommandType.StoredProcedure);

            return clothes;
        }


        public async Task<int> AddWhishlist(Guid userid, Guid productid)
        {
            return await _dbConnection.ExecuteAsync("insert into Wishlist (user_id,clothes_id) values(@usrid,@prid)", new { usrid = userid, prid = productid });
        }


        public async Task<IEnumerable<WhishList>> GetWishlist(Guid userId)
        {
            var query = @"select Wishlist.id as WhishlistId,Cloths.Id,Title,Description,Price,ImageUrl from Wishlist join Cloths on Wishlist.clothes_id=Cloths.id where Wishlist.user_id=@usrid";
            return await _dbConnection.QueryAsync<WhishList>(query, new { usrid = userId });
        }


        public async Task<int> ExistItem(Guid userId, Guid productId)
        {
            var query = @"SELECT COUNT(*) 
                  FROM Wishlist 
                  WHERE user_id = @usrid AND clothes_id = @prid";

            return await _dbConnection.ExecuteAsync(query, new { usrid = userId, prid = productId });
        }

       
        public async Task<ClothCategory> GetCategory()
        {

            var query = "GetClothCategory";

            var result = await _dbConnection.QueryMultipleAsync(query,commandType:CommandType.StoredProcedure, commandTimeout: 60);
            var designtype = await result.ReadAsync<CategoryItem>();
            var colors = await result.ReadAsync<CategoryItem>();
            var materialtype = await result.ReadAsync<CategoryItem>();
            if (!designtype.Any() && !colors.Any() && !materialtype.Any())
            {
                return new ClothCategory
                {
                    Designtype = null,
                    colors = null,
                    Materialtype = null
                };

            }
            return new ClothCategory
            {
                Designtype = designtype?.ToList() ?? new List<CategoryItem>(),
                colors = colors?.ToList() ?? new List<CategoryItem>(),
                Materialtype = materialtype?.ToList() ?? new List<CategoryItem>()
            };
        }

    }


}
