using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects
{
    public class Utils
    {
        public static decimal GetDecimalFromString(string value)
        {
            string amount = value;

            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var separator = culture.NumberFormat.CurrencyDecimalSeparator;

            amount = amount.Replace(",", separator);
            amount = amount.Replace(".", separator);

            decimal transactionAmount = Convert.ToDecimal(amount);

            return transactionAmount;
        }
    }
}
