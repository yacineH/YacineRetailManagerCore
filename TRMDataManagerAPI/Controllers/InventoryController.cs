using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDLL.DataAccess;
using TRMDLL.Models;

namespace TRMDataManagerAPI.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        //manager ou bien admin
        [Authorize(Roles = "Manager,Admin")]
        public List<InventoryModel> Get()
        {
            InventoryData inventory = new InventoryData();

            return inventory.GetInventory();
        }

        //warehouse and Admin
        //[Authorize(Roles = "WareHouseWorker")]
        //[Authorize(Roles = "Admin")]

        [Authorize(Roles = "Admin")]
        public void Post(InventoryModel inventory)
        {
            InventoryData item = new InventoryData();
            item.SaveInventoryRecord(inventory);
        }
    }
}
