using calculator.Model.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator.Model
{
    public class CalculatorModel
    {
        public double CurrentValue { get; set; }
        public double PreviousValue { get; set; }
        public string Operation { get; set; }
        public bool IsOperationPending { get; set; }

        public void ExecuteOperation()
        {
            switch (Operation)
            {
                case "+":
                    CurrentValue = PreviousValue + CurrentValue;
                    break;
                case "-":
                    CurrentValue = PreviousValue - CurrentValue;
                    break;
                case "*":
                    CurrentValue = PreviousValue * CurrentValue;
                    break;
                case "/":
                    if (CurrentValue != 0)
                        CurrentValue = PreviousValue / CurrentValue;
                    else
                        throw new DivideByZeroException("Cannot divide by zero");
                    break;
            }
        }

        public void ExecuteFunction(string function)
        {
            switch (function)
            {
                case "sqrt":
                    CurrentValue = Math.Sqrt(CurrentValue);
                    break;
                case "x^2":
                    CurrentValue = Math.Pow(CurrentValue, 2);
                    break;
                case "1/x":
                    if (CurrentValue != 0)
                        CurrentValue = 1 / CurrentValue;
                    else
                        throw new CurrentValueIsZeroException("Cannot divide by zero",CurrentValue);
                    break;

                case "%":
                    CurrentValue = PreviousValue * (CurrentValue / 100);
                    break;
            }
        }

        public void Clear()
        {
            CurrentValue = 0;
            PreviousValue = 0;
            Operation = string.Empty;
            IsOperationPending = false;
        }
    }
}
