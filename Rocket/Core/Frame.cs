using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Rocket.Core
{
    public class Frame<T> : IEnumerable<T>
    {
        public Frame(EntryHash hash)
        {
            Hash = hash;
            Items = new List<T>();
        }

        public void Add(T item,Func<List<T>,int> Eval = null)
        {
            if (Eval != null)
            {
                int index = Eval.Invoke(Items);
                if (index > 0)
                {
                    Items[index] = item;
                    return;
                }
            }
            Items.Add(item);
        }
        public void Add(IEnumerable<T> items)
        {
            Items.AddRange(items);
        }
        public EntryHash Hash { get; private set; }

        public List<T> Items { get; set; }

        public Frame<T> Transform(IMutate<T,IEnumerable<T>> filter)
        {
            IEnumerable<T> result = filter.Excute(Items);
            Items = new List<T>();
            Items.AddRange(result);
            return this;
        }
        public Frame<T> Copy()
        {
            Frame<T> list = new Frame<T>(this.Hash);
            foreach (T item in Items)
            {
                list.Add(item);
            }
            return list;
        }

        public U Extract<U>(IMutate<T,U> filter)
        {
            return filter.Excute(Items);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}