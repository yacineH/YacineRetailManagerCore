using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUILibrary.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        public decimal GetTaxRate()
        {

            string rateText = ConfigurationManager.AppSettings["taxRate"];

            bool isValidTextRate = decimal.TryParse(rateText, out decimal output);

            if (isValidTextRate == false)
            {
                throw new ConfigurationErrorsException("the tax is not set up property");
            }

            return output;
        }
    }
}
