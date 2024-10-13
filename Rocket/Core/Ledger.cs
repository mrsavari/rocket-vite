using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Core
{
    public class Ledger //:IDictionary<EntryHash,Entry>
    {
        public Ledger()
        {
            Items = new Dictionary<int, CityPartition>();
        }

        protected Dictionary<EntryHash, Entry> Data = new Dictionary<EntryHash, Entry>();

        private EntryHash _make(string key)
        {
            EntryType type = (EntryType)int.Parse(key.Substring(0, 1));
            int cityId = int.Parse(key.Substring(1, 3));
            int karbari = int.Parse(key.Substring(4, 4));
            int costtypeId = int.Parse(key.Substring(8, 2));
            int phase1 = int.Parse(key.Substring(10, 4));
            int phase2 = int.Parse(key.Substring(14, 1));
            int phase3 = int.Parse(key.Substring(15, 1));
            return new EntryHash(type, cityId, karbari, costtypeId, phase1, phase2, phase3);
        }

        public Dictionary<int, CityPartition> Items { get; private set; }

        public void Add(string key, Block value)
        {
            EntryHash hash = _make(key);
            if(!Items.ContainsKey(hash.CityId))
                Items.Add(hash.CityId, new CityPartition(hash.CityId));
            if (!Items[hash.CityId].Items.ContainsKey(hash.Karbari))
                Items[hash.CityId].Items.Add(hash.Karbari,new KarbariPartition(hash.Karbari));
            if (!Items[hash.CityId][hash.Karbari].Items.ContainsKey(hash.CostTypeId))
                Items[hash.CityId][hash.Karbari].Items.Add(hash.CostTypeId, new CostTypePartition(hash.CostTypeId));
            string code = "1"+hash.Phase1.ToString()+hash.Phase2.ToString()+hash.Phase3.ToString();
            if (!Items[hash.CityId][hash.Karbari][hash.CostTypeId].Items.ContainsKey(int.Parse(code)))
                Items[hash.CityId][hash.Karbari][hash.CostTypeId].Items.Add(int.Parse(code), new Entry());
            Items[hash.CityId][hash.Karbari][hash.CostTypeId][int.Parse(code)].AddLast(value);
        }

        public void Add(EntryHash hash, Block item)
        {
            if (!Items.ContainsKey(hash.CityId))
                Items.Add(hash.CityId, new CityPartition(hash.CityId));
            if (!Items[hash.CityId].Items.ContainsKey(hash.Karbari))
                Items[hash.CityId].Items.Add(hash.Karbari, new KarbariPartition(hash.Karbari));
            if (!Items[hash.CityId][hash.Karbari].Items.ContainsKey(hash.CostTypeId))
                Items[hash.CityId][hash.Karbari].Items.Add(hash.CostTypeId, new CostTypePartition(hash.CostTypeId));
            string code = "1" + hash.Phase1.ToString() + hash.Phase2.ToString() + hash.Phase3.ToString();
            if (!Items[hash.CityId][hash.Karbari][hash.CostTypeId].Items.ContainsKey(int.Parse(code)))
                Items[hash.CityId][hash.Karbari][hash.CostTypeId].Items.Add(int.Parse(code), new Entry());
            Items[hash.CityId][hash.Karbari][hash.CostTypeId][int.Parse(code)].AddLast(item);
        }

        public bool ContainsKey(EntryHash hash)
        {
            if (!Items.ContainsKey(hash.CityId))
                return false;
            if (!Items[hash.CityId].Items.ContainsKey(hash.Karbari))
                return false;
            if (!Items[hash.CityId][hash.Karbari].Items.ContainsKey(hash.CostTypeId))
                return false;
            string code = "1" + hash.Phase1.ToString() + hash.Phase2.ToString() + hash.Phase3.ToString();
            if (!Items[hash.CityId][hash.Karbari][hash.CostTypeId].Items.ContainsKey(int.Parse(code)))
                return false;
            return true;
        }

        public ICollection<EntryHash> Keys
        {
            get {
                return Data.Keys;
            }
        }

        public bool Remove(EntryHash key)
        {
            return Data.Remove(key);
        }

        public bool TryGetValue(EntryHash key, out Entry value)
        {
            return Data.TryGetValue(key,out value);
        }

        public Entry GetFamily(IUsageSubject usage, int TedadKhanvar)
        {
            Entry family = new Entry();
            family.AddLast(new Block(usage.StartedAt.AddDays(-1), 0, 99999, 1, "F", 0));
            family.AddLast(new Block(usage.EndedAt, 0, 99999, TedadKhanvar, "F", 0));
            family.AddLast(new Block(usage.EndedAt.AddYears(1), 0, 99999, 1, "F", 0));
            return family;
        }

        public ICollection<Entry> Values
        {
            get { return Data.Values; }
        }

        public Entry this[EntryHash key]
        {
            get
            {
                string code = "1" + key.Phase1.ToString() + key.Phase2.ToString() + key.Phase3.ToString();
                return Items[key.CityId][key.Karbari][key.CostTypeId][int.Parse(code)];
            }
            set
            {
                string code = "1" + key.Phase1.ToString() + key.Phase2.ToString() + key.Phase3.ToString();
                Items[key.CityId][key.Karbari][key.CostTypeId][int.Parse(code)] = value;
            }
        }

        public void Add(KeyValuePair<EntryHash, Entry> item)
        {
            Data.Add(item.Key,item.Value);
        }

        public void Clear()
        {
            Data.Clear();
        }

        public bool Contains(KeyValuePair<EntryHash, Entry> item)
        {
            return Data.Contains(item);
        }

        public int Count
        {
            get { return Data.Count; }
        }

    }
}