using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRMDLL.DataAccess;
using TRMDLL.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
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
