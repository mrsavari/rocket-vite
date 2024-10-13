using System;
using System.Collections;
using System.Collections.Generic;

namespace Rocket.Core
{
    public static class Utils
    {
        public static int ToInt(this string instance){
            int number = 0;
            int.TryParse(instance,out number);
            return number;
        }
        
        public static double ToDouble(this int instance){
            double number = 0;
            double.TryParse(instance.ToString(),out number);
            return number;
        }
        public static double ToDouble(this string instance){
            double number = 0;
            double.TryParse(instance,out number);
            return number;
        }
        
        public static Frame<Block> ToFrame(this IEnumerable<Gap> items, string label)
        {
            Frame<Block> output = new Frame<Block>(new EntryHash(EntryType.Water, 0, 0, 0, 0, 0, 0));
            foreach (Gap item in items)
            {
                if (item.Ref == 2 || item.Ref == 3)
                {
                    string num = item.StartedAt.ToString().Trim();
                    if (num.Length == 8)
                    {
                        DateTime date = new DateTime(num.Substring(0, 4).ToInt(), num.Substring(4, 2).ToInt(), num.Substring(6, 2).ToInt());
                        output.Add(new Block(date, 0, 9999999, item.Payload.ToInt(), label, 0));
                    }
                    num = item.EndedAt.ToString().Trim();
                    if (num.Length == 8)
                    {
                        DateTime date = new DateTime(num.Substring(0, 4).ToInt(), num.Substring(4, 2).ToInt(), num.Substring(6, 2).ToInt());
                        output.Add(new Block(date, 0, 9999999, (item.Ref == 2)?1:0, label, 0));
                    }
                }
            }
            return output;
        }
        public static LinkedListNode<Dictionary<string, Block>> PriceSample(IEnumerable<string> labels,IUsageSubject usage,double value = 10,double extra=5){
            LinkedList<Dictionary<string, Block>> output = new LinkedList<Dictionary<string, Block>>();
            
            foreach(string label in labels){
                Dictionary<string, Block> record = new Dictionary<string, Block>();
                Block item = new Block(usage.StartedAt,0,99999,value,label,extra);
                item.SetDuration(usage.EndedAt);
                item.SetUsage(usage);
                record.Add(label,item);
                output.AddLast(record);
            }
            return output.First;
        }
    }
}