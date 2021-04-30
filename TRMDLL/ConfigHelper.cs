using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDLL
{
    public class ConfigHelper 
    {
        public static decimal GetTaxRate()
        {

            string rateText = ConfigurationManager.AppSettings["textRate"];

            bool isValidTextRate = decimal.TryParse(rateText, out decimal output);

            if (isValidTextRate == false)
            {
                throw new ConfigurationErrorsException("the tax is not set up property");
            }

            return output;
        }
    }
}
