using Rocket.Core;
using Rocket.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Branch
{
    public class WaterNasb : Unit, IObserver
    {
        public WaterNasb(TimeSeries series, WaterNasbOptions options)
            : base()
        {
            Options = options;
            DataSeries = series;
        }
        public WaterNasbOptions Options { get; set; }
        public TimeSeries DataSeries { get; set; }

        public override void Update(object sender, EventArgs e)
        {
            Reset(TransactionSource.WaterNasb);
            IUsageSubject usage = Subject as IUsageSubject;
            
            //if (!Options.HasWater())
            //    return;
            if (Options.KindBranchId == 3)
                return;
            Meter MeterObj = null;
            if (Options.Counter)
            {
                MeterOptions meterOptions = new MeterOptions() { CityId = Options.CityId, WaterDiameterId = Options.WaterDiameterId };
                MeterObj = new Meter(DataSeries, meterOptions);
                MeterObj.Subject = Subject;
                MeterObj.Update(sender, e);
            }
            Kit KitObj = null;
            if (Options.Kit)
            {
                KitOptions kitOptions = new KitOptions() { CityId = Options.CityId, WaterDiameterId = Options.WaterDiameterId };
                KitObj = new Kit(DataSeries, kitOptions);
                KitObj.Subject = Subject;
                KitObj.Update(sender, e);
            }
            Pipe PipeObj = null;
            if (Options.Pipe)
            {
                PipeOptions pipeOptions = new PipeOptions(Options.CityId, Options.WaterDiameterId);// { CityId = Options.CityId, WaterDiameterId = Options.WaterDiameterId };
                PipeObj = new Pipe(DataSeries, pipeOptions);
                PipeObj.Subject = Subject;
                PipeObj.Update(sender, e);
            }

            InTime timeObj = usage.InTime();
            Frame<Block> Series = new Frame<Block>(EntryHash.Empty);
            Frame<Block> TRABHash = DataSeries.Select(new EntryHash(EntryType.Branch, Options.CityId, 1, 20, 0, Options.WaterDiameterId, 0));
            LinkedList<Dictionary<string, Block>> rates = Series.Transform(new MergeFrame(TRABHash)).Extract(new Link(timeObj.From, timeObj.To, null));
            //D[TRABHash].Slice(options).GetBlocks(options);
            if (rates.Count > 0)
            {
                Effect(new Transaction(TransactionSource.WaterNasb, TransactionType.Debit, rates.First.Value["20"].Value));

                if (rates.First.Value["20"].Value  > 0)
                {
                    if (Options.Counter)
                    {
                        Effect(new Transaction(TransactionSource.WaterNasb, TransactionType.Credit, MeterObj.Balance, "Meter"));
                    }
                        //this.Update(new Credit(MeterCredit));
                    if (Options.Kit)
                    {
                        Effect(new Transaction(TransactionSource.WaterNasb, TransactionType.Credit, KitObj.Balance,"Kit"));
                    }
                        //this.Update(new Credit(KitCredit));
                    if (Options.Pipe)
                    {
                        Effect(new Transaction(TransactionSource.WaterNasb, TransactionType.Credit, PipeObj.Balance, "Pipe"));
                    }
                        //this.Update(new Credit(PipCredit));
                }
                //if (Balance.Type == MoneyType.Credit && Balance.Amount > 0)
                //    this.Update(new Debit(Balance.Amount));


                if (Options.TakhfifNasbAB > 0 && Balance > 0)
                {
                    if (Options.TakhfifNasbAB >= 100)
                    {
                        Effect(new Transaction(TransactionSource.WaterNasb, TransactionType.Discount, Balance));
                    }
                    else
                    {
                        Effect(new Transaction(TransactionSource.WaterNasb, TransactionType.Discount, Balance * (Options.TakhfifNasbAB / 100)));
                    }
                }
            }

            

        }
    }
}
