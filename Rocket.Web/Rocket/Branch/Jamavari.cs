using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class Jamavari : Unit, IObserver
    {
        public Jamavari(TimeSeries series, JamavariOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public JamavariOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.Jamavari);
            IUsageSubject usage = Subject as IUsageSubject;

            int tariff = Options.Tariff == 11 ? 11 : 4;
            InTime timeObj = usage.InTime();
            LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, tariff, 66, 0, Options.WaterDiameterId, 0))
                                                           .Extract(new Link(timeObj.From, timeObj.To, null));
            if (rates.Count > 0)
            {
                Effect(new Transaction(TransactionSource.Jamavari, TransactionType.Debit, rates.First.Value["66"].Value));
            }
        }
    }
}
