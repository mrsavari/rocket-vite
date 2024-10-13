using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Core
{
    public class Block
    {
        public Block(DateTime date, int from, int to, double value, string label, double extra = 0)
        {
            Date = date;
            FromRange = from;
            ToRange = to;
            Value = value;
            Label = label;
            Extra = extra;
        }
        public string Label { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; private set; }
        public int FromRange { get; set; }
        public int ToRange { get; set; }
        public double Value { get; set; }
        public double Extra { get; set; }
        public IUsageSubject Usage { get; private set; }

        public Block SetUsage(IUsageSubject usage)
        {
            this.Usage = usage;
            return this;
        }

        public Block SetDuration(DateTime to)
        {
            this.Duration = to - Date;
            return this;
        }

        public Block SetDuration(TimeSpan duration)
        {
            this.Duration = duration;
            return this;
        }

        public bool InRange(double value)
        {
            if (FromRange == 0 && value == 0)
                return true;
            if (value <= FromRange || value > ToRange)
                return false;
            return true;
        }
    }
}