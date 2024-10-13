using Rocket.Core;
using System.Linq;

namespace Rocket.Options
{
    public class ThirdRemarkFAOptions
    {
        public ThirdRemarkFAOptions(){}
        public ThirdRemarkFAOptions(bool TaiedFA,int cityId, int karbari, int tariff, int kindBranch, int categoryUses, int sewageDiameterId,int areaId, double capacity, bool isVillage, int tedadVahed, double takhfifENFA)
        {
            CityId = cityId;
            Karbari = karbari;
            Tariff = tariff;
            KindBranch = kindBranch;
            CategoryUses = categoryUses;
            SewageDiameterId = sewageDiameterId;
            Capacity = capacity < 0 ? 0 : capacity;
            AreaId = areaId;
            TedadVahed = tedadVahed < 1 ? 1 : tedadVahed;
            TakhfifENFA = takhfifENFA < 0 ? 0 : takhfifENFA;
            IsVillage = isVillage;
        }
        public int CityId { get; set; }
        public int Karbari { get; set; }
        public int Tariff { get; set; }
        public int KindBranch { get; set; }
        public int AreaId { get; set; }
        public bool IsVillage { get; set; }
        public int CategoryUses { get; set; }
        public int SewageDiameterId { get; set; }
        public double Capacity { get; set; }
        public bool ChangeCapacity { get; set; }
        public int TedadVahed { get; set; }
        public bool TaiedFA { get; set; }
        public bool HasSewage { get{ return this.TaiedFA && new double[] { 1, 3 }.Contains(this.KindBranch);}}
        public double TakhfifENFA { get; set; }
    }
}