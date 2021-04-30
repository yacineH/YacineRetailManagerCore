using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDLL.Internal.DataAccess;
using TRMDLL.Models;

namespace TRMDLL.DataAccess
{
    public class InventoryData
    {
        public List<InventoryModel> GetInventory()
        {
            SQLDataAccess sql = new SQLDataAccess();

            var output = sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "TRMDataBase");

            return output;

        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            SQLDataAccess sql = new SQLDataAccess();

            sql.SaveData("dbo.spInventory_Insert", item, "TRMDataBase");
        }
    }
}
