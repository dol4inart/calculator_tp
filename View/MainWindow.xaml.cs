using calculator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace calculator.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new CalculatorViewModel();
        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is CalculatorViewModel viewModel)
            {
                // Игнорируем нажатие Enter, если фокус не на TextBox
                if (e.Key == Key.Enter && !IsFocusedOnTextBox())
                {
                    e.Handled = true; // Предотвращаем нажатие выделенной кнопки
                    viewModel.ProcessKeyPress(e.Key);
                }
                else
                {
                    viewModel.ProcessKeyPress(e.Key);
                }
            }
        }

        private bool IsFocusedOnTextBox()
        {
            // Проверяем, находится ли фокус на TextBox
            return Keyboard.FocusedElement is System.Windows.Controls.TextBox;
        }


    }
}
