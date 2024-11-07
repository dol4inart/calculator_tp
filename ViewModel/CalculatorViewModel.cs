using calculator.Command;
using calculator.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Controls;


namespace calculator.ViewModel
{ 


    public class CalculatorViewModel : Notifier
    {
        private MemoryHistoryItem _memory;
        private readonly CalculatorModel _calculatorModel;
        private string _currentInput;
        private bool _isMemoryHistoryVisible;
        private bool _isNotDividingByZero;
        private bool _isMemoryNotEmpty;
        private bool _actionButtonPressed;
        private int _equalCount;
        

        // Свойства
        public string CurrentInput
        {
            get => _currentInput;
            set => SetProperty(ref _currentInput, value);
        }

        public bool IsMemoryHistoryVisible
        {
            get => _isMemoryHistoryVisible;
            set => SetProperty(ref _isMemoryHistoryVisible, value);
        }

        public bool IsNotDividingByZero
        {
            get => _isNotDividingByZero;
            set => SetProperty(ref _isNotDividingByZero, value);
        }

        public bool ActionButtonPressed
        {
            get => _actionButtonPressed;
            set => SetProperty(ref _actionButtonPressed, value);
        }

        public bool IsMemoryNotEmpty
        {
            get => _isMemoryNotEmpty;
            set => SetProperty(ref _isMemoryNotEmpty, value);
        }

        public int EqualCount
        {
            get => _equalCount;
            set => SetProperty(ref _equalCount, value);
        }
        public ObservableCollection<MemoryHistoryItem> MemoryHistory { get; }

        // Команды
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

        // Конструктор
        public CalculatorViewModel()
        {
            _calculatorModel = new CalculatorModel();
            CurrentInput = "0";
            EqualCount = 0;     
            IsNotDividingByZero = true; // Нет деления на ноль изначально

            

            MemoryHistory = new ObservableCollection<MemoryHistoryItem>();
            _memory = new MemoryHistoryItem(0, "");

            // Команды
            MemoryClearCommand = new RelayCommand(OnMemoryClear);
            MemoryRecallCommand = new RelayCommand(OnMemoryRecall);
            MemoryStoreCommand = new RelayCommand(OnMemoryStore);
            MemoryAddCommand = new RelayCommand(OnMemoryAdd);
            MemorySubtractCommand = new RelayCommand(OnMemorySubtract);
            CloseMemoryHistoryCommand = new RelayCommand(OnCloseMemoryHistory);
            ToggleMemoryHistoryCommand = new RelayCommand(OnToggleMemoryHistory);
            NumberCommand = new RelayCommand(OnNumberButtonPressed);
            OperationCommand = new RelayCommand(OnOperationButtonPressed);
            FunctionCommand = new RelayCommand(OnFunctionButtonPressed);
            EqualsCommand = new RelayCommand(OnEqualsButtonPressed);
            ClearCommand = new RelayCommand(OnClearButtonPressed);
            ClearEntryCommand = new RelayCommand(OnClearEntryButtonPressed);
            BackspaceCommand = new RelayCommand(OnBackspaceButtonPressed);
            InvertSignCommand = new RelayCommand(OnInvertSignButtonPressed);
        }


        // Функции памяти
        private void OnMemoryClear(object parameter)
        {
            MemoryHistory.Clear();
            IsMemoryNotEmpty = false;
            CurrentInput = "0";
        }

        private void OnMemoryStore(object parameter)
        {
            if (double.TryParse(CurrentInput, out double value))
            {
                _memory.Value = value;
                MemoryHistory.Add(new MemoryHistoryItem(value, "Stored"));
                IsMemoryNotEmpty = true;
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
                IsMemoryNotEmpty = true;
            }
        }

        private void OnMemorySubtract(object parameter)
        {
            if (double.TryParse(CurrentInput, out double value))
            {
                _memory.Value -= value;
                MemoryHistory.Add(new MemoryHistoryItem(_memory.Value, "M-"));
                IsMemoryNotEmpty = true;
            }
        }

