using System;
using Rocket.Core;

namespace Rocket.Options
{
    public class WaterMembershipFeeOptions
    {
        public WaterMembershipFeeOptions(){}
        public WaterMembershipFeeOptions(DateTime date,KindBranch kind,int cityId, int karbari, int tariff, int categoryUses, double tedadVahed, bool isVillage, double capacity, int changeCapacity)
        {
            Date = date;
            Kind = kind;
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            CategoryUses = categoryUses;
            TedadVahed = tedadVahed < 1? 1 : tedadVahed;
            IsVillage = isVillage;
            ChangeCapacity = changeCapacity;
            Capacity = capacity;
        }
        public DateTime Date { get; set; }
        public KindBranch Kind { get; set; }
        public double TakhfifENAB { get; set; }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int CategoryUses { get; set; }
        public double TedadVahed { get; set; }
        public int ChangeCapacity { get; set; }
        public double Capacity { get; set; }
        public bool IsVillage { get; set; }
    }
}