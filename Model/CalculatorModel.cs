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
                    /*try 
                    {*/
                        if (CurrentValue != 0)
                            CurrentValue = PreviousValue / CurrentValue;
                        else
                        {
                            throw new CurrentValueIsZeroException("Cannot divide by zero", PreviousValue);
                        }
                    /*}*/
                    

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
                            throw new NegativeSqrtException("NegativeSqrtException", CurrentValue);
                        }
                        CurrentValue = Math.Sqrt(CurrentValue);
                    }
                    catch (NegativeSqrtException)
                    {
                        MessageBox.Show("Взять квадратный корень из отрицательного числа нельзя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        Clear();
                    }

                    break;
                case "x^2":
                    CurrentValue = Math.Pow(CurrentValue, 2);
                    break;
                case "1/x":
                    try 
                    {
                        if (CurrentValue == 0)
                            throw new CurrentValueIsZeroException("Cannot divide by zero", CurrentValue);
                        else
                            CurrentValue = 1 / CurrentValue;

                    }
                    catch (CurrentValueIsZeroException)
                    {
                        MessageBox.Show("Деление на ноль невозможно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        Clear();
                    }
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
    public class Memory
    {
        public double Value { get; set; }
        public string Operation { get; set; }

        public Memory(double value, string operation)
        {
            Value = value;
            Operation = operation;
        }
    }
}
