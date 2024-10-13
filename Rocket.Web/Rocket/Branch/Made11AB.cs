using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;
using Rocket.Options;

namespace Rocket.Branch
{
    public class Made11AB : Unit, IObserver
    {
        public Made11AB(TimeSeries series, Made11Options options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public Made11Options Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.Made11AB);
            IUsageSubject usage = Subject as IUsageSubject;
            if (Options.IsVillage)
                return;
            int costTypeId = 0;
            if (Options.Karbari == 1)
                costTypeId = 18;
            else
                costTypeId = 17;
            InTime timeObj = usage.InTime();
            var ds = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, costTypeId, 0, 0, 0));
            if(Options.Karbari != 1)
                ds = ds.Transform(new InVolume(usage.Capacity));

            LinkedList<Dictionary<string, Block>> blocks = ds.Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));

            double baseCost = usage.Capacity * blocks.First.Value[costTypeId.ToString()].Value;
            baseCost += blocks.First.Value[costTypeId.ToString()].Extra;

            if (baseCost <= 0)
                return;

            

            Effect(new Transaction(TransactionSource.Made11AB, TransactionType.Debit, baseCost));

            double DiscountFactor = Options.TakhfifENAB;
            if (DiscountFactor > 0 && this.Balance > 0)
            {
                if (DiscountFactor >= 100)
                {
                    Effect(new Transaction(TransactionSource.Made11AB, TransactionType.Discount, this.Balance));
                }
                else
                {
                    Effect(new Transaction(TransactionSource.Made11AB, TransactionType.Discount, this.Balance * (DiscountFactor / 100)));
                }
            }

        }

    }
}