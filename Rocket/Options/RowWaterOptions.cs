using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Options
{
    public class RowWaterOptions
    {
        public RowWaterOptions(int cityId,int waterDiameterId)
        {
            CityId = cityId;
            WaterDiameterId = waterDiameterId;
        }
        public int CityId { get; set; }
        public int WaterDiameterId { get; set; }
    }
}
