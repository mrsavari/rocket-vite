using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Data;

namespace Rocket.Core
{
    public class GapRecord
    {

        public string Hash { get; set; }

        public int? From { get; set; }

        public int? To { get; set; }

        public string Payload { get; set; }

        public IEnumerable<Block> ToBlock()
        {
            return new List<Block>();
        }
    }
}