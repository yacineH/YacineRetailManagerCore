using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDLL.Internal.DataAccess;
using TRMDLL.Models;

namespace TRMDLL.DataAccess
{
    public class ProductData
    {
        public List<ProductModel> GetProducts()
        {
            SQLDataAccess sql = new SQLDataAccess();

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll",new { },"TRMDataBase");

            return output;
        }

        public ProductModel GetProductById(int productid)
        {
            SQLDataAccess sql = new SQLDataAccess();

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = productid }, "TRMDataBase").FirstOrDefault();

            return output;
        }
    }
}
