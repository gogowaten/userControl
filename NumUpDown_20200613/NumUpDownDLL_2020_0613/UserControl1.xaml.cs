using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace NumUpDownDLL_2020_0613
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();

            this.Loaded += UserControl1_Loaded;
        }

        private void UserControl1_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #region 依存プロパティ
        public decimal MyValue
        {
            get => (decimal)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(MyValue), typeof(decimal), typeof(UserControl1), new PropertyMetadata(0m));


        public decimal MySmallChange
        {
            get => (decimal)GetValue(MySmallChangeProperty);
            set => SetValue(MySmallChangeProperty, value);
        }
        public static readonly DependencyProperty MySmallChangeProperty =
            DependencyProperty.Register(nameof(MySmallChange), typeof(decimal), typeof(UserControl1), new PropertyMetadata(1m));


        public decimal MyLargeChange
        {
            get => (decimal)GetValue(MyLargeChangeProperty);
            set => SetValue(MyLargeChangeProperty, value);
        }
        public static readonly DependencyProperty MyLargeChangeProperty =
            DependencyProperty.Register(nameof(MyLargeChange), typeof(decimal), typeof(UserControl1), new PropertyMetadata(10m));


        //表示桁数指定
        //整数
        public int MyKetaInt
        {
            get => (int)GetValue(MyKetaIntProperty);
            set
            {
                if (value < 1)
                    value = 1;
                SetValue(MyKetaIntProperty, value);
            }
        }
        public static readonly DependencyProperty MyKetaIntProperty =
                DependencyProperty.Register(nameof(MyKetaInt), typeof(int), typeof(UserControl1), new PropertyMetadata(1));
        
        //小数点以下
        public int MyKetaDecimal
        {
            get => (int)GetValue(MyKetaDecimalProperty);
            set
            {
                if (value < 0) value = 0;
                SetValue(MyKetaDecimalProperty, value);
            }
        }
        public static readonly DependencyProperty MyKetaDecimalProperty =
                DependencyProperty.Register(nameof(MyKetaDecimal), typeof(int), typeof(UserControl1), new PropertyMetadata(0));




        #endregion 依存プロパティ


        #region クリックとかのイベント処理

        private void RepeatButtonUp_Click(object sender, RoutedEventArgs e)
        {
            this.MyValue += MySmallChange;
        }

        private void RepeatButtonDown_Click(object sender, RoutedEventArgs e)
        {
            this.MyValue -= MySmallChange;
        }

        private void RepeatButton_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                MyValue -= MySmallChange;
            }
            else
            {
                MyValue += MySmallChange;
            }
        }

        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                MyValue -= MyLargeChange;
            }
            else
            {
                MyValue += MyLargeChange;
            }
        }
        #endregion クリックとかのイベント処理


    }



    //数値を文字列に変換するときに、指定桁数で変換する用
    public class MyConverterMulti : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            decimal d = (decimal)values[0];//数値
            int i = (int)values[1];//整数の表示桁数
            int f = (int)values[2];//小数点以下の表示桁数

            //            文字列を指定回数繰り返した文字列を取得する - .NET Tips(VB.NET, C#...)
            //https://dobon.net/vb/dotnet/string/repeat.html
            //書式設定
            string str = new string('0', i) + "." + new string('0', f);

            return d.ToString(str);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}