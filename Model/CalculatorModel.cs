using calculator.Model.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                    try
                    {
                        if (CurrentValue == 0)
                            ThrowException("Cannot divide by zero", CurrentValue, Operation);
                        else
                            CurrentValue = 1 / CurrentValue;

                    }
                    catch { }


                    break;
            }
        }

        public void ExecuteFunction(string function)
        {
            switch (function)
            {
                case "sqrt":
                    try
                    {
                        if (CurrentValue < 0)
                        {
                            ThrowException("NegativeSqrtException", CurrentValue, function);
                        }
                        CurrentValue = Math.Sqrt(CurrentValue);
                    }
                    catch
                    { }

                    break;
                case "x^2":
                    CurrentValue = Math.Pow(CurrentValue, 2);
                    break;
                case "1/x":
                    try 
                    {
                        if (CurrentValue == 0)
                            ThrowException("Cannot divide by zero", CurrentValue, function);
                        else
                            CurrentValue = 1 / CurrentValue;

                    }
                    catch { }

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

        private void ThrowException(string message, double CurrentValue, string function)
        {
            if (function == "1/x" || function == "/")
            {
                throw new CurrentValueIsZeroException(message, CurrentValue);
            }
            else if (function == "sqrt")
            {
                throw new NegativeSqrtException(message, CurrentValue);
            }

        }

    }
}
