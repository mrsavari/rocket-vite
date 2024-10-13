using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class PipeFA : Unit, IObserver
    {
        public PipeFA(TimeSeries series, PipeFAOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public PipeFAOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.WaterPipe);
            IUsageSubject usage = Subject as IUsageSubject;
            
            InTime timeObj = usage.InTime();
            LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 5, 0, Options.SewageDiameterId, 0))
                                                           .Extract(new Link(timeObj.From, timeObj.To, null));
            if (rates.Count > 0)
            {
                Effect(new Transaction(TransactionSource.SewagePipe, TransactionType.Debit, rates.First.Value["5"].Value));
            }

            if (Options.Length > 0)
            {
                rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 31, 0, Options.SewageDiameterId, 0))
                                                              .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));

                Effect(new Transaction(TransactionSource.ExtraSewagePipe, TransactionType.Debit, rates.First.Value["31"].Value * Options.Length));
            }
        }
    }
}
