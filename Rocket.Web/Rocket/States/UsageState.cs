using System;
using Rocket.Core;

namespace Rocket.States
{
    [Serializable]
    public abstract class UsageState: IObserver
    {
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
        protected IUsageSubject usageSubject;
        protected double consumption;
        protected double pattern;
        protected double maximum;
        protected double extra;

        public IUsageSubject Usage
        {
            get { return usageSubject; }
            set { usageSubject = value; }
        }
        public double Consumption
        {
            get { return consumption; }
            set { consumption = value; }
        }
        public virtual double GetExtraUsage()
        {
            return this.extra;
        }
        public abstract void Update(object sender, EventArgs e);
    }
}