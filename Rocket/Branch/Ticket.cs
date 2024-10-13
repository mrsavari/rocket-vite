using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class Ticket : Unit, IObserver
    {
        public Ticket(TimeSeries series, TicketOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public TicketOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.Ticket);
            IUsageSubject usage = Subject as IUsageSubject;
            InTime timeObj = usage.InTime();
            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
            Frame<Block> TRABHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, Options.Tariff, 42, 0, Options.WaterDiameterId, 0));
            Frame<Block> TRABHash2 = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 19, 0, 0, 0));
            LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(TRABHash)).Transform(new MergeFrame(TRABHash2))
                                                .Extract(new Link(timeObj.From, timeObj.To, null));
            if (Series.Count() == 0)
                return;
            if (rates.Count > 0)
            {
                Effect(new Transaction(TransactionSource.Ticket, TransactionType.Debit, rates.First.Value["42"].Value));
                Effect(new Transaction(TransactionSource.TicketTax, TransactionType.Debit, rates.First.Value["42"].Value * rates.First.Value["19"].Value));
            }
        }
    }
}
