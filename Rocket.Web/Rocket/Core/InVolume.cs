using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Core
{
    public class InVolume : IMutate<Block,IEnumerable<Block>>
    {
        public InVolume(double value)
        {
            Value = value;
        }

        public double Value { get; private set; }

        List<Block> Items = new List<Block>();
        public IEnumerable<Block> Excute(IEnumerable<Block> source)
        {
            Items = new List<Block>();
            Items.AddRange(source.Where((item) => { return item.InRange(Value); }));
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