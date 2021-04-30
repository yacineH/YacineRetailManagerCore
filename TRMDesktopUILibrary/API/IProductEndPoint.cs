using System.Collections.Generic;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Model;

namespace TRMDesktopUILibrary.API
{
    public interface IProductEndPoint
    {
        Task<List<ProductModel>> GetAll();
    }
}