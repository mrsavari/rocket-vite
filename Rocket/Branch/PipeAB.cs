using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class PipeAB : Unit, IObserver
    {
        public PipeAB(TimeSeries series, PipeABOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public PipeABOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.WaterPipe);
            IUsageSubject usage = Subject as IUsageSubject;
            
            InTime timeObj = usage.InTime();
            LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 27, 0, Options.WaterDiameterId, 0))
                                                           .Extract(new Link(timeObj.From, timeObj.To, null));
            if (rates.Count > 0)
            {
                Effect(new Transaction(TransactionSource.WaterPipe, TransactionType.Debit, rates.First.Value["27"].Value));
            }

            if (Options.Length > 0)
            {
                rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 21, 0, Options.WaterDiameterId, 0))
                                                              .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));

                Effect(new Transaction(TransactionSource.ExtraWaterPipe, TransactionType.Debit, rates.First.Value["21"].Value * Options.Length));
            }
        }
    }
}
