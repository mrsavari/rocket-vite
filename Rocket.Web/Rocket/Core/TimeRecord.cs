using System;

namespace Rocket.Core
{
    public class TimeRecord
    {
        public string Hash { get; set; }
        public DateTime Date { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public double Value { get; set; }
        public double Extra { get; set; }
        public string Label { get; set; }
    }
}