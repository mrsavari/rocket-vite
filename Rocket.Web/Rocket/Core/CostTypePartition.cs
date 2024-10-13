using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Core
{
    public class CostTypePartition
    {
        public CostTypePartition(int Id)
        {
            this.Id = Id;
            Items = new Dictionary<int, Entry>();
        }

        public Dictionary<int, Entry> Items { get; private set; }

        public int Id { get; private set; }

        public Entry this[int key]
        {
            get
            {
                return Items[key];
            }
            set
            {
                Items[key] = value;
            }
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

    }
}