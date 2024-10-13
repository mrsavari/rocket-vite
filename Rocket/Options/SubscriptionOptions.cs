namespace Rocket.Options
{
    public class SubscriptionOptions
    {
        public SubscriptionOptions(){}
        public SubscriptionOptions(int cityId,int karbari,int tariff,int categoryUses,int tedadVahed){
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            CategoryUses = categoryUses;
            TedadVahed = tedadVahed < 1? 1 : tedadVahed;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int CategoryUses { get; set; }
        public int TedadVahed { get; set; }
    }
}