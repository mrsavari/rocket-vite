using System.Collections.Generic;
namespace Rocket.Options
{
    public enum TankerSource
    {
        Water,
        Transportation,
        ThirdRemark,
        Seasonal,
        Tax
    }

    public class TankerOptions
    {
        public TankerOptions(){}
        public TankerOptions(int cityId, int karbari, int tariff, int categoryUses, int capacity, int distence, int consumptionType, int? categoryId, bool isHouse, bool isVillage, bool transportation, bool ro, int? cycleId)
        {
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            CategoryUses = categoryUses;
            Capacity = capacity;
            IsVillage = isVillage;
            IsHouse = isHouse;
            Transportation = transportation;
            Distence = distence;
            RO = ro;
            Excludes = new List<TankerSource>();
            ConsumptionType = consumptionType;
            CategoryId = categoryId;
            CycleId = cycleId;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int CategoryUses { get; set; }
        public int Capacity { get; set; }
        public int Distence { get; set; }
        public bool IsHouse { get; set; }
        public bool IsVillage { get; set; }
        public bool Transportation { get; set; }
        public bool RO { get; set; }
        public int ConsumptionType { get; set; }
        public int? CategoryId { get; set; }
        public int? CycleId { get; set; }

        public double WaterRate { get; set; }
        public List<TankerSource> Excludes { get; set; }
    }
}