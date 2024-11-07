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
using System.Configuration;

namespace calculator.ViewModel
{
    public abstract class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }


    public class CalculatorViewModel : Notifier
    {
        private MemoryHistoryItem _memory;
        private readonly CalculatorModel _calculatorModel;
        private string _currentInput;
        private bool _isMemoryHistoryVisible;
        private string _currentOperation;

        public string CurrentInput
        {
            get => _currentInput;
            set => SetProperty(ref _currentInput, value);
        }

        public string CurrentOperation // Свойство для текущей операции
        {
            get => _currentOperation;
            set => SetProperty(ref _currentInput, value);

        }

        public bool IsMemoryHistoryVisible
        {
            get => _isMemoryHistoryVisible;
            set => SetProperty(ref _isMemoryHistoryVisible, value);
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
        public ICommand CloseMemoryHistoryCommand { get; }

        public ObservableCollection<MemoryHistoryItem> MemoryHistory { get; }

        public MemoryHistoryViewModel MemoryHistoryViewModel { get; }


        public double MemoryValue { get; private set; }
        

        public bool IsMemoryNotEmpty => MemoryHistory.Count > 0;


        public CalculatorViewModel()
        {

            _calculatorModel = new CalculatorModel();
            _currentInput = "0";

            MemoryClearCommand = new RelayCommand(OnMemoryClear);
            MemoryRecallCommand = new RelayCommand(OnMemoryRecall);
            MemoryStoreCommand = new RelayCommand(OnMemoryStore);
            MemoryAddCommand = new RelayCommand(OnMemoryAdd);
            MemorySubtractCommand = new RelayCommand(OnMemorySubtract);
            CloseMemoryHistoryCommand = new RelayCommand(OnCloseMemoryHistory);
            ToggleMemoryHistoryCommand = new RelayCommand(OnToggleMemoryHistory);

            MemoryHistory = new ObservableCollection<MemoryHistoryItem>();
            _memory = new MemoryHistoryItem(0, "");
            _currentOperation = "";
            //IsMemoryHistoryVisible = false;

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
            MemoryHistory.Clear();
            CurrentOperation = "";
        }

        private void OnMemoryStore(object parameter)
        {
            if (double.TryParse(CurrentInput, out double value)) // Проверяем, есть ли значение для сохранения
            {
                _memory.Value = value;
                _memory.Operation = CurrentOperation; // Сохраняем текущее состояние операции
                MemoryHistory.Add(new MemoryHistoryItem(value, _memory.Operation)); // Сохраняем в историю
            }
        }

        private void OnMemoryRecall(object parameter)
        {
            if (MemoryHistory.Count > 0)
            {
                var lastMemory = MemoryHistory[MemoryHistory.Count - 1];
                CurrentInput = lastMemory.Value.ToString();
            }
        }

        private void OnMemoryAdd(object parameter)
        {
            if (double.TryParse(CurrentInput, out double value))
            {
                _memory.Value += value;
                MemoryHistory.Add(new MemoryHistoryItem(_memory.Value, "M+"));
            }
        }

        private void OnMemorySubtract(object parameter)
        {
            if (double.TryParse(CurrentInput, out double value))
            {
                _memory.Value -= value;
                MemoryHistory.Add(new MemoryHistoryItem(_memory.Value, "M-"));
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
        private void OnCloseMemoryHistory(object parameter)
        {
            IsMemoryHistoryVisible = false;
        }

        private void OnToggleMemoryHistory(object parameter)
        {
            IsMemoryHistoryVisible = !IsMemoryHistoryVisible;
        }


        internal void ProcessKeyPress(Key key, TextBox inputTextBox)
        {
            throw new NotImplementedException();
        }
    }
}
