using System.Net.Http;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Model;

namespace TRMDesktopUILibrary.API
{
    public interface IAPIHelper
    {
        HttpClient ApiClient { get; }

        void LogOffUser();
        Task<AuthenticatedUser> Authenticate(string username, string password);
        Task GetLoggedInUserInfo(string token);

   
    }
}