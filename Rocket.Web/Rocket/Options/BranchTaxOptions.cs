using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Options
{
    public class BranchTaxOptions
    {
        public BranchTaxOptions(int cityId, double amount)
        {
            CityId = cityId;
            Amount = amount;
        }

        public int CityId { get; set; }
        public double Amount { get; set; }
    }
}
