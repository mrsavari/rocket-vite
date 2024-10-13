using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Core
{
    public class EntryHash
    {
        public static EntryHash Empty = new EntryHash(0, 0, 0, 0, 0, 0, 0);
        public static EntryHash GetKey(EntryType type, int cityId, int karbari, int costtypeId, int phase1 = 0, int phase2 = 0, int phase3 = 0, Ledger ledger = null)
        {
            EntryHash hash = new EntryHash(type, cityId, karbari, costtypeId, phase1, phase2, phase3);
            if (ledger != null)
            {
                if (ledger.ContainsKey(hash))
                {
                    return hash;
                }
                hash = new EntryHash(type, cityId, 0, costtypeId, phase1, phase2, phase3);
                if (ledger.ContainsKey(hash))
                {
                    return hash;
                }
                hash = new EntryHash(type, 0, karbari, costtypeId, phase1, phase2, phase3);
                if (ledger.ContainsKey(hash))
                {
                    return hash;
                }
                hash = new EntryHash(type, 0, 0, costtypeId, phase1, phase2, phase3);
                if (ledger.ContainsKey(hash))
                {
                    return hash;
                }
                return new EntryHash(0, 0, 0, 0, 0, 0, 0);
            }
            return hash;
        }

        public EntryHash(EntryType type, int cityId, int karbari, int costtypeId, int phase1=0, int phase2=0, int phase3=0)
        {
            Type = type;
            CityId = cityId;
            Karbari = karbari;
            CostTypeId = costtypeId;
            Phase1 = phase1;
            Phase2 = phase2;
            Phase3 = phase3;
        }
        public EntryHash(EntryType type, long extend)
        {
            Type = type;
            Extend = extend;
        }

        public EntryType Type { get; set; }

        public int CityId { get; set; }

        public int Karbari { get; set; }

        public int CostTypeId { get; set; }

        public int Phase1 { get; set; }

        public int Phase2 { get; set; }

        public int Phase3 { get; set; }

        public long Extend { get; set; }

        public override string ToString()
        {
            //if (Type == EntryType.Extend)
            //    return string.Format("{0}", Extend);
            return string.Format("{0}{1:D3}{2:D4}{3:D2}{4:D4}{5}{6}", ((int)Type), CityId, Karbari, CostTypeId, Phase1, Phase2, Phase3);
        }

    }
}