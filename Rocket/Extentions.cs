using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Data;
using Rocket.Core;

namespace Rocket
{
    public static class Extentions
    {
        public static double GetTax(this TimeSeries DataSeries,double amount, int cityId, DateTime? time = null)
        {
            if (time == null)
                time = DateTime.Now;
            LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, cityId, 1, 19, 0, 0, 0))
                                                           .Extract<LinkedList<Dictionary<string, Block>>>(new Link(time.Value, time.Value, null));
            if (rates.Count > 0)
            {
                return rates.First.Value["19"].Value * amount;
            }
            return 0;
        }

        public static double Discount(this double amount, double varint)
        {
            if (varint > 0 && amount > 0)
            {
                if (varint >= 1)
                {
                    return 0;
                }
                else
                {
                    return amount * varint;
                }
            }
            return amount;
        }
    }
}