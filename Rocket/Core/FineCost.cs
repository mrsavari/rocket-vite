using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Rocket.Core
{
    public class FineCost
    {
        public List<long> Items {get; private set;}
        public List<long> Estates {get; private set;}
        public FineCost(IEnumerable<long> items,IEnumerable<long> estates){
            this.Items = new List<long>(items);
            this.Estates = new List<long>(estates);
        }
        
        public bool IsSpecial(long key)
        {
            return Estates.Contains(key);
        }

        public bool Contains(long key)
        {
            return Items.Contains(key);
        }

    }
}