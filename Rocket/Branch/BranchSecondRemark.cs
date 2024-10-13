using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class BranchSecondRemark : Unit, IObserver
    {
        public BranchSecondRemark(TimeSeries series, BranchSecondRemarkOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public BranchSecondRemarkOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.RowWater);
            IUsageSubject usage = Subject as IUsageSubject;
            IHasTransactions ts = Subject as IHasTransactions;
            if (Options.IsVillage)
                return;
            /*string KindBranchId = options.Get("KindBranchId");
            if (KindBranchId != "2")
                return;
            if (options.GetNumber("TariffUsesId") == 23)
                return;*/
            if (Options.TakhfifENAB < 0)
                Options.TakhfifENAB = 0;
            InTime timeObj = usage.InTime();
            LinkedList<Dictionary<string, Block>> rates = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 10, 0, 0, 0))
                                                           .Extract(new Link(timeObj.From, timeObj.To, null));
            if (rates.Count > 0)
            {
                double result = ts.Debit(TransactionSource.WaterMembershipFee) * rates.First.Value["10"].Value / 100;
                Effect(new Transaction(TransactionSource.BranchSecondRemark, TransactionType.Debit, result));

                if (Options.TakhfifENAB > 0 && result > 0)
                {
                    if (Options.TakhfifENAB >= 100)
                    {
                        Effect(new Transaction(TransactionSource.BranchSecondRemark, TransactionType.Discount, result));
                    }
                    else
                    {
                        Effect(new Transaction(TransactionSource.BranchSecondRemark, TransactionType.Discount, result * (Options.TakhfifENAB / 100)));
                    }
                }
            }

        }
    }
}
