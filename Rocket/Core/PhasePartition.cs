using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Core
{
    public class PhasePartition : LinkedList<Block>
    {
        public PhasePartition(int Id)
        {
            this.Id = Id;
        }

        public int Id { get; private set; }

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