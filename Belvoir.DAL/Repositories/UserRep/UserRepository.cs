using Belvoir.DAL.Models;
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
    
    public interface IUserRepository
    {
        public Task<UserProfile> SingleProfile(Guid userid);
    }
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;
        public UserRepository(IDbConnection dbconnection) {
            _dbConnection = dbconnection;
        }

        public async Task<UserProfile> SingleProfile(Guid userid)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<UserProfile>("select * from User  where User.id=@id", new { id = userid });

        }

    }
}
