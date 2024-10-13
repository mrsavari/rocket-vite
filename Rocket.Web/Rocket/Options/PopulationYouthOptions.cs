namespace Rocket.Options
{
    public class PopulationYouthOptions
    {
        public PopulationYouthOptions(){}
        public PopulationYouthOptions(int cityId, int karbari, int tariff, int categoryUses, int tedadVahed, double capacity, bool isVillage)
        {
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            CategoryUses = categoryUses;
            TedadVahed = tedadVahed < 1? 1 : tedadVahed;
            IsVillage = isVillage;
            Capacity = capacity;
        }
        public double Capacity { get; set; }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int CategoryUses { get; set; }
        public int TedadVahed { get; set; }
        public bool IsVillage { get; set; }
    }
}