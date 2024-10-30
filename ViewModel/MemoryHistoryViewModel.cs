using calculator.Model;
using calculator.Command;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace calculator.ViewModel
{
    public class MemoryHistoryViewModel : ViewModelBase
    {
        public ICommand CloseCommand { get; }
        public ICommand ShowMemoryHistoryCommand { get; }

        private bool _isMemoryHistoryVisible;
        public bool IsMemoryHistoryVisible
        {
            get => _isMemoryHistoryVisible;
            set => Set(ref _isMemoryHistoryVisible, value);
        }

        public ObservableCollection<MemoryHistoryItem> MemoryHistory { get; set; }

        public MemoryHistoryViewModel()
        {
            MemoryHistory = new ObservableCollection<MemoryHistoryItem>();
            CloseCommand = new RelayCommand(OnCloseCommand);
            ShowMemoryHistoryCommand = new RelayCommand(OnShowMemoryHistoryCommand);
            IsMemoryHistoryVisible = false;

        }

        public void AddMemoryItem(double value, string operation)
        {
            MemoryHistory.Add(new MemoryHistoryItem(value, operation));
        }

        private void OnCloseCommand(object window)
        {
            if (window is System.Windows.Window w)
            {
                w.Close();
            }
        }

        private void OnShowMemoryHistoryCommand(object parametr)
        {
            System.Diagnostics.Debug.WriteLine("Memory history visibility toggled.");
            IsMemoryHistoryVisible = !IsMemoryHistoryVisible;
        }
    }
}
