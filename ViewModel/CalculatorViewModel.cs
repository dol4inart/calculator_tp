using calculator.Command;
using calculator.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace calculator.ViewModel
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private Memory _memory;
        private readonly CalculatorModel _calculatorModel;
        private string _currentInput;
        private bool _isMemoryHistoryVisible;
        private string _currentOperation;

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

        public string CurrentOperation // Свойство для текущей операции
        {
            get => _currentOperation;
            set
            {
                if (_currentOperation != value)
                {
                    _currentOperation = value;
                    OnPropertyChanged();
                }

            }
        }

        public bool IsMemoryHistoryVisible
        {
            get => _isMemoryHistoryVisible;
            set
            {
                if (_isMemoryHistoryVisible != value)
                {
                    _isMemoryHistoryVisible = value;
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
        public ICommand MemoryStoreCommand { get; }
        public ICommand MemoryAddCommand { get; }
        public ICommand MemorySubtractCommand { get; }
        public ICommand ToggleMemoryHistoryCommand { get; }

        public ObservableCollection<Memory> MemoryHistory { get; }




        public double MemoryValue { get; private set; }
        public MemoryHistoryViewModel MemoryHistoryViewModel { get; }


        public CalculatorViewModel()
        {

            _calculatorModel = new CalculatorModel();
            _currentInput = "0";

            MemoryClearCommand = new RelayCommand(OnMemoryClear);
            MemoryRecallCommand = new RelayCommand(OnMemoryRecall);
            MemoryStoreCommand = new RelayCommand(OnMemoryStore);
            MemoryAddCommand = new RelayCommand(OnMemoryAdd);
            MemorySubtractCommand = new RelayCommand(OnMemorySubtract);
            ToggleMemoryHistoryCommand = new RelayCommand(OnToggleMemoryHistory);

            MemoryHistory = new ObservableCollection<Memory>();
            _memory = new Memory(0, ""); // Инициализация памяти

            NumberCommand = new RelayCommand(OnNumberButtonPressed);
            OperationCommand = new RelayCommand(OnOperationButtonPressed);
            FunctionCommand = new RelayCommand(OnFunctionButtonPressed);
            EqualsCommand = new RelayCommand(OnEqualsButtonPressed);
            ClearCommand = new RelayCommand(OnClearButtonPressed);
            ClearEntryCommand = new RelayCommand(OnClearEntryButtonPressed);
            BackspaceCommand = new RelayCommand(OnBackspaceButtonPressed);
            InvertSignCommand = new RelayCommand(OnInvertSignButtonPressed);
        }


        private void OnMemoryClear(object parametr)
        {
            _memory.Value = 0;
            _memory.Operation = "";
        }

        private void OnMemoryStore(object parametr)
        {
            if (double.TryParse(CurrentInput, out double value)) // Проверяем, есть ли значение для сохранения
            {
                _memory.Value = value;
                _memory.Operation = CurrentOperation; // Сохраняем текущее состояние операции
                MemoryHistory.Add(new Memory(value, _memory.Operation)); // Сохраняем в историю
            }
        }

        private void OnMemoryRecall(object parametr)
        {
            CurrentInput = _memory.Value.ToString();
            // Здесь можно обновить состояние, если нужно
        }

        private void OnMemoryAdd(object parametr)
        {
            if (double.TryParse(CurrentInput, out double value))
            {
                _memory.Value += value;
                MemoryHistory.Add(new Memory(_memory.Value, "M+"));
            }
        }

        private void OnMemorySubtract(object parametr)
        {
            if (double.TryParse(CurrentInput, out double value))
            {
                _memory.Value -= value;
                MemoryHistory.Add(new Memory(_memory.Value, "M-"));
            }
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
        private void OnToggleMemoryHistory(object parametr)
        {
            IsMemoryHistoryVisible = !IsMemoryHistoryVisible; // Переключаем видимость истории памяти
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void ProcessKeyPress(Key key, TextBox inputTextBox)
        {
            throw new NotImplementedException();
        }
    }
}
