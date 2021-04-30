using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManagerAPI.Models;
using TRMDLL.DataAccess;
using TRMDLL.Models;

namespace TRMDataManagerAPI.Controllers
{
   [Authorize]
    public class SaleController : ApiController
    {
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData();
            string userId = RequestContext.Principal.Identity.GetUserId();

            data.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Admin,Manager")]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            #region Aide
            //pour savoir dans quel rele est suppesé etre le userLoggedIn
            //if(RequestContext.Principal.IsInRole("Admin"))
            //{
            //    //Do Admin stuff
            //}
            //else if(RequestContext.Principal.IsInRole("Manager"))
            //{
            //    //do Manager stuff

            //}
            #endregion

            SaleData sale = new SaleData();
            
            return sale.GetSaleReport();
        }
    }
}