        // Обработка кнопок с цифрами
        public void OnNumberButtonPressed(object parameter)
        {

            var number = parameter.ToString();

            if (CurrentInput == "0")
                CurrentInput = number;
            else if (ActionButtonPressed)
            {
                CurrentInput = number;
                ActionButtonPressed = false;
            }
            else if (!IsNotDividingByZero)
            {
                IsNotDividingByZero = true;
                CurrentInput = number;
            }
            else
            {
                CurrentInput += number;
            }
        }

        // Обработка операций
        public void OnOperationButtonPressed(object parameter)
        {
            if (IsNotDividingByZero == true)
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
                ActionButtonPressed = true;
            }
        }



        // Обработка функций (например, %)
        public void OnFunctionButtonPressed(object parameter)
        {
            if (IsNotDividingByZero == true)
            {
                var function = parameter.ToString();
                if (function == "1/x" && double.Parse(CurrentInput) == 0)
                {
                    ThrowExceptonDivideByZero();
                }

                else if (_calculatorModel.IsOperationPending && function != "%")
                {

                    _calculatorModel.CurrentValue = double.Parse(CurrentInput);
                    _calculatorModel.ExecuteOperation();
                    CurrentInput = _calculatorModel.CurrentValue.ToString();
                    _calculatorModel.IsOperationPending = false;
                }
                else
                {
                    _calculatorModel.CurrentValue = double.Parse(CurrentInput);
                    _calculatorModel.ExecuteFunction(function);
                    CurrentInput = _calculatorModel.CurrentValue.ToString();
                }
            }
        }

        // Обработка кнопки "="
        public void OnEqualsButtonPressed(object parameter)
        {
            if (_calculatorModel.Operation == "/" && double.Parse(CurrentInput) == 0)
            {
                ThrowExceptonDivideByZero();
            }
            else
            {
                if (EqualCount < 1)
                {
                    _calculatorModel.CurrentValue = double.Parse(CurrentInput);
                    _calculatorModel.ExecuteOperation();
                    _calculatorModel.PreviousValue = double.Parse(CurrentInput);
                    CurrentInput = _calculatorModel.CurrentValue.ToString();
                    _calculatorModel.IsOperationPending = false;
                }
                else
                {
                    _calculatorModel.CurrentValue = double.Parse(CurrentInput);
                    _calculatorModel.ExecuteOperation();
                    _calculatorModel.PreviousValue = _calculatorModel.PreviousValue;
                    CurrentInput = _calculatorModel.CurrentValue.ToString();
                    _calculatorModel.IsOperationPending = false;
                }


            }
            EqualCount++;
        }

        // Очистка
        private void OnClearButtonPressed(object parameter)
        {


            _calculatorModel.Clear();
            CurrentInput = "0";
            IsNotDividingByZero = true;
            EqualCount = 0;
        }

        private void OnClearEntryButtonPressed(object parameter)
        {

            CurrentInput = "0";
            IsNotDividingByZero = true;
            EqualCount = 0;
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


        // Процесс нажатия клавиши
        public void ProcessKeyPress(Key key)
        {


            if (key >= Key.D0 && key <= Key.D9)
            {
                OnNumberButtonPressed((key - Key.D0).ToString());
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
            else if (key == Key.Delete)
            {
                OnClearButtonPressed(null);
            }
        }

        // Закрыть историю памяти
        private void OnCloseMemoryHistory(object parameter)
        {
            IsMemoryHistoryVisible = false;
        }

        // Показать/скрыть историю памяти
        private void OnToggleMemoryHistory(object parameter)
        {
            IsMemoryHistoryVisible = !IsMemoryHistoryVisible;
        }

        private void ThrowExceptonDivideByZero()
        {
            CurrentInput = "Деление на ноль невозможно";
            _calculatorModel.IsOperationPending = false;
            IsNotDividingByZero = false;
        }
    }
}
