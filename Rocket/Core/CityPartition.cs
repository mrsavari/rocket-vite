using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Core
{
    public class CityPartition
    {
        public CityPartition(int Id)
        {
            this.Id = Id;
            Items = new Dictionary<int, KarbariPartition>();
        }

        public Dictionary<int, KarbariPartition> Items { get; private set; }

        public int Id { get; private set; }

        public KarbariPartition this[int key]
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