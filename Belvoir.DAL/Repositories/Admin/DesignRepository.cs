using Belvoir.DAL.Models;
using Belvoir.DAL.Models.Mesurements;
using Belvoir.DAL.Models.Query;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Belvoir.DAL.Repositories.Admin
{
    public interface IDesignRepository
    {
        public Task<List<Design>> GetDesignsAsync(DesignQueryParameters queryParams);
        public Task<Design> GetDesignById(Guid designId);

        public Task<int> AddDesignWithImagesAsync(Design design);
        public Task<IEnumerable<Image>> GetImagesByDesignIdAsync(Guid designId);

        public Task<bool> UpdateDesignAsync(Design design, List<Guid>? removeImageIds, List<Image>? newImages);
        public Task<int> SoftDeleteDesignAsync(Guid designId);


        public Task<bool> AddMesurementGuide(Mesurment_Guides mesurment);
        public Task<bool> AddDesignMesurment(List<Design_Mesurment> design_Mesurments);
        public Task<IEnumerable<MesurementListGet>> GetDesignMesurment(Guid design_id);
        public Task<IEnumerable<MesurmentResponse>> AddMesurmentValues(MesurementSet mesurment, Guid user_id);
        public Task<IEnumerable<MesurmentGuidGet>> GetMesurement();
    }

    public class DesignRepository : IDesignRepository
    {
        private readonly IDbConnection _dbConnection;
        public DesignRepository(IDbConnection dbConnection) 
        {
            _dbConnection = dbConnection;
        }
        public async Task<List<Design>> GetDesignsAsync(DesignQueryParameters queryParams)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Name", queryParams.Name);
            parameters.Add("@p_Category", queryParams.Category);
            parameters.Add("@p_MinPrice", queryParams.MinPrice);
            parameters.Add("@p_MaxPrice", queryParams.MaxPrice);
            parameters.Add("@p_Available", queryParams.Available);
            parameters.Add("@p_SortBy", queryParams.SortBy);
            parameters.Add("@p_IsDescending", queryParams.IsDescending);
            parameters.Add("@p_PageNo", queryParams.PageNo);
            parameters.Add("@p_PageSize", queryParams.PageSize);

            var query = "CALL SearchDressDesignsWithImages(@p_Name, @p_Category, @p_MinPrice, @p_MaxPrice, @p_Available, @p_SortBy, @p_IsDescending, @p_PageNo, @p_PageSize);";

            var designDictionary = new Dictionary<Guid, Design>();

            var result = await _dbConnection.QueryAsync<Design, Image, Design>(
                query,
                (design, image) =>
                {
                    if (!designDictionary.TryGetValue(design.Id, out var existingDesign))
                    {
                        existingDesign = design;
                        existingDesign.Images = new List<Image>();
                        designDictionary.Add(design.Id, existingDesign);
                    }

                    existingDesign.Images.Add(image);
                    return existingDesign;
                },
                param: parameters,
                splitOn: "ImageUrl"
            );

            return designDictionary.Values.ToList();
        }

        public async Task<Design> GetDesignById(Guid designId)
        {
            var designDictionary = new Dictionary<Guid, Design>();

            //var res = await _dbConnection.QueryAsync<dynamic>(
            //    "CALL GetDressDesignById(@DesignId)",
            //    new { DesignId = designId });

            //Console.WriteLine(JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented));


            var result = await _dbConnection.QueryAsync<Design, Image, Design>(
                "CALL GetDressDesignById(@DesignId)",
                (design, image) =>
                {
                    if (!designDictionary.TryGetValue(design.Id, out var existingDesign))
                    {
                        existingDesign = design;
                        existingDesign.Images = new List<Image>();
                        designDictionary.Add(existingDesign.Id, existingDesign);
                    }

                    if (image != null && image.Id != Guid.Empty)
                    {
                        existingDesign.Images.Add(image);
                    }

                    return existingDesign;
                },
                new { DesignId = designId },
                splitOn: "ImageUrl"
            );

            return designDictionary.Values.FirstOrDefault();
        }

        public async Task<int> AddDesignWithImagesAsync(Design design)
        {
            // Ensure the connection is open
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                var designSql = @"
                INSERT INTO DressDesign 
                (Id, Name, Description, Category, Price, Available, IsDeleted, CreatedBy, CreatedAt) 
                VALUES 
                (@Id, @Name, @Description, @Category, @Price, @Available, 0, @CreatedBy, NOW());
            ";

                await _dbConnection.ExecuteAsync(designSql, design, transaction);

                var imageSql = @"
                INSERT INTO DesignImages (Id, DesignId, ImageUrl, IsPrimary, CreatedAt)
                VALUES (@Id, @EntityId, @ImageUrl, @IsPrimary, NOW());
            ";

                await _dbConnection.ExecuteAsync(imageSql, design.Images, transaction);

                transaction.Commit();
                return 1;  // Success
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("An unexpected error occurred.", ex); // Provide more detailed error info
            }
            finally
            {
                if (_dbConnection.State == ConnectionState.Open)
                {
                    _dbConnection.Close();
                }
            }
        }

        public async Task<IEnumerable<Image>> GetImagesByDesignIdAsync(Guid designId)
        {
            var sql = "SELECT Id, DesignId AS EntityId, ImageUrl, IsPrimary FROM DesignImages WHERE DesignId = @DesignId";
            return await _dbConnection.QueryAsync<Image>(sql, new { DesignId = designId });
        }


        public async Task<bool> UpdateDesignAsync(Design design, List<Guid>? removeImageIds, List<Image>? newImages)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

                using var transaction = _dbConnection.BeginTransaction();
            try
            {
                // Update design details
                var updateDesignSql = @"
            UPDATE DressDesign 
            SET Name = @Name, 
                Description = @Description, 
                Category = @Category, 
                Price = @Price, 
                Available = @Available, 
                UpdatedAt = NOW() 
            WHERE Id = @Id;
        ";
                await _dbConnection.ExecuteAsync(updateDesignSql, design, transaction);

                // Remove specified images
                if (removeImageIds != null && removeImageIds.Any())
                {
                    var removeImagesSql = "DELETE FROM DesignImages WHERE Id IN @Ids;";
                    await _dbConnection.ExecuteAsync(removeImagesSql, new { Ids = removeImageIds }, transaction);
                }

                // Add new images if provided
                if (newImages != null && newImages.Any())
                {
                    var addImagesSql = @"
                INSERT INTO DesignImages (Id, DesignId, ImageUrl, IsPrimary, CreatedAt)
                VALUES (@Id, @EntityId, @ImageUrl, @IsPrimary, NOW());
            ";
                    await _dbConnection.ExecuteAsync(addImagesSql, newImages, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<int> SoftDeleteDesignAsync(Guid designId)
        {
            var sql = "UPDATE DressDesign SET IsDeleted = 1 WHERE Id = @DesignId";
            return await _dbConnection.ExecuteAsync(sql, new { DesignId = designId });
        }



        public async Task<bool> AddMesurementGuide(Mesurment_Guides mesurment)
        {
            string query = @"INSERT INTO `belvoir`.`measurement_guides` (`guide_id`, `measurement_name`,`description`) VALUES (UUID(),@measurement_name,@description);";

            return await _dbConnection.ExecuteAsync(query, new { measurement_name = mesurment.measurement_name, description = mesurment.description }) > 0;
        }

        public async Task<bool> AddDesignMesurment(List<Design_Mesurment> design_Mesurments)
        {
            _dbConnection.Open();
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    string query = @"INSERT INTO `belvoir`.`design_mesurment` (`id`,`design_id`,`guide_id`) VALUES (UUID(),@design_id,@guide_id);";

                    foreach (var item in design_Mesurments) {
                        await _dbConnection.ExecuteAsync(query, new { guide_id = item.guide_id, design_id  = item.design_id});
                    }
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
            
            
            
        }

        public async Task<IEnumerable<MesurementListGet>> GetDesignMesurment(Guid design_id)
        {
            string query = @"SELECT id,measurement_name,description FROM belvoir.design_mesurment as dm JOIN belvoir.measurement_guides as mg ON dm.guide_id = mg.guide_id Where design_id = @design ;";             
            return await _dbConnection.QueryAsync<MesurementListGet>(query,new { design = design_id});
        }

        public async Task<IEnumerable<MesurmentResponse>> AddMesurmentValues(MesurementSet mesurment,Guid user_id)
        {
            Guid set_id = Guid.NewGuid();
            string query1 = @"INSERT INTO `belvoir`.`measurements` (`measurement_id`,`set_id`,`des_mes_id`,`measurement_value`) VALUES (UUID(),@set_id,@des_mes_id,@measurement_value);";
            string query2 = @"INSERT INTO `belvoir`.`measurement_sets` (`set_id`,`set_name`,`user_id`,`design_id`) VALUES (@set_id,@set_name,@user_id,@tailor_product);";

            _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    bool ans = await _dbConnection.ExecuteAsync(query2, new { set_id = set_id, set_name = mesurment.set_name, user_id = user_id, tailor_product = mesurment.tailor_product_id }) > 0;
                    if (ans)
                    {
                        foreach (var item in mesurment.values)
                        {
                            await _dbConnection.ExecuteAsync(query1, new { set_id = set_id, des_mes_id = item.id, measurement_value = item.mesurement_value });
                        }
                    }
                    transaction.Commit();

                    var res = await _dbConnection.QueryAsync<MesurmentResponse>("SELECT measurement_name,measurement_value FROM design_mesurment dm JOIN measurement_guides mg ON dm.guide_id = mg.guide_id left join measurements m ON m.des_mes_id = dm.id where m.set_id = @set_id", new { set_id = set_id });
                    return res;
                }
                catch (Exception ex) 
                {
                    transaction.Rollback();
                    throw ex;
                    
                    
                }
                
            }
            
            
        }

        public async Task<IEnumerable<MesurmentGuidGet>> GetMesurement()
        {
            var query = "SELECT * FROM measurement_guides";
            return await _dbConnection.QueryAsync<MesurmentGuidGet>(query);
        }
        
    }
}
