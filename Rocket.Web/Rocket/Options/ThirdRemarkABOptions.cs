using Rocket.Core;
using System.Linq;
namespace Rocket.Options
{
    public class ThirdRemarkABOptions
    {
        public ThirdRemarkABOptions(){}
        public ThirdRemarkABOptions(bool TaiedAB,int cityId, int karbari, int tariff, int kindBranch, int categoryUses, int waterDiameterId,int areaId, double capacity, bool isVillage, int tedadVahed, double tkhfifENAB)
        {
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            KindBranch = kindBranch;
            CategoryUses = categoryUses;
            WaterDiameterId = waterDiameterId;
            Capacity = capacity < 0 ? 0 : capacity;
            AreaId = areaId;
            TedadVahed = tedadVahed < 1 ? 1 : tedadVahed;
            TakhfifENAB = tkhfifENAB < 0 ? 0 : tkhfifENAB;
            IsVillage = isVillage;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int KindBranch { get; set; }
        public int AreaId { get; set; }
        public bool IsVillage { get; set; }
        public int CategoryUses { get; set; }
        public int WaterDiameterId { get; set; }
        public double Capacity { get; set; }
        public bool ChangeCapacity { get; set; }
        public int TedadVahed { get; set; }
        public bool TaiedAB { get; set; }
        public bool HasWater { get{ return this.TaiedAB && new double[] { 1, 2 }.Contains(this.KindBranch);}}
        public double TakhfifENAB { get; set; }
    }
}