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

namespace UserControlDll動作確認用
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Data MyData;
        public MainWindow()
        {
            InitializeComponent();

            //string s = "-,0";
            //decimal m = 123m;
            //var neko = m.ToString(s);
            //////var inu = -m == decimal.Parse(neko);
            //s = ",.";
            ////neko = m.ToString(s);
            ////s = "j";
            ////neko = m.ToString(s);
            //neko = 123m.ToString(".0");
            //neko = 123m.ToString(",.0");
            //neko = 123m.ToString("0,.");
            //neko = 123m.ToString("0.");
            //neko = 123m.ToString("0,");
            //neko = 123m.ToString(";");
            MyData = new Data();
            MyData.MyData = 50m;
            this.DataContext = MyData;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            nume.MyMinValue = -10;
            nume.MyMaxValue = 20;
            nume.MyValue = 5;
        }

        private void nume_MyValueChanged(object sender, ControlLibraryCore20200620.MyValuechangedEventArgs e)
        {
            if (MyLabel == null) return;
            MyLabel.Content = $"古い値は：{e.MyOldValue}、新しい値は：{e.MyNewValue}";
        }

        private void MyButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var neko = nume.MyValue;
            var inu = MyData.MyData;
        }
    }

    public class Data
    {
        public decimal MyData { get; set; } = 50m;
    }
}
