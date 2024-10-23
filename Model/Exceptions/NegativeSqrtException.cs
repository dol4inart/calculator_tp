using System;


namespace calculator.Model.Exceptions
{
    public class NegativeSqrtException : Exception
    {
        public NegativeSqrtException(string message, double currentValue)
        {
            CurrentValue = currentValue;
        }

        public double CurrentValue { get; }
    }
}

