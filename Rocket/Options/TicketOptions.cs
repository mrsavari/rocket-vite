using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Options
{
    public class TicketOptions
    {
        public TicketOptions(int cityId, int waterDiameterId, int tariff)
        {
            CityId = cityId;
            WaterDiameterId = waterDiameterId;
            Tariff = tariff;
        }
        public int CityId { get; set; }
        public int WaterDiameterId { get; set; }
        public int Tariff { get; set; }
    }
}
