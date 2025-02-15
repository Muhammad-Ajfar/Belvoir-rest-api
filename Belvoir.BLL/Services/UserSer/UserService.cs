using Belvoir.Bll.DTO.Delivery;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.UserRep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Services.UserSer
{
    public interface IuserService
    {
        public Task<Response<UserProfile>> GetUserProfile(Guid id);
    }
    public class UserService : IuserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Response<UserProfile>> GetUserProfile(Guid id)
        {
            var response = await _userRepository.SingleProfile(id);
            if (response == null)
            {
                return new Response<UserProfile> { StatusCode = 404, Message = "the profile doesnot exist", };
            }
            return new Response<UserProfile> { StatusCode = 200, Message = "success", Data = response };
        }
    }
}
