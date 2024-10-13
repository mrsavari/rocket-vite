using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class RowWater : Unit, IObserver
    {
        public RowWater(TimeSeries series, RowWaterOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public RowWaterOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.RowWater);
            IUsageSubject usage = Subject as IUsageSubject;
            /* OutSide checking
            if (options.GetNumber("TariffUsesId") != 23)
                return;
            */
            InTime timeObj = usage.InTime();
            LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 28, 0, Options.WaterDiameterId, 0))
                                                           .Extract(new Link(timeObj.From, timeObj.To, null));
            if (rates.Count > 0)
            {
                Effect(new Transaction(TransactionSource.Pipe, TransactionType.Debit, rates.First.Value["28"].Value));
            }

            
        }
    }
}
