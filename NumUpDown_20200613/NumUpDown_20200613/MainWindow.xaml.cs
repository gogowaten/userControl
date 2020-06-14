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

namespace NumUpDown_20200613
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

          
        }

        private void ButtonSmall_01_Click(object sender, RoutedEventArgs e)
        {
            My1.MySmallChange = 0.1m;
        }

        private void ButtonSmall_02_Click(object sender, RoutedEventArgs e)
        {
            My1.MySmallChange = 1m;
        }

        private void ButtonMax_Click(object sender, RoutedEventArgs e)
        {
            My1.MyMaximum = 10;
        }

        private void ButtonMin_Click(object sender, RoutedEventArgs e)
        {
            My1.MyMinimum = -10;
        }

        private void ButtonSet10000_Click(object sender, RoutedEventArgs e)
        {
            My1.MyValue = 10000;
        }

        private void ButtonMin10000_Click(object sender, RoutedEventArgs e)
        {
            My1.MyMinimum = 10000;
        }
    }
}
