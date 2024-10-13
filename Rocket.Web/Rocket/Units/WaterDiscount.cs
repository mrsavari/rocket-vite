using System;
using System.Collections.Generic;
using System.Linq;
using Rocket;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Units
{
    public class WaterDiscount
    {
        public WaterDiscount() { }

        public WaterDiscount(TimeSeries D, int cityId, int karbari, int tariff, IUsageSubject usage, double price)
        {
            try
            {
                InTime timeObj = usage.InTime();
                Frame<Block> Hope = D.Select(new EntryHash(EntryType.Water, cityId, karbari, 25, 1, 0, 0)).Transform(new InVolume(usage.MonthlyAverage));
                LinkedListNode<Dictionary<string, Block>> node = Hope.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, usage)).First;
                if (new int[] { 24, 5 }.Contains(tariff) == false && node != null && node.Value["25"].Value > 0)
                {
                    this._Discount = node.Value["25"].Value;
                    this._Price = price * (node.Value["25"].Value / 100);
                }
            }
            catch { }
        }

        private double _Discount = 0;
        public double Discount { get { return _Discount; } }


        private double _Price = 0;
        public double Price { get { return _Price; } }
    }
}