namespace Rocket.Options
{
    public class SubscriptionSewageOptions
    {
        public SubscriptionSewageOptions(){}
        public SubscriptionSewageOptions(int cityId,int karbari,int tariff,int categoryUses,int tedadVahed,int kindBranch){
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            CategoryUses = categoryUses;
            TedadVahed = tedadVahed < 1? 1 : tedadVahed;
            KindBranch = kindBranch;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int CategoryUses { get; set; }
        public int TedadVahed { get; set; }
        public int KindBranch { get; set; }
    }
}