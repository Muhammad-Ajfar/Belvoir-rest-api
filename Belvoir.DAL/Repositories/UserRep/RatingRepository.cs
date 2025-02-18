using Belvoir.DAL.Models;
using Belvoir.DAL.Models.TailorProductModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.UserRep
{
    public interface IRatingRepository
    {
        public Task<int> AddRating(Guid EntityId, Guid userid, RatingItem data,string rating_to);
        public Task<AvgRating> GetRating(Guid entityid, string rating_to);
        public Task<int> DeleteRating(Guid ratingId, string rating_to);
        public Task<int> UpdateRating(Guid ratingId, RatingItem data, Guid userId, string rating_to);
    }
    public class RatingRepository : IRatingRepository
    {
        private readonly IDbConnection _dbConnection;
        public RatingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> AddRating(Guid EntityId, Guid userid, RatingItem data,string rating_to)
        {
            int reviewExists = await _dbConnection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(*) 
                        FROM Ratings 
                        WHERE userid = @UserId 
                        AND isDeleted = false
                        AND (
                            (@rating_to = 'tailor' AND tailorid = @EntityId) OR
                            (@rating_to = 'rental' AND rentalid = @EntityId) OR
                            (@rating_to = 'cloth' AND clothid = @EntityId) OR
                            (@rating_to = 'design' AND designid = @EntityId)
                        );",
                    new { UserId = userid, EntityId = EntityId , rating_to = rating_to});

            if (reviewExists > 0)
            {
                return -1; // Indicates that the user has already rated this product
            }

            var query = @"INSERT INTO Ratings (id, designid, clothid,tailorid,rentalid, userid, rating_to, isdeleted, createdby, message, ratingvalue)
                            VALUES 
                            (UUID(), 
                                CASE WHEN @rating_to = 'design' THEN @entityid ELSE NULL END, 
                                CASE WHEN @rating_to = 'cloth' THEN @entityid ELSE NULL END, 
                                CASE WHEN @rating_to = 'tailor' THEN @entityid ELSE NULL END, 
                                CASE WHEN @rating_to = 'rental' THEN @entityid ELSE NULL END, 
                                @userid,@rating_to,@isdeleted,@createdby,@message,@ratingvalue);";
            return await _dbConnection.ExecuteAsync(query, new { entityid = EntityId, userid = userid, isdeleted = false, createdby = userid, message = data.message, ratingvalue = data.ratingvalue ,rating_to = rating_to});
        }
        public async Task<AvgRating> GetRating(Guid entityid, string rating_to)
        {
            var query1 = @"SELECT AVG(ratingvalue), COUNT(*) from Ratings 
                            JOIN User ON Ratings.userid=User.id  
                            WHERE Ratings.isDeleted = false AND
                                (@rating_to = 'cloth' AND Ratings.clothid = @EntityId) OR
                                (@rating_to = 'tailor' AND Ratings.tailorid = @EntityId) OR
                                (@rating_to = 'rental' AND Ratings.rentalid = @EntityId) OR
                                (@rating_to = 'design' AND Ratings.designid = @EntityId);";
            var res = await _dbConnection.QueryFirstOrDefaultAsync<AVGCount>(query1, new { EntityId = entityid, rating_to = rating_to });
            var query = @"SELECT 
                            Ratings.id, 
                            User.name AS username, 
                            Ratings.ratingvalue, 
                            Ratings.message
                        FROM Ratings
                        JOIN User ON Ratings.userid = User.Id
                        WHERE Ratings.isDeleted = false AND
                            (@rating_to = 'cloth' AND Ratings.clothid = @EntityId) OR
                            (@rating_to = 'tailor' AND Ratings.tailorid = @EntityId) OR
                            (@rating_to = 'rental' AND Ratings.rentalid = @EntityId) OR
                            (@rating_to = 'design' AND Ratings.designid = @EntityId);";
            return new AvgRating() { ratings = await _dbConnection.QueryAsync<Ratings>(query, new { EntityId = entityid, rating_to = rating_to }), averageRating = res.averageRating, count = res.count };
        }

        public async Task<int> DeleteRating(Guid ratingId, string rating_to)
        {
            var query = @"UPDATE Ratings SET isdeleted = true WHERE id = @ratingId AND rating_to = @rating_to;";
            return await _dbConnection.ExecuteAsync(query, new { ratingId = ratingId, rating_to = rating_to });
        }

        public async Task<int> UpdateRating(Guid ratingId, RatingItem data, Guid userId, string rating_to)
        {
            var query = @" UPDATE Ratings 
                            SET ratingvalue = @ratingValue, 
                                message = @message
                            WHERE id = @ratingId AND isDeleted = false AND userid = @userId AND rating_to = @rating_to";

            return await _dbConnection.ExecuteAsync(query, new { ratingId = ratingId, @message = data.message, @ratingValue = data.ratingvalue , userId = userId, rating_to = rating_to });
        }

    }
}
