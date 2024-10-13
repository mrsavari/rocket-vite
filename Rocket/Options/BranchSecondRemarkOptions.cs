using Rocket.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Options
{
    public class BranchSecondRemarkOptions 
    {
        public BranchSecondRemarkOptions(int cityId, bool isVillage, int kindBranchId, int tariff, double takhfifENAB)
        {
            CityId = cityId;
            IsVillage = isVillage;
            KindBranchId = kindBranchId;
            Tariff = tariff;
            TakhfifENAB = takhfifENAB;
        }

        public int CityId { get; set; }
        public bool IsVillage { get; set; }
        public int KindBranchId { get; set; }
        public int Tariff { get; set; }
        public double TakhfifENAB { get; set; }
    }
}
