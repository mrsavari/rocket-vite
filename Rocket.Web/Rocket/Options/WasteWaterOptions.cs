namespace Rocket.Options
{
    public class WasteWaterOptions
    {
        public WasteWaterOptions(){}
        public WasteWaterOptions(int cityId,int karbari,int tariff,int categoryUses,int kindBranch,bool isVillage){
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            CategoryUses = categoryUses;
            KindBranch = kindBranch;
            IsVillage = isVillage;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int CategoryUses { get; set; }
        public int KindBranch { get; set; }
        public bool IsVillage { get; set; }
    }
}