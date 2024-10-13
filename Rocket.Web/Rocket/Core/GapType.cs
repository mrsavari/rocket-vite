using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Data;

namespace Rocket.Core
{
    public enum GapType
    {
        UnderConstruction =1,
        Family=2,
        Illegal =3,
        TemporalHousing=4
    }
}