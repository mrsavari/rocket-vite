using Rocket.Core;

namespace Rocket.Options
{
    public class WaterCostOptions
    {
        public WaterCostOptions(){}
        public WaterCostOptions(int cityId,int karbari,int tariff,int categoryUses,int tedadVahed,bool isVillage,int readCode,long estateId, FineCost fineCost,int cycleId = 0){
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            CategoryUses = categoryUses;
            TedadVahed = tedadVahed < 1? 1 : tedadVahed;
            IsVillage = isVillage;
            ReadCode = readCode;
            EstateId = estateId;
            FineCost = fineCost;
            CycleId = cycleId;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int CategoryUses { get; set; }
        public int TedadVahed { get; set; }
        public bool IsVillage { get; set; }
        public int ReadCode { get; set; }
        public long EstateId { get; set; }
        public FineCost FineCost { get; set; }

        public int IllegalId { get; set; }
        public int CycleId { get; set; }
    }
}