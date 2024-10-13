using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Rocket.Core
{
    public class TimeSeries : IEnumerable<Frame<Block>>
    {
        public SortedSplitList<Frame<Block>> Source {get; private set;}
        public IEnumerable<TimeRecord> Records {get;set;}
        public IEnumerable<TimeRecord> Overrides {get;set;}

        public TimeSeries(IEnumerable<TimeRecord> records, IEnumerable<TimeRecord> overrides = null)
        {
            Source = new SortedSplitList<Frame<Block>>(new CompareByHash<Block>());
            
            Process(records, overrides);
        }

        public void Process(IEnumerable<TimeRecord> records,IEnumerable<TimeRecord> overrides = null)
        {
            string tempHash = "";
            EntryHash hash = null;
            Frame<Block> currentFrame = null;
            //bool found = false;
            foreach (TimeRecord record in records)
            {
                if (tempHash != record.Hash)
                {
                    if (currentFrame != null && currentFrame.Count() > 0)
                        Source.Add(currentFrame);
                    EntryType entryType = (EntryType)int.Parse(record.Hash.Substring(0, 1));
                    int cityId = int.Parse(record.Hash.Substring(1, 3));
                    int karbari = int.Parse(record.Hash.Substring(4, 4));
                    int costtypeId = int.Parse(record.Hash.Substring(8, 2));
                    //if (record.Hash == "00000001050000161")
                    //    found = true;
                    int phase1 = int.Parse(record.Hash.Substring(10, 4));
                    int phase2 = int.Parse(record.Hash.Substring(14, 1));
                    int phase3 = int.Parse(record.Hash.Substring(15, 1));
                    hash = new EntryHash(entryType, cityId, karbari, costtypeId, phase1, phase2, phase3);
                    currentFrame = new Frame<Block>(hash);

                }
                currentFrame.Add(new Block(record.Date, record.From, record.To, record.Value, record.Label, record.Extra));
                tempHash = record.Hash;
            }
            if (overrides != null )
            {
                tempHash = "";
                hash = null;
                currentFrame = null;
                foreach (TimeRecord record in overrides)
                {
                    EntryType entryType = (EntryType)int.Parse(record.Hash.Substring(0, 1));
                    int cityId = int.Parse(record.Hash.Substring(1, 3));
                    int karbari = int.Parse(record.Hash.Substring(4, 4));
                    int costtypeId = int.Parse(record.Hash.Substring(8, 2));
                    int phase1 = int.Parse(record.Hash.Substring(10, 4));
                    int phase2 = int.Parse(record.Hash.Substring(14, 1));
                    int phase3 = int.Parse(record.Hash.Substring(15, 1));
                    hash = new EntryHash(entryType, cityId, karbari, costtypeId, phase1, phase2, phase3);
                    int index = Source.BinarySearch(new Frame<Block>(hash));
                    if (index > -1)
                    {
                        Source[index].Add(new Block(record.Date, record.From, record.To, record.Value, record.Label, record.Extra), (blocks) => { return blocks.FindIndex(block => block.Date == record.Date && block.FromRange == record.From && block.ToRange == record.To && block.Label == record.Label); });
                        continue;
                    }
                    if (tempHash != record.Hash)
                    {
                        if (currentFrame != null && currentFrame.Count() > 0)
                            Source.Add(currentFrame);
                        currentFrame = new Frame<Block>(hash);

                    }
                    currentFrame.Add(new Block(record.Date, record.From, record.To, record.Value, record.Label, record.Extra));
                    tempHash = record.Hash;
                }
            }
        }


        public Frame<Block> Select(EntryHash hash)
        {
            Frame<Block> item = new Frame<Block>(EntryHash.Empty);
            try
            {
                int index = Source.BinarySearch(new Frame<Block>(hash));
                if (index < 0)
                {
                    EntryHash newHash = null;
                    try
                    {
                        newHash = new EntryHash(hash.Type, hash.CityId, 0, hash.CostTypeId, hash.Phase1, hash.Phase2, hash.Phase3);
                        index = Source.BinarySearch(new Frame<Block>(newHash));
                        if (index > -1)
                            return Source[index].Copy();
                    }
                    catch { }
                    try
                    {
                        newHash = new EntryHash(hash.Type, 0, hash.Karbari, hash.CostTypeId, hash.Phase1, hash.Phase2, hash.Phase3);
                        index = Source.BinarySearch(new Frame<Block>(newHash));
                        if (index > -1)
                            return Source[index].Copy();
                    }
                    catch { }
                    try
                    {
                        newHash = new EntryHash(hash.Type, 0, 0, hash.CostTypeId, hash.Phase1, hash.Phase2, hash.Phase3);
                        index = Source.BinarySearch(new Frame<Block>(newHash));
                        if (index > -1)
                            return Source[index].Copy();
                    }
                    catch { }
                    item.Add(new Block(DateTime.Now.AddYears(-15),0,999999,0,hash.CostTypeId.ToString(),0));
                    return item;
                }
                return Source[index].Copy();
            }
            catch { }
            item.Add(new Block(DateTime.Now.AddYears(-15), 0, 999999, 0, hash.CostTypeId.ToString(), 0));
            return item;
        }

        public Frame<Block> GetFamily(IUsageSubject usage,int TedadKhanvar)
        {
            Frame<Block> family = new Frame<Block>(new EntryHash(0,0,0,0,0,0,0));
            family.Add(new Block(usage.StartedAt.AddDays(-1), 0, 99999, 1, "F", 0));
            family.Add(new Block(usage.EndedAt, 0, 99999, TedadKhanvar, "F", 0));
            family.Add(new Block(usage.EndedAt.AddYears(1), 0, 99999, 1, "F", 0));
            return family;
        }

        public IEnumerator<Frame<Block>> GetEnumerator()
        {
            return Source.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}