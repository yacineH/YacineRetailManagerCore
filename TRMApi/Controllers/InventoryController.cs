using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;

        public InventoryController(IConfiguration config)
        {
            _config = config;
        }

        //manager ou bien admin
        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public List<InventoryModel> Get()
        {
            InventoryData inventory = new InventoryData(_config);

            return inventory.GetInventory();
        }

        //warehouse and Admin
        //[Authorize(Roles = "WareHouseWorker")]
        //[Authorize(Roles = "Admin")]

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post(InventoryModel inventory)
        {
            InventoryData item = new InventoryData(_config);
            item.SaveInventoryRecord(inventory);
        }
    }
}
