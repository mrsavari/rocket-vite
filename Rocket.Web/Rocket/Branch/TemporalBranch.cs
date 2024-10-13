using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class TemporalBranch : Unit, IObserver
    {
        public TemporalBranch(TimeSeries series, TemporalBranchOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public TemporalBranchOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.TemporalBranch);
            IUsageSubject usage = Subject as IUsageSubject;
            InTime timeObj = usage.InTime();
            LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 24, 0, Options.WaterDiameterId, 0))
                                                           .Extract(new Link(timeObj.From, timeObj.To, null));
            if (rates.Count > 0)
            {
                Effect(new Transaction(TransactionSource.TemporalBranch, TransactionType.Debit, rates.First.Value["24"].Value));
            }

        }
    }
}
