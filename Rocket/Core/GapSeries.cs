using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Rocket.Core
{
    public class GapSeries : IEnumerable<Frame<Block>>
    {
        public SortedSplitList<Frame<Block>> Source {get; private set;}
        public IEnumerable<GapRecord> Records { get; set; }

        public GapSeries(IEnumerable<GapRecord> records)
        {
            Source = new SortedSplitList<Frame<Block>>(new CompareByHash<Block>());
            
            Process(records);
        }

        public void Process(IEnumerable<GapRecord> records)
        {
            string tempHash = "";
            string label = "";
            EntryHash hash = null;
            Frame<Block> currentFrame = null;
            foreach (GapRecord record in records)
            {
                try
                {
                    if (tempHash != record.Hash)
                    {
                        if (currentFrame != null && currentFrame.Count() > 0)
                            Source.Add(currentFrame);
                        int CityId = int.Parse(record.Hash.Substring(0, 3));
                        int SerialNo = int.Parse(record.Hash.Substring(3, 6));
                        bool found = false;
                        if (SerialNo == 13441)
                            found = true;
                        int Ref = int.Parse(record.Hash.Substring(9, 2));
                        label = Ref == 2 ? "F" : (Ref == 3 ? "I" : "G" + Ref.ToString());
                        hash = new EntryHash(EntryType.Extend, CityId, SerialNo, Ref, 0, 0, 0);
                        currentFrame = new Frame<Block>(hash);

                    }
                    if (record.From != null)
                    {
                        int year = record.From.ToString().Substring(0, 4).ToInt();
                        int month = record.From.ToString().Substring(4, 2).ToInt();
                        int day = record.From.ToString().Substring(6, 2).ToInt();
                        double value = 0;
                        switch (currentFrame.Hash.CostTypeId)
                        {
                            case 1:
                                value = record.Payload.Substring(21, 1).ToDouble();
                                currentFrame.Add(new Block(new DateTime(year, month, day, 0, 0, 0), 0, 0, value, label, 0));
                                break;
                            default:
                                value = record.Payload.ToDouble();
                                currentFrame.Add(new Block(new DateTime(year, month, day, 0, 0, 0), 0, 0, value, label, 0));
                                break;
                        }
                        
                    }
                    if (record.To != null)
                    {
                        int year = record.To.ToString().Substring(0, 4).ToInt();
                        int month = record.To.ToString().Substring(4, 2).ToInt();
                        int day = record.To.ToString().Substring(6, 2).ToInt();
                        double value = 0;
                        switch (currentFrame.Hash.CostTypeId)
                        {
                            case 2:
                                value = 1;
                                currentFrame.Add(new Block(new DateTime(year, month, day, 0, 0, 0), 0, 0, value, label, 0));
                                break;
                            default:
                                currentFrame.Add(new Block(new DateTime(year, month, day, 0, 0, 0), 0, 0, value, label, 0));
                                break;
                        }

                    }
                    tempHash = record.Hash;
                }
                catch { continue; }
                
            }
            if (currentFrame != null && records.Count() > 0)
            {
                Source.Add(currentFrame);
            }
            
        }


        public Frame<Block> Select(EntryHash hash,Func<Frame<Block>> recover = null)
        {
            Frame<Block> item = new Frame<Block>(EntryHash.Empty);
            try
            {
                int index = Source.BinarySearch(new Frame<Block>(hash));
                if (index < 0)
                {
                    if(recover != null)
                        return recover();
                }
                return Source[index].Copy();
            }
            catch { }
            item.Add(new Block(DateTime.Now.AddYears(-15), 0, 999999, 0, hash.CostTypeId.ToString(), 0));
            return item;
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