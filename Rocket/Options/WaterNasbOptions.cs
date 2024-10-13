using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Options
{
    public class WaterNasbOptions
    {
        public WaterNasbOptions(int cityId, int waterDiameterId, double takhfifNasbAB, int kindBranchId, bool counter, bool kit, bool pipe)
        {
            CityId = cityId;
            WaterDiameterId = waterDiameterId;
            TakhfifNasbAB = takhfifNasbAB;
            KindBranchId = kindBranchId;
            Counter = counter;
            Kit = kit;
            Pipe = pipe;
        }
        public int CityId { get; set; }
        public int WaterDiameterId { get; set; }
        public double TakhfifNasbAB { get; set; }
        public int KindBranchId { get; set; }
        public bool Counter { get; set; }
        public bool Kit { get; set; }
        public bool Pipe { get; set; }
    }
}
