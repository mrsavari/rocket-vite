using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Core
{
    public class InTime: IMutate<Block,IEnumerable<Block>>
    {
        public InTime(DateTime from,DateTime to)
        {
            From = from;
            To = to;
        }

        public DateTime From { get; private set; }
        public DateTime To { get; private set; }

        List<Block> Items = new List<Block>();
        public IEnumerable<Block> Excute(IEnumerable<Block> source)
        {
            Items = new List<Block>();
            TimeSpan duration = To - From;
            LinkedList<Block> formated = new LinkedList<Block>(source.OrderBy((item) => item.Date));
            if (formated.First.Value.Date > To)
                return Items;
            if (formated.Last.Value.Date < From)
            {
                List<string> labels = source.OrderBy((block) => { return block.Label; }).Select((item) => { return item.Label; }).Distinct().ToList();
                foreach (string label in labels)
                {
                    Block selected = source.Where((item) => { return item.Label == label; }).LastOrDefault();
                    Items.Add(new Block(From, selected.FromRange, selected.ToRange, selected.Value, selected.Label, selected.Extra));
                    Items.Add(new Block(To, selected.FromRange, selected.ToRange, selected.Value, selected.Label, selected.Extra));
                }
                return Items;
            }
            LinkedListNode<Block> current = formated.First;
            int maxLoop = 15;
            while (current != null)
            {
                if (current.Value.Date > To)
                {
                    Block selected = source.Where((item) => { return item.Label == current.Value.Label; }).LastOrDefault();
                    if (Items.Count > 0 && selected.Date != To)
                    {
                        Items.Add(new Block(To, current.Previous.Value.FromRange, current.Previous.Value.ToRange, current.Previous.Value.Value, current.Previous.Value.Label, current.Previous.Value.Extra));
                    }
                    break;
                }
                LinkedListNode<Block> next = current.Next;
                if (next != null)
                {
                    if (current.Value.Date < From)
                    {
                        if (next.Value.Date <= From)
                        {
                            current = current.Next;
                            maxLoop--;
                            continue;
                        }
                        else
                        {
                            Items.Add(new Block(From, current.Value.FromRange, current.Value.ToRange, current.Value.Value, current.Value.Label, current.Value.Extra));
                            current = current.Next;
                            maxLoop--;
                            continue;
                        }
                    }
                }
                Items.Add(current.Value);

                current = current.Next;
                maxLoop--;
                if (maxLoop == 0)
                    break;
            }
            if (Items.Last().Date != To)
            {
                if (Items.Count > 0)
                    Items.Add(new Block(To, formated.Last.Value.FromRange, formated.Last.Value.ToRange, formated.Last.Value.Value, formated.Last.Value.Label, formated.Last.Value.Extra));
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