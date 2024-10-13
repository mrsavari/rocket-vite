using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class SewageNasb : Unit, IObserver
    {
        public SewageNasb(TimeSeries series, SewageNasbOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public SewageNasbOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.SewageNasb);
            IUsageSubject usage = Subject as IUsageSubject;
            IHasTransactions ts = Subject as IHasTransactions;

            if (Options.IsVillage)
                return;
            /*if (!Options.HasSewage())
                return;*/
            if (Options.KindBranchId == 2)
                return;
            if (Options.Tariff == 23)
                return;
            if (new double[] { 7, 34, 33 }.Contains(Options.Karbari))
                return;

            double SifonEzafe = Options.SifonEzafe;
            if (SifonEzafe < 1)
                SifonEzafe = 1;
            SifonEzafe = (Options.SifonEzafe / Options.EstateCount);
            InTime timeObj = usage.InTime();
            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
            Frame<Block> TRABHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 30, 0, Options.SewageDiameterId, 0));
            Frame<Block> TRABHash2 = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 35, 0, Options.SewageDiameterId, 0));
            LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(TRABHash)).Transform(new MergeFrame(TRABHash2))
                                                .Extract(new Link(timeObj.From, timeObj.To, null));

            if (rates.Count > 0)
            {
                double result = rates.First.Value["30"].Value * SifonEzafe;
                if (Options.ToolsByCustomer && rates.First.Value["35"].Value > 0)
                {
                    Effect(new Transaction(TransactionSource.SewageNasb, TransactionType.Credit, rates.First.Value["35"].Value * SifonEzafe, "SifonTools"));
                }

                Effect(new Transaction(TransactionSource.SewageNasb, TransactionType.Debit, result));

                if (Options.TakhfifNasbFA > 0 && result > 0)
                {
                    if (Options.TakhfifNasbFA >= 100)
                    {
                        Effect(new Transaction(TransactionSource.SewageNasb, TransactionType.Discount, Balance));
                    }
                    else
                    {
                        Effect(new Transaction(TransactionSource.SewageNasb, TransactionType.Discount, Balance * (Options.TakhfifNasbFA / 100)));
                    }
                }
            }

        }
    }
}
