using Rocket.Core;
namespace Rocket.Options
{
    public class WaterMembershipOptions
    {
        public WaterMembershipOptions(){}
        public WaterMembershipOptions(int cityId, int karbari, int tariff, int kindBranch, int categoryUses, int waterDiameterId, double capacity, bool isVillage, int tedadVahed, double tkhfifENAB, double takhfifENFA)
        {
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            KindBranch = kindBranch;
            CategoryUses = categoryUses;
            WaterDiameterId = waterDiameterId;
            Capacity = capacity < 0 ? 0 : capacity;
            TedadVahed = tedadVahed < 1 ? 1 : tedadVahed;
            TakhfifENAB = tkhfifENAB < 0 ? 0 : tkhfifENAB;
            TakhfifENFA = takhfifENFA < 0 ? 0 : takhfifENFA;
            IsVillage = isVillage;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int KindBranch { get; set; }
        public bool IsVillage { get; set; }
        public int CategoryUses { get; set; }
        public int WaterDiameterId { get; set; }
        public double Capacity { get; set; }
        public bool ChangeCapacity { get; set; }
        public int TedadVahed { get; set; }
        public bool HasWater { get; set; }
        public bool TaiedAb { get; set; }
        public double TakhfifENAB { get; set; }
        public double TakhfifENFA { get; set; }
    }
}