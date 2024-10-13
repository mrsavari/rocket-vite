using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class BranchTax : Unit, IObserver
    {
        public BranchTax(TimeSeries series, BranchTaxOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public BranchTaxOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.BranchTax);
            IUsageSubject usage = Subject as IUsageSubject;
            InTime timeObj = usage.InTime();
            LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 19, 0, 0, 0))
                                                           .Extract<LinkedList<Dictionary<string, Block>>>(new Link(timeObj.From, timeObj.To, null));
            if (rates.Count > 0)
            {
                Effect(new Transaction(TransactionSource.BranchTax, TransactionType.Debit, rates.First.Value["19"].Value * Options.Amount));
            }
            
        }
    }
}
