using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Options
{
    public class SewageNasbOptions
    {
        public SewageNasbOptions(int cityId, int karbari, int tariff, int sewageDiameterId, int kindBranchId, double estateCount, double sifonEzafe, bool sifonShared, double sifonEzafeInput, bool isVillage, bool toolsByCustomer, double takhfifNasbFA)
        {
            TakhfifNasbFA = takhfifNasbFA;
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            SewageDiameterId = sewageDiameterId;
            KindBranchId = kindBranchId;
            EstateCount = estateCount;
            SifonEzafe = sifonEzafe;
            SifonShared = sifonShared;
            SifonEzafeInput = sifonEzafeInput;
            IsVillage = isVillage;
            ToolsByCustomer = toolsByCustomer;
        }

        public double TakhfifNasbFA { get; set; }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int SewageDiameterId { get; set; }
        public bool SifonShared { get; set; }
        public int KindBranchId { get; set; }
        public double EstateCount { get; set; }
        public double SifonEzafe { get; set; }
        public double SifonEzafeInput { get; set; }
        public bool IsVillage { get; set; }
        public bool ToolsByCustomer { get; set; }
    }
}
