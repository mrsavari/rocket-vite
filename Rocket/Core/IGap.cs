using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Data;

namespace Rocket.Core
{
    public interface IGap
    {
        object FromGap(Gap instance);
        IEnumerable<Block> ToBlock();
        int Ref { get; }
    }
}