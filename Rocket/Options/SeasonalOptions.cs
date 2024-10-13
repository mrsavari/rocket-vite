namespace Rocket.Options
{
    public class SeasonalOptions
    {
        public SeasonalOptions(){}
        public SeasonalOptions(int cityId,int karbari,int tariff,int categoryUses,bool isVillage){
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            CategoryUses = categoryUses;
            IsVillage = isVillage;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int CategoryUses { get; set; }
        public bool IsVillage { get; set; }
    }
}