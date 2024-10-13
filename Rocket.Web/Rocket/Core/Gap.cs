using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Data;

namespace Rocket.Core
{
    public class Gap : IGap
    {
        public long GapId { get; set; }

        public int CityId { get; set; }

        public long SerialNo { get; set; }

        public int UserId { get; set; }

        public int StartedAt { get; set; }

        public int EndedAt { get; set; }

        public int Ref { get; set; }

        public string Payload { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public object FromGap(Gap instance)
        {
            return instance;
        }

        public IEnumerable<Block> ToBlock()
        {
            return new List<Block>();
        }
    }
}