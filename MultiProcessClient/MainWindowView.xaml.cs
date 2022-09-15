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

namespace MultiProcessClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindowView : Window
    {
        MainViewModel _mainViewModel;
        public MainWindowView()
        {
            InitializeComponent();
            this.Closed += MainWindowView_Closed;
            _mainViewModel = new MainViewModel();
            this.DataContext = _mainViewModel;
        }

        private void MainWindowView_Closed(object sender, EventArgs e)
        {
            _mainViewModel.CloseAllTabs();
        }
    }
}
