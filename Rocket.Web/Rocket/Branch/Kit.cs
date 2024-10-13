using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class Kit : Unit, IObserver
    {
        public Kit(TimeSeries series, KitOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public KitOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.Ticket);
            IUsageSubject usage = Subject as IUsageSubject;
            InTime timeObj = usage.InTime();
            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
            Frame<Block> TRABHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 26, 0, Options.WaterDiameterId, 0));
            LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(TRABHash)).Extract(new Link(timeObj.From, timeObj.To, null));
            if (Series.Count() == 0)
                return;
            if (rates.Count > 0)
            {
                Effect(new Transaction(TransactionSource.Ticket, TransactionType.Debit, rates.First.Value["26"].Value));
            }

        }
    }
}
