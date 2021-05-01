using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDLL.Internal.DataAccess;
using TRMDLL.Models;

namespace TRMDLL.DataAccess
{
    public class UserData
    {
        private readonly IConfiguration _config;

        public UserData(IConfiguration config)
        {
            _config = config;
        }
        public List<UserModel> GetUserById(string Id)
        {
            SQLDataAccess sql = new SQLDataAccess(_config);
            
            var p = new { Id=Id };
            
            var output=sql.LoadData<UserModel,dynamic>("dbo.spUserLookUp", p, "TRMDataBase");

            return output;
        }
    }
}
