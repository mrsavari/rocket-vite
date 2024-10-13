using Rocket.Core;
namespace Rocket.Options
{
    public class SewageMembershipOptions
    {
        public SewageMembershipOptions(){}
        public SewageMembershipOptions(int cityId, int karbari, int tariff, int kindBranch, bool isVillage,  int tedadVahed, double takhfifENFA,int categoryUses)
        {
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            KindBranch = kindBranch;
            TedadVahed = tedadVahed < 1 ? 1 : tedadVahed;
            TakhfifENFA = takhfifENFA < 0 ? 0 : takhfifENFA;
            IsVillage = isVillage;
            CategoryUses = categoryUses;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int KindBranch { get; set; }
        public int CategoryUses { get; set; }
        public bool IsVillage { get; set; }
        public int TedadVahed { get; set; }
        public double TakhfifENFA { get; set; }
    }
}