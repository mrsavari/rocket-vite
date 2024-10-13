using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;
using Rocket.States;

namespace Rocket
{
    [Serializable]
    public class UsageSubject : IUsageSubject, ITimeRange, IHasTransactions
    {
        public UsageSubject(DateTime startedAt,DateTime endedAt,int karbari,double? consumption = null,double? monthlyAverage = null,double? capacity = null){
            Transactions = new List<Transaction>();
            this.startedAt = startedAt;
            this.endedAt = endedAt;
            this.karbari = karbari;
            if(consumption != null){
                this.consumption = consumption.Value;
                if (monthlyAverage == null || monthlyAverage.Value == 0)
                {
                    this.monthlyAverage = this.consumption / (this.EndedAt - this.StartedAt).Days.ToDouble() * 30;
                }
                else
                {
                    this.monthlyAverage = monthlyAverage.Value;
                }
            }else{
                if(monthlyAverage != null){
                    this.monthlyAverage = monthlyAverage.Value;
                }else{
                    throw new ArgumentNullException("Consumption & MonthlyAverage Both Are Not Supplied! One Is Required");
                }
            }
            this.capacity = capacity != null ?capacity.Value:0;
        }

        public List<Transaction> Transactions { get; set; }

        public double Balance(TransactionSource? source)
        {
            double value = 0.0;
            IEnumerable<Transaction> targets = Transactions.Where((item) => item.Type != TransactionType.Transit);
            if (source != null)
            {
                targets = Transactions.Where((item) => item.Type != TransactionType.Transit && item.Source == source.Value);
            }

            foreach (Transaction item in targets)
            {
                value += ((item.Type == TransactionType.Debit) ? item.Amount : (item.Amount * -1));
            }
            return value;
        }
        public double Transit(TransactionSource? source)
        {
            double value = 0.0;
            IEnumerable<Transaction> targets = Transactions.Where((item) => item.Type == TransactionType.Transit);
            if (source != null)
            {
                targets = Transactions.Where((item) => item.Type == TransactionType.Transit && item.Source == source.Value);
            }

            foreach (Transaction item in targets)
            {
                value += item.Amount;
            }
            return value;
        }
        public double Transit(TransactionSource? source, Func<string, bool> filter)
        {
            double value = 0.0;
            IEnumerable<Transaction> targets = Transactions.Where((item) => item.Type == TransactionType.Transit);
            if (source != null)
            {
                targets = Transactions.Where((item) => item.Type == TransactionType.Transit && item.Source == source.Value);
            }

            foreach (Transaction item in targets)
            {
                if (filter(item.Description))
                value += item.Amount;
            }
            return value;
        }

        public event EventHandler OnNotify;
        
        public void Notify(){
            if (OnNotify != null) OnNotify(this, new EventArgs());
        }

        public UsageState State { get; set; }

        private int karbari;
        public int Karbari
        {
            get { return karbari; }
            set
            {
                karbari = value;
                if (OnNotify != null) OnNotify(this, new EventArgs());
            }
        }

        private double consumption;
        public double Consumption
        {
            get { return consumption; }
            set
            {
                consumption = value;
                if (OnNotify != null) OnNotify(this, new EventArgs());
            }
        }

        private double monthlyAverage;
        public double MonthlyAverage
        {
            get { return monthlyAverage; }
            set
            {
                monthlyAverage = value;
                if (OnNotify != null) OnNotify(this, new EventArgs());
            }
        }

        private double capacity;
        public double Capacity
        {
            get { return capacity; }
            set
            {
                capacity = value;
                if (OnNotify != null) OnNotify(this, new EventArgs());
            }
        }

        private DateTime startedAt;
        public DateTime StartedAt
        {
            get { return startedAt; }
            set
            {
                startedAt = value;
                if (OnNotify != null) OnNotify(this, new EventArgs());
            }
        }

        private DateTime endedAt;
        public DateTime EndedAt
        {
            get { return endedAt; }
            set
            {
                endedAt = value;
                if (OnNotify != null) OnNotify(this, new EventArgs());
            }
        }

        public TimeSpan Duration
        {
            get { return EndedAt - StartedAt; }
        }

        public GapSeries Gaps { get; set; }

        public UsageSubject Clone(DateTime? startedAt = null,DateTime? endedAt = null,int? karbari = null,double? consumption = null,double? monthlyAverage = null,double? capacity = null){
            if(karbari==null)
                karbari = Karbari;
            if(startedAt==null)
                startedAt = StartedAt;
            if(endedAt==null)
                endedAt = EndedAt;
            if (capacity == null)
                capacity = Capacity;
            if(consumption != null && monthlyAverage == null){
                monthlyAverage = consumption / (endedAt - startedAt).Value.Days * 30;
            }
            if(consumption == null && monthlyAverage != null){
                consumption = monthlyAverage / 30 * (endedAt - startedAt).Value.Days;
            }
            UsageSubject cloned = new UsageSubject(
                startedAt.Value,
                endedAt.Value,
                karbari.Value,
                consumption.Value,
                monthlyAverage.Value,
                capacity.Value
                );
            cloned.State = this.State;
            cloned.Gaps = this.Gaps;
            return cloned;
        }

        public UsageSubject SetTime(DateTime? startedAt,DateTime? endedAt){
            if(startedAt == null)
                startedAt = this.StartedAt;
            if(endedAt == null)
                endedAt = this.EndedAt;
            TimeSpan localDuration = (endedAt - startedAt).Value;
            double changeRate = 0;
            if(localDuration.Days < Duration.Days){
                changeRate = localDuration.Days.ToDouble() / Duration.Days.ToDouble();
            }else{
                changeRate = Duration.Days.ToDouble() / localDuration.Days.ToDouble();
            }
            double localConsumption = Consumption * changeRate;
            double localMonthlyAverage = localConsumption / localDuration.Days.ToDouble() * 30;
            UsageSubject newUsage = new UsageSubject(startedAt.Value,endedAt.Value,this.Karbari,localConsumption,localMonthlyAverage,this.capacity);
            newUsage.State = this.State;
            newUsage.Gaps = this.Gaps;
            return newUsage;
        }

        public InTime InTime()
        {
            return new InTime(this.StartedAt, this.EndedAt);
        }

        public UsageView GetView()
        {
            UsageView view = new UsageView();
            view.Consumption = this.Consumption;
            view.MonthlyAverage = this.MonthlyAverage;
            view.ExtraUsage = this.State.GetExtraUsage();
            foreach (Transaction item in Transactions.Where((item) => item.Type != TransactionType.Transit))
                view.Transactions.Add(item);
            return view;
        }

        public double Debit(TransactionSource source)
        {
            return Transactions.Where(item => item.Source == source && item.Type == TransactionType.Debit).Sum(item => item.Amount);
        }
    }
}