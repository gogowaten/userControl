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

namespace ControlLibraryCore20200620
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        //comment
        private void MyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void MyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void MyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void MyTextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}