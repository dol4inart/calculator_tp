using System;

namespace calculator.Model.Exceptions
{
    public class CurrentValueIsZeroException : DivideByZeroException
    {
        public CurrentValueIsZeroException(string message, double currentValue) : base(message)
        {
            CurrentValue = currentValue;
        }
        public double CurrentValue { get;}
    }
}
