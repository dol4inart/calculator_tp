using calculator.Model;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace calculator.ViewModel
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorModel _calculatorModel;
        private string _currentInput;

        public string CurrentInput
        {
            get => _currentInput;
            set
            {
                if (_currentInput != value)
                {
                    _currentInput = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand NumberCommand { get; }
        public ICommand OperationCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand InvertSignCommand { get; }

        public CalculatorViewModel()
        {
            _calculatorModel = new CalculatorModel();
            _currentInput = "0";

            NumberCommand = new RelayCommand(OnNumberButtonPressed);
            OperationCommand = new RelayCommand(OnOperationButtonPressed);
            EqualsCommand = new RelayCommand(OnEqualsButtonPressed);
            ClearCommand = new RelayCommand(OnClearButtonPressed);
            ClearEntryCommand = new RelayCommand(OnClearEntryButtonPressed);
            BackspaceCommand = new RelayCommand(OnBackspaceButtonPressed);
            InvertSignCommand = new RelayCommand(OnInvertSignButtonPressed);
        }

        private void OnNumberButtonPressed(object parameter)
        {
            var number = parameter.ToString();
            if (CurrentInput == "0")
                CurrentInput = number;
            else
                CurrentInput += number;
        }

        private void OnOperationButtonPressed(object parameter)
        {
            _calculatorModel.PreviousValue = double.Parse(CurrentInput);
            _calculatorModel.Operation = parameter.ToString();
            CurrentInput = "0";
        }

        private void OnEqualsButtonPressed(object parameter)
        {
            _calculatorModel.CurrentValue = double.Parse(CurrentInput);
            _calculatorModel.ExecuteOperation();
            CurrentInput = _calculatorModel.CurrentValue.ToString();
        }

        private void OnClearButtonPressed(object parameter)
        {
            _calculatorModel.Clear();
            CurrentInput = "0";
        }

        private void OnClearEntryButtonPressed(object parameter)
        {
            CurrentInput = "0";
        }

        private void OnBackspaceButtonPressed(object parameter)
        {
            if (CurrentInput.Length > 1)
                CurrentInput = CurrentInput.Substring(0, CurrentInput.Length - 1);
            else
                CurrentInput = "0";
        }

        private void OnInvertSignButtonPressed(object parameter)
        {
            if (double.TryParse(CurrentInput, out double value))
                CurrentInput = (-value).ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
