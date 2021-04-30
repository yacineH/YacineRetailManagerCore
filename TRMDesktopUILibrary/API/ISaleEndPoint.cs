using System.Threading.Tasks;
using TRMDesktopUILibrary.Model;

namespace TRMDesktopUILibrary.API
{
    public interface ISaleEndPoint
    {
        Task PostSale(SaleModel sale);
    }
}