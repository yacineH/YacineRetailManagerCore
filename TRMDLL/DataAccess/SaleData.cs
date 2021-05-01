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
    public class SaleData
    {
        private readonly IConfiguration _config;

        public SaleData(IConfiguration config)
        {
            _config = config;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {

            //start filling in the sale detail models we will save to the database
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData product = new ProductData(_config);
            var taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                //get the information about this product
                var productInfo = product.GetProductById(item.ProductId);
                if (productInfo == null)
                {
                    throw new Exception($"the product id of {item.ProductId} not found in database");
                }

                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;

                if (productInfo.IsTaxable)
                {
                    detail.Tax = detail.PurchasePrice * taxRate;
                }

                details.Add(detail);
            }

            //create the sale model
            SaleDBModel sale = new SaleDBModel
            {
                Subtotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.Subtotal + sale.Tax;

            #region version initial 
            //save the sale model
            //SQLDataAccess sql = new SQLDataAccess();
            //sql.SaveData<SaleDBModel>("dbo.spSale_Insert", sale, "TRMDataBase");

            ////get the id from the sale mode
            //sale.Id=sql.LoadData<int, dynamic>("dbo.spSale_Lookup", new {sale.CashierId,sale.SaleDate }, "TRMDataBase").FirstOrDefault();

            ////finish filling in the detail models
            //foreach (var item in details)
            //{
            //    item.SaleId = sale.Id;
            //    //save the sale detail models
            //    sql.SaveData("dbo.spSaleDetail_Insert", item, "TRMDataBase");//ici on commence a inserer SaleDetail imaginos il y'a quatre
            //                                                                 //et l'insertion du dernier plante big probleme
            //}
            #endregion

            #region Rapped in transaction (transaction in C#)(faire attention a la non fermeture des transactions problemes de performance)

            using (SQLDataAccess sql = new SQLDataAccess(_config))
            {
                try
                {
                    sql.StartTransaction("TRMDataBase");

                    //save the sale model
                    sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                    //get the id from the sale mode
                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

                    //finish filling in the detail models
                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;

                        //save the sale detail models
                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                    }

                    sql.CommitTransaction();
                }
                catch
                {
                    sql.RoolBackTransaction();
                    throw;
                }
            }
            #endregion
        }


        public List<SaleReportModel> GetSaleReport()
        {
            SQLDataAccess sql = new SQLDataAccess(_config);

            var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMDataBase");

            return output;
        }
    }
}
