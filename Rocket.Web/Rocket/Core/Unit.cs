using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Core
{
    public class Unit : IObserver
    {
        public Unit(){
            Transactions = new List<Transaction>();
        }

        public ISubject subject;
        public ISubject Subject
        {
            get { return subject; }
            set
            {
                // Relase old event
                if (subject != null) subject.OnNotify -= Update;

                subject = value;

                // Connect new event
                if (subject != null) subject.OnNotify += Update;
            }
        }

        public void Disconnect()
        {
            if (subject != null) 
                subject.OnNotify -= Update;
        }

        public virtual void Update(object sender, EventArgs e) { 
            
        }

        public double Balance {
            get{
                return Debit - (Credit + Discount);
            }
        }

        public double Debit {
            get{
                double value =0.0;
                foreach(Transaction item in Transactions.Where((item)=>item.Type == TransactionType.Debit)){
                    value += item.Amount;
                }
                return value;
            }
        }

        public double Credit {
            get{
                double value =0.0;
                foreach(Transaction item in Transactions.Where((item)=>item.Type == TransactionType.Credit)){
                    value += item.Amount;
                }
                return value;
            }
        }

        public double Discount {
            get{
                double value =0.0;
                foreach(Transaction item in Transactions.Where((item)=>item.Type == TransactionType.Discount)){
                    value += item.Amount;
                }
                return value;
            }
        }

        public List<Transaction> Transactions{get; set;}

        public List<Transaction> Effect(Transaction item){
            (Subject as IHasTransactions).Transactions.Add(item);
            Transactions.Add(item);
            return Transactions;
        }

        public void Reset(TransactionSource? source=null){
            if (source != null)
            {
                List<Transaction> selected = new List<Transaction>();
                selected.AddRange((Subject as IHasTransactions).Transactions.Where(item => item.Source == source.Value));
                foreach (Transaction item in selected)
                {
                    (Subject as IHasTransactions).Transactions.Remove(item);
                }
            }
            Transactions = new List<Transaction>();
        }

    }
}