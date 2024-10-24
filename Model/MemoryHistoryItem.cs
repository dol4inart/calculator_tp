using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator.Model
{

        public class MemoryHistoryItem
        {
            public double Value { get; set; }
            public string Operation { get; set; }
            public DateTime Timestamp { get; set; }

            public MemoryHistoryItem(double value, string operation)
            {
                Value = value;
                Operation = operation;
                Timestamp = DateTime.Now;
            }



            public override string ToString()
            {
                return $"{Timestamp}: {Value} ({Operation})";
            }
        }
 }
