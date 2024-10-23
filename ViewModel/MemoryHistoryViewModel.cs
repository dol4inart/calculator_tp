using calculator.Model;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace calculator.ViewModel
{
    public class MemoryHistoryViewModel : ViewModelBase
    {
        public ObservableCollection<MemoryHistoryItem> MemoryHistory { get; set; }

        public MemoryHistoryViewModel()
        {
            MemoryHistory = new ObservableCollection<MemoryHistoryItem>();
        }

        public void AddMemoryItem(double value, string operation)
        {
            MemoryHistory.Add(new MemoryHistoryItem(value, operation));
        }
    }
}
