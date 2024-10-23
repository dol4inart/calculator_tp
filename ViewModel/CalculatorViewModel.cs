using calculator.Command;
using calculator.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
        public ICommand FunctionCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand InvertSignCommand { get; }
        public ICommand MemoryClearCommand { get; }
        public ICommand MemoryRecallCommand { get; }
        public ICommand MemorySaveCommand { get; }
        public ICommand MemoryAddCommand { get; }
        public ICommand MemorySubtractCommand { get; }
        public ICommand ShowMemoryHistoryCommand { get; }

        public double MemoryValue { get; private set; }
        public MemoryHistoryViewModel MemoryHistoryViewModel { get; }


        public CalculatorViewModel()
        {

            _calculatorModel = new CalculatorModel();
            _currentInput = "0";

            MemoryHistoryViewModel = new MemoryHistoryViewModel();
            MemoryClearCommand = new RelayCommand(OnMemoryClear);
            MemoryRecallCommand = new RelayCommand(OnMemoryRecall);
            MemorySaveCommand = new RelayCommand(OnMemorySave);
            MemoryAddCommand = new RelayCommand(OnMemoryAdd);
            MemorySubtractCommand = new RelayCommand(OnMemorySubtract);
            ShowMemoryHistoryCommand = new RelayCommand(OnShowMemoryHistory);

            NumberCommand = new RelayCommand(OnNumberButtonPressed);
            OperationCommand = new RelayCommand(OnOperationButtonPressed);
            FunctionCommand = new RelayCommand(OnFunctionButtonPressed);
            EqualsCommand = new RelayCommand(OnEqualsButtonPressed);
            ClearCommand = new RelayCommand(OnClearButtonPressed);
            ClearEntryCommand = new RelayCommand(OnClearEntryButtonPressed);
            BackspaceCommand = new RelayCommand(OnBackspaceButtonPressed);
            InvertSignCommand = new RelayCommand(OnInvertSignButtonPressed);
        }


        private void OnMemoryClear(object parameter)
        {
            MemoryValue = 0;
            MessageBox.Show("Memory cleared.");
        }

        private void OnMemoryRecall(object parameter)
        {
            CurrentInput = MemoryValue.ToString();
        }

        private void OnMemorySave(object parameter)
        {
            MemoryValue = double.Parse(CurrentInput);
            MemoryHistoryViewModel.AddMemoryItem(MemoryValue, "Saved");
            MessageBox.Show($"Memory saved: {MemoryValue}");
        }

        private void OnMemoryAdd(object parameter)
        {
            MemoryValue += double.Parse(CurrentInput);
            MemoryHistoryViewModel.AddMemoryItem(MemoryValue, "Added");
            MessageBox.Show($"Memory value: {MemoryValue}");
        }

        private void OnMemorySubtract(object parameter)
        {
            MemoryValue -= double.Parse(CurrentInput);
            MemoryHistoryViewModel.AddMemoryItem(MemoryValue, "Subtracted");
            MessageBox.Show($"Memory value: {MemoryValue}");
        }

        private void OnShowMemoryHistory(object parameter)
        {
            var memoryHistoryWindow = new MemoryHistoryWindow();
            memoryHistoryWindow.DataContext = MemoryHistoryViewModel; // Передаем контекст
            memoryHistoryWindow.ShowDialog();
        }

        public void OnNumberButtonPressed(object parameter)
        {
            var number = parameter.ToString();
            if (CurrentInput == "0")
                CurrentInput = number;
            else
                CurrentInput += number;
        }

        public void OnOperationButtonPressed(object parameter)
        {
            if (_calculatorModel.IsOperationPending)
            {
                _calculatorModel.CurrentValue = double.Parse(CurrentInput);
                _calculatorModel.ExecuteOperation();
                CurrentInput = _calculatorModel.CurrentValue.ToString();
            }

            _calculatorModel.PreviousValue = double.Parse(CurrentInput);
            _calculatorModel.Operation = parameter.ToString();
            _calculatorModel.IsOperationPending = true;
            CurrentInput = "0";
        }

        public void OnFunctionButtonPressed(object parameter)
        {
            var function = parameter.ToString();
            if (_calculatorModel.IsOperationPending && function != "%")
            {
                _calculatorModel.CurrentValue = double.Parse(CurrentInput);
                _calculatorModel.ExecuteOperation();
                CurrentInput = _calculatorModel.CurrentValue.ToString();
                _calculatorModel.IsOperationPending = false;
            }
            else
            {
                if (function == "%")
                {
                    _calculatorModel.CurrentValue = double.Parse(CurrentInput);
                    _calculatorModel.ExecuteFunction(function);
                    CurrentInput = _calculatorModel.CurrentValue.ToString();
                }
                else
                {
                    _calculatorModel.CurrentValue = double.Parse(CurrentInput);
                    _calculatorModel.ExecuteFunction(function);
                    CurrentInput = _calculatorModel.CurrentValue.ToString();
                }
            }
        }

        public void OnEqualsButtonPressed(object parameter)
        {
            _calculatorModel.CurrentValue = double.Parse(CurrentInput);
            _calculatorModel.ExecuteOperation();
            CurrentInput = _calculatorModel.CurrentValue.ToString();
            _calculatorModel.IsOperationPending = false;
        }

        public void OnClearButtonPressed(object parameter)
        {
            _calculatorModel.Clear();
            CurrentInput = "0";
        }

        public void OnClearEntryButtonPressed(object parameter)
        {
            CurrentInput = "0";
        }

        public void OnBackspaceButtonPressed(object parameter)
        {
            if (CurrentInput.Length > 1)
                CurrentInput = CurrentInput.Substring(0, CurrentInput.Length - 1);
            else
                CurrentInput = "0";
        }

        public void OnInvertSignButtonPressed(object parameter)
        {
            if (double.TryParse(CurrentInput, out double value))
                CurrentInput = (-value).ToString();
        }

        public void ProcessKeyPress(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
            {
                OnNumberButtonPressed((key - Key.D0).ToString());
            }
            else if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                OnNumberButtonPressed((key - Key.NumPad0).ToString());
            }
            else if (key == Key.Add)
            {
                OnOperationButtonPressed("+");
            }
            else if (key == Key.Subtract)
            {
                OnOperationButtonPressed("-");
            }
            else if (key == Key.Multiply)
            {
                OnOperationButtonPressed("*");
            }
            else if (key == Key.Divide)
            {
                OnOperationButtonPressed("/");
            }
            else if (key == Key.Enter)
            {
                OnEqualsButtonPressed(null);
            }
            else if (key == Key.Back)
            {
                OnBackspaceButtonPressed(null);
            }
            else if (key == Key.OemPeriod || key == Key.Decimal)
            {
                OnNumberButtonPressed(".");
            }
            else if (key == Key.OemMinus)
            {
                OnOperationButtonPressed("-");
            }
            else if (key == Key.OemPlus && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                OnOperationButtonPressed("+");
            }
            else if (key == Key.OemQuestion)
            {
                OnOperationButtonPressed("/");
            }
            else if (key == Key.OemPipe)
            {
                OnFunctionButtonPressed("sqrt");
            }
            else if (key == Key.Delete)
            {
                OnClearButtonPressed(null);  
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
