using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Core
{
    public class MergeFrame:IMutate<Block,IEnumerable<Block>>
    {
        public MergeFrame(Frame<Block> value)
        {
            Value = value;
        }

        public Frame<Block> Value { get; private set; }

        List<Block> Items = new List<Block>();

        public IEnumerable<Block> Excute(IEnumerable<Block> source)
        {
            Items = new List<Block>();
            List<DateTime> points = source.Concat(Value.ToList()).OrderBy((block) => { return block.Date; }).Select((item) => { return item.Date; }).Distinct().ToList();
            List<string> labels = source.Concat(Value.ToList()).OrderBy((block) => { return block.Label; }).Select((item) => { return item.Label; }).Distinct().ToList();
            List<Block> tmp = source.Concat(Value.ToList()).OrderBy((block) => { return block.Date; }).ToList();
            for (int i = 0; i < points.Count; i++)
            {
                foreach (string label in labels)
                {
                    Block item = source.Concat(Value.ToList()).OrderBy((block) => { return block.Date; }).Where((block) => { return block.Label == label && block.Date <= points[i]; }).LastOrDefault();
                    if (item == null)
                    {
                        Block instance = source.Concat(Value.ToList()).OrderBy((block) => { return block.Date; }).Where((block) => { return block.Label == label; }).LastOrDefault();
                        item = new Block(points[i], instance.FromRange, instance.ToRange, 0, label, 0);
                    }
                    Items.Add(new Block(points[i], item.FromRange, item.ToRange, item.Value, label, item.Extra));
                }
            }
            return Items;
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