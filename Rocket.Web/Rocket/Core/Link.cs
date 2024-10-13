using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Core
{
    public class Link:IMutate<Block,LinkedList<Dictionary<string,Block>>>
    {
        public Link(DateTime from, DateTime to ,IUsageSubject usage,double defaultValue=0)
        {
            From = from;
            To = to;
            Usage = usage;
            DefaultValue = defaultValue;
        }

        public DateTime From { get; private set; }
        public DateTime To { get; private set; }
        public IUsageSubject Usage { get; private set; }
        private double DefaultValue=0;
        List<Block> Items = new List<Block>();

        public LinkedList<Dictionary<string, Block>> Excute(IEnumerable<Block> source)
        {
            List<DateTime> bases = new List<DateTime>();
            bases.Add(From);
            bases.Add(To);
            int fullrange = (To - From).Days;
            LinkedList<Dictionary<string, Block>> output = new LinkedList<Dictionary<string, Block>>();
            List<DateTime> points = source.OrderBy((block) => { return block.Date; }).Select((item) => { return (item.Date > To) ? To : (item.Date < From) ? From : item.Date; }).Concat((IEnumerable<DateTime>)bases).Distinct().OrderBy((item) => { return item; }).ToList();
            List<string> labels = source.OrderBy((block) => { return block.Label; }).Select((item) => { return item.Label; }).Distinct().ToList();
            if (points.Count == 1)
            {
                source.OrderBy((block) => { return block.Date; }).LastOrDefault();
                Dictionary<string, Block> record = new Dictionary<string, Block>();
                foreach (string label in labels)
                {
                    Block item = source.OrderBy((block) => { return block.Date; }).Where((block) => { return block.Label == label && block.Date <= points[0]; }).LastOrDefault();
                    if (item == null)
                    {
                        item = source.OrderBy((block) => { return block.Date; }).Where((block) => { return block.Label == label; }).FirstOrDefault();
                        if (item == null)
                        {
                            item = new Block(From, 0, 999999, DefaultValue, label, 0);
                        }
                    }
                    record.Add(label, new Block(From, item.FromRange, item.ToRange, item.Value, label, item.Extra));
                    record[label].SetDuration(To);
                }
                output.AddLast(record);
                return output;
            }
            for (int i=0; i+1 <points.Count; i++)
            {
                Dictionary<string, Block> record = new Dictionary<string, Block>();
                foreach (string label in labels)
                {
                    Block item = source.OrderBy((block) => { return block.Date; }).Where((block) => { return block.Label == label && block.Date <= points[i]; }).LastOrDefault();
                    if (item == null)
                    {
                        Block instance = source.OrderBy((block) => { return block.Date; }).Where((block) => { return block.Label == label; }).LastOrDefault();
                        item = new Block(points[i], instance.FromRange, instance.ToRange, DefaultValue, label, 0);
                    }
                    record.Add(label, new Block(points[i], item.FromRange, item.ToRange, item.Value, label, item.Extra));

                    if (points.Count > i+1 )
                    {
                        record[label].SetDuration(points[i + 1]);
                        if (Usage != null)
                        {
                            record[label].SetUsage(Usage.SetTime(record[label].Date,points[i + 1]));
                        }
                    }
                }
                if(record.Count>0)
                    output.AddLast(record);
            }
            return output;
        }

        public IEnumerator<Block> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}