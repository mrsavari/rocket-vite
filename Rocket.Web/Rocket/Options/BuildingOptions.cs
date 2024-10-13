using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Options
{
    public class BuildingOptions
    {
        public BuildingOptions(bool isVillage, int cityId, double infrastructure, double structureKind, double damageCost, string damageTitle)
        {
            IsVillage = isVillage;
            CityId = cityId;
            Infrastructure = infrastructure;
            StructureKind = structureKind;
            DamageCost = damageCost;
            DamageTitle = damageTitle;
        }

        public bool IsVillage { get; set; }
        public int CityId { get; set; }
        public double Infrastructure { get; set; }
        public double StructureKind { get; set; }
        public double DamageCost { get; set; }
        public string DamageTitle { get; set; }
    }
}
