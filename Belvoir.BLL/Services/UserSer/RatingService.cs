using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Rental;
using Belvoir.DAL.Repositories.UserRep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Services.UserSer
{
    public interface IRatingService
    {
        public Task<Response<object>> AddRating(Guid userid, Guid entityid, RatingItem data, string rating_to);
        public Task<Response<IEnumerable<Ratings>>> GetRating(Guid entityid, string rating_to);

        public Task<Response<object>> DeleteRating(Guid ratingId, string rating_to);
        public Task<Response<object>> UpdateRating(Guid ratingId, RatingItem data,Guid userId, string rating_to);
    }
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }
        public async Task<Response<object>> AddRating(Guid userid, Guid entityid, RatingItem data,string rating_to)
        {
            var response = await _ratingRepository.AddRating(entityid, userid, data,rating_to);

            if (response == -1)
            {
                return new Response<object>
                {
                    StatusCode = 400,
                    Message = "The user has already rated this cloth "
                };
            }
            if (response == 0)
            {
                return new Response<object>
                {
                    StatusCode = 400,
                    Message = "Error in adding Rating"
                };
            }
            return new Response<object>
            {

                StatusCode = 200,
                Message = "success"
            };
        }

        
        public async Task<Response<AvgRating>> GetRating(Guid entityid , string rating_to)
        {
            var ratings = await _ratingRepository.GetRating(entityid, rating_to);


            return new Response<AvgRating>
            {
                StatusCode = 200,
                Message = "Ratings retrieved successfully",
                Data = ratings
            };
        }

        public async Task<Response<object>> DeleteRating(Guid ratingId, string rating_to)
        {
            var response = await _ratingRepository.DeleteRating(ratingId,rating_to);

            if (response == 0)
            {
                return new Response<object>
                {
                    StatusCode = 400,
                    Message = "Error deleting rating"
                };
            }

            return new Response<object>
            {
                StatusCode = 200,
                Message = "Rating deleted successfully"
            };
        }

        public async Task<Response<object>> UpdateRating(Guid ratingId, RatingItem data, Guid userId, string rating_to)
        {
            var response = await _ratingRepository.UpdateRating(ratingId, data,userId,rating_to);

            if (response == 0)
            {
                return new Response<object>
                {
                    StatusCode = 400,
                    Message = "Error updating rating"
                };
            }

            return new Response<object>
            {
                StatusCode = 200,
                Message = "Rating updated successfully"
            };
        }

    }
}
