using Rocket.Core;

namespace Rocket.Options
{
    public class DigOptions
    {
        public DigOptions(CalculationType type, int cityId,double sifonEzafeInput ,bool sifonShared ,int estateCount ,int waterDiameterId ,int sewageDiameterId ,double distanceToMeter ,double distanceToSifon)
        {
            Type = type;
            CityId = cityId;
            SifonEzafeInput = sifonEzafeInput;
            SifonShared = sifonShared;
            EstateCount = estateCount;
            WaterDiameterId = waterDiameterId;
            SewageDiameterId = sewageDiameterId;
            DistanceToMeter = distanceToMeter;
            DistanceToSifon = distanceToSifon;
        }

        public CalculationType Type { get; set; }
        public int CityId { get; set; }
        public double SifonEzafeInput { get; set; }
        public bool SifonShared { get; set; }
        public int EstateCount { get; set; }
        public int WaterDiameterId { get; set; }
        public int SewageDiameterId { get; set; }
        public double DistanceToMeter { get; set; }
        public double DistanceToSifon { get; set; }
    }
}