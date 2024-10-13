namespace Rocket.Options
{
    public class BrokenPipeOptions
    {
        public BrokenPipeOptions(){}
        public BrokenPipeOptions(int cityId,int karbari,int tariff,int categoryUses,int tedadVahed,bool isVillage,int readCode){
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            CategoryUses = categoryUses;
            TedadVahed = tedadVahed < 1? 1 : tedadVahed;
            IsVillage = isVillage;
            ReadCode = readCode;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int CategoryUses { get; set; }
        public int TedadVahed { get; set; }
        public bool IsVillage { get; set; }
        public int ReadCode { get; set; }
    }
}