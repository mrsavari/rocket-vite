using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Core
{
    public class Entry : LinkedList<Block>
    {
        public static Entry operator +(Entry E1, Entry E2)
        {
            List<DateTime> points = E1.Concat(E2).OrderBy((block) => { return block.Date; }).Select((item) => { return item.Date; }).Distinct().ToList();
            List<string> labels = E1.Concat(E2).OrderBy((block) => { return block.Label; }).Select((item) => { return item.Label; }).Distinct().ToList();
            Entry output = new Entry();
            List<Block> tmp = E1.Concat(E2).OrderBy((block) => { return block.Date; }).ToList();
            for (int i = 0; i < points.Count; i++)
            {
                List<Block> records = new List<Block>();
                records.AddRange(E1.Concat(E2).Where((item)=>{ return item.Date == points[i];}).OrderBy((block) => { return block.Date; }).ToList());
                for(int j = 0; j <labels.Count; j++){
                    Block selected = records.Where((item) => { return item.Label == labels[j] && item.Date == points[i]; }).FirstOrDefault();
                    if (selected == null)
                    {
                        //search past 
                        Block passed = tmp.OrderByDescending((block) => { return block.Date; }).Where((item) => { return item.Label == labels[j] && item.Date < points[i]; }).FirstOrDefault();
                        if (passed == null)
                        {
                            passed = tmp.Where((item) => { return item.Label == labels[j]; }).FirstOrDefault();
                            records.Add(new Block(points[i], passed.FromRange, passed.ToRange, 0, passed.Label, 0));
                        }
                        else
                        {
                            records.Add(new Block(points[i], passed.FromRange, passed.ToRange, passed.Value, passed.Label, passed.Extra));
                        }
                    }
                }
                output.AddRange(records.OrderBy((block) => { return block.Label; }).ToList());
                   
            }
            
            return output;
           
        }

        public Entry AddRange(IEnumerable<Block> items)
        {
            foreach (Block item in items)
                this.AddLast(item);
            return this;
        }
        public Entry Slice(IUsageSubject options)
        {
            return Slice(options.StartedAt, options.EndedAt, -1);
        }

        public Entry Slice(IUsageSubject usage,bool inRange = true)
        {
            return Slice(usage.StartedAt, usage.EndedAt, inRange?usage.MonthlyAverage:-1);
        }

        public Entry Slice(DateTime fromDate, DateTime toDate, double range = -1)
        {
            if (range > -1)
            {
                List<Block> InRange = this.Where((item) => { return item.InRange(range); }).ToList();
                this.Clear();
                foreach (Block item in InRange)
                    this.AddLast(item);
            }
            if (this.Count == 0)
                return this;

            TimeSpan duration = toDate - fromDate;
            Entry output = new Entry();
            if (this.First.Value.Date > toDate)
                return output;
            if (this.Last.Value.Date < fromDate)
            {
                List<string> labels =this.OrderBy((block) => { return block.Label; }).Select((item) => { return item.Label; }).Distinct().ToList();
                foreach (string label in labels)
                {
                    Block selected = this.Where((item) => { return item.Label == label; }).LastOrDefault();
                    output.AddLast(new Block(fromDate, selected.FromRange, selected.ToRange, selected.Value, selected.Label, selected.Extra));
                    output.AddLast(new Block(toDate, selected.FromRange, selected.ToRange, selected.Value, selected.Label, selected.Extra));
                }
                
                return output;
            }
            LinkedListNode<Block> current = this.First;
            int maxLoop = 15;
            while (current != null)
            {
                if (range > -1 && current.Value.InRange(range) == false)
                {
                    current = current.Next;
                    maxLoop--;
                    continue;
                }
                if (current.Value.Date > toDate)
                {
                    if (output.Count > 0 && output.Last.Value.Date != toDate)
                    {
                        output.AddLast(new Block(toDate, current.Previous.Value.FromRange, current.Previous.Value.ToRange, current.Previous.Value.Value, current.Previous.Value.Label, current.Previous.Value.Extra));
                    }
                    break;
                }
                LinkedListNode<Block> next = current.Next;
                if (next != null)
                {
                    if (current.Value.Date < fromDate)
                    {
                        if (next.Value.Date <= fromDate)
                        {
                            current = current.Next;
                            maxLoop--;
                            continue;
                        }
                        else
                        {
                            output.AddLast(new Block(fromDate, current.Value.FromRange, current.Value.ToRange, current.Value.Value, current.Value.Label, current.Value.Extra));
                            current = current.Next;
                            maxLoop--;
                            continue;
                        }
                    }
                }
                output.AddLast(current.Value);

                current = current.Next;
                maxLoop--;
                if (maxLoop == 0)
                    break;
            }
            if (output.Last.Value.Date != toDate)
            {
                if (output.Count > 0)
                    output.AddLast(new Block(toDate, this.Last.Value.FromRange, this.Last.Value.ToRange, this.Last.Value.Value, this.Last.Value.Label, this.Last.Value.Extra));
            }
            return output;
        }

        public LinkedList<Dictionary<string, Block>> GetBlocks()
        {
            return GetBlocks(new UsageSubject(this.First().Date,this.Last().Date,1,0,0,0));
        }

        /*public LinkedList<Dictionary<string, Block>> GetBlocks(IUsageSubject opts)
        {
            LinkedList<Dictionary<string, Block>> output = new LinkedList<Dictionary<string, Block>>();
            List<DateTime> points = this.OrderBy((block) => { return block.Date; }).Select((item) => { return (item.Date > opts.EndedAt) ? opts.EndedAt : (item.Date < opts.StartedAt) ? opts.StartedAt : item.Date; }).Distinct().ToList();
            if (points.Count == 1)
            {
                List<Block> lst = this.Where((block) => { return block.Date == points[0]; }).ToList();
                Dictionary<string, Block> record = new Dictionary<string, Block>();
                foreach (Block item in lst)
                    if(!record.ContainsKey(item.Label))
                        record.Add(item.Label, item);
                output.AddLast(record);
                return output;
            }
            for (int now = 0; now + 1 < points.Count; now++)
            {
                List<Block> lst = this.Where((block) => { return block.Date == points[now]; }).ToList();
                Dictionary<string, Block> record = new Dictionary<string, Block>();
                foreach (Block item in lst)
                {
                    record.Add(item.Label, item);
                }
                if (record.Count > 0)
                    output.AddLast(record);
            }
            return output;
        }*/

        public LinkedList<Dictionary<string, Block>> GetBlocks(IUsageSubject opts)
        {
            LinkedList<Dictionary<string, Block>> output = new LinkedList<Dictionary<string, Block>>();
            List<DateTime> points = this.OrderBy((block) => { return block.Date; }).Select((item) => { return (item.Date > opts.EndedAt) ? opts.EndedAt : (item.Date < opts.StartedAt) ? opts.StartedAt : item.Date; }).Distinct().ToList();
            for (int now = 0; now+1 < points.Count;now++ )
            {
                double fullrange = opts.Duration.Days;
                IUsageSubject usgInstance= opts.SetTime(((points[now] >= opts.StartedAt) ? points[now] : opts.StartedAt), ((points[now + 1] <= opts.EndedAt) ? points[now + 1] : opts.EndedAt));
                double duration = (((points[now + 1] <= opts.EndedAt) ? points[now + 1] : opts.EndedAt) - ((points[now] >= opts.StartedAt) ? points[now] : opts.StartedAt)).Days;
                //Usage usgInstance = new Usage(new Quantum(opts.Usage.Amount.GetValue() / fullrange * duration), new Quantum(opts.Usage.Capacity.GetValue() / fullrange * duration), new TimeSpan(duration.ToString().ToInt(), 0, 0, 0, 0), Concepts.UsageType.Periodic);

                List<Block> lst = this.Where((block) => { return block.Date == points[now]; }).ToList();
                Dictionary<string, Block> record = new Dictionary<string, Block>();
                foreach (Block item in lst)
                {
                    item.SetDuration(usgInstance.Duration);
                    item.SetUsage(usgInstance);
                    record.Add(item.Label, item);
                }
                if (record.Count > 0)
                    output.AddLast(record);
            }
            return output;
        }

    }
}