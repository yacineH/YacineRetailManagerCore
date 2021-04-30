using System.Collections.Generic;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Model;

namespace TRMDesktopUILibrary.API
{
    public interface IUserEndPoint
    {
        Task<List<UserModel>> GetAll();

        Task<Dictionary<string, string>> GetAllRoles();

        Task AddUserToRole(string userId, string roleName);

        Task RemoveUserFromRole(string userId, string roleName);
    }
}