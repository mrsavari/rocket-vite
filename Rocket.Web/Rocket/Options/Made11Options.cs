using Rocket.Core;
namespace Rocket.Options
{
    public class Made11Options
    {
        public Made11Options(){}
        public Made11Options(ResourceType type, int cityId, int karbari, int tariff, int kindBranch, bool isVillage, double tkhfifENAB, double takhfifENFA)
        {
            Type = type;
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            KindBranch = kindBranch;
            TakhfifENAB = tkhfifENAB < 0 ? 0 : tkhfifENAB;
            TakhfifENFA = takhfifENFA < 0 ? 0 : takhfifENFA;
            IsVillage = isVillage;
        }
        public ResourceType Type { get; set; }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int KindBranch { get; set; }
        public bool IsVillage { get; set; }
        public double TakhfifENAB { get; set; }
        public double TakhfifENFA { get; set; }
    }
}