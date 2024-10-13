using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Core
{
    public class KarbariPartition
    {
        public KarbariPartition(int Id)
        {
            this.Id = Id;
            Items = new Dictionary<int, CostTypePartition>();
        }

        public Dictionary<int, CostTypePartition> Items { get; private set; }

        public int Id { get; private set; }

        public CostTypePartition this[int key]
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