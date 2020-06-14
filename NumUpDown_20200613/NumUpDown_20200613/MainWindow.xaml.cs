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

            double d = 20.5;
            MyTextBlock.Text = d.ToString("000000.000");
            decimal dc = 20m;
            int i = (int)dc;
        }

        private void ButtonSmall_01_Click(object sender, RoutedEventArgs e)
        {
            My1.MySmallChange = 0.1m;
        }
    }
}
