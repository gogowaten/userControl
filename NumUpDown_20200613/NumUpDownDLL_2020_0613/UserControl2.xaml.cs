using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
    /// UserControl2.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        public UserControl2()
        {
            InitializeComponent();

           
            //MyValueと表示入力用のTextBoxとのBinding
            var mb = new MultiBinding();
            mb.Converter = new MyStringConverter();
            //mb.ConverterParameter = this;

            Binding b;
            //Value用のBinding
            b = new Binding();
            b.Source = this;
            b.Path = new PropertyPath(MyValueProperty);
            b.Mode = BindingMode.TwoWay;
            mb.Bindings.Add(b);

            //StringFormat用のBinding
            b = new Binding();
            b.Source = this;
            b.Path = new PropertyPath(MyStringFormatProperty);
            b.Mode = BindingMode.OneWay;//重要、TextBoxの値からはStringFormatを変換しないので渡さない
            //b.Mode = BindingMode.TwoWay;
            mb.Bindings.Add(b);


            MyTextBox.SetBinding(TextBox.TextProperty, mb);



        }

        //        wpf - 標準依存プロパティ | wpf Tutorial
        //https://riptutorial.com/ja/wpf/example/9857/%E6%A8%99%E6%BA%96%E4%BE%9D%E5%AD%98%E3%83%97%E3%83%AD%E3%83%91%E3%83%86%E3%82%A3
        //より
        //バインドでMode=TwoWay （ TextBox.Textの動作に類似）を指定する必要性を排除するには、 
        //PropertyMetadata代わりにFrameworkPropertyMetadataを使用し、適切なフラグを指定するようにコードを更新します。
        public decimal MyValue
        {
            get { return (decimal)GetValue(MyValueProperty); }
            set { SetValue(MyValueProperty, value); }
        }
        //public static readonly DependencyProperty MyValueProperty =
        //    DependencyProperty.Register(nameof(MyValue), typeof(decimal), typeof(UserControl2),
        //        new PropertyMetadata(0m));

        //↑のBinding.Modeの既定値をTwoWayにしたのが↓

        //public static readonly DependencyProperty MyValueProperty =
        //    DependencyProperty.Register(nameof(MyValue), typeof(decimal), typeof(UserControl2),
        //        new FrameworkPropertyMetadata(0m, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //minmaxの検証を追加した最終形態
        public static readonly DependencyProperty MyValueProperty =
            DependencyProperty.Register(nameof(MyValue), typeof(decimal), typeof(UserControl2),
                new FrameworkPropertyMetadata(0m, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnMyValueChanged, CoerceMyValue));

        private static void OnMyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //特にすることないけどCoerceValueCallbackを呼び出すのに必要？
        }

        private static object CoerceMyValue(DependencyObject d, object baseValue)
        {
            //最小値と最大値を越えないように値を矯正
            var value = (decimal)baseValue;
            var uc = (UserControl2)d;
            if (value < uc.MyMin)
                value = uc.MyMin;
            if (value > uc.MyMax)
                value = uc.MyMax;
            return value;
        }

        public decimal MySmallChange
        {
            get => (decimal)GetValue(MySmallChangeProperty);
            set => SetValue(MySmallChangeProperty, value);
        }
        public static readonly DependencyProperty MySmallChangeProperty =
            DependencyProperty.Register(nameof(MySmallChange), typeof(decimal), typeof(UserControl2), new PropertyMetadata(1m));


        public decimal MyLargeChange
        {
            get => (decimal)GetValue(MyLargeChangeProperty);
            set => SetValue(MyLargeChangeProperty, value);
        }
        public static readonly DependencyProperty MyLargeChangeProperty =
            DependencyProperty.Register(nameof(MyLargeChange), typeof(decimal), typeof(UserControl2), new PropertyMetadata(10m));

        #region stringformat
        public string MyStringFormat
        {
            get => (string)GetValue(MyStringFormatProperty);
            set => SetValue(MyStringFormatProperty, value);
        }
        public static readonly DependencyProperty MyStringFormatProperty =
            DependencyProperty.Register(nameof(MyStringFormat), typeof(string), typeof(UserControl2), new PropertyMetadata(""));

        public int MyKetaFront
        {
            get => (int)GetValue(MyKetaFrontProperty);
            set => SetValue(MyKetaFrontProperty, value);
        }
        public static readonly DependencyProperty MyKetaFrontProperty =
            DependencyProperty.Register(nameof(MyKetaFront), typeof(int), typeof(UserControl2), new PropertyMetadata(1, OnMyF1Changed));
        private static void OnMyF1Changed(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var uc = (UserControl2)obj;
            if (uc != null)
            {
                //新しい桁数が0以下 or 小数桁が0未満なら書式変更しない
                int keta = (int)e.NewValue;
                int rear = uc.MyKetaRear;
                if (keta < 0 || rear < 0)
                {
                    return;
                }

                //整数桁部分
                string front = new string('0', keta);


                if (rear == 0)
                {
                    //小数桁部分がなければそのまま指定
                    uc.MyStringFormat = front;
                }
                else
                {
                    //小数桁部分があるとき、小数点と桁表示の0を追加
                    uc.MyStringFormat = front + '.' + new string('0', rear);
                }
            }
        }

        public int MyKetaRear
        {
            get => (int)GetValue(MyKetaRearProperty);
            set => SetValue(MyKetaRearProperty, value);
        }
        public static readonly DependencyProperty MyKetaRearProperty =
            DependencyProperty.Register(nameof(MyKetaRear), typeof(int), typeof(UserControl2), new PropertyMetadata(0, OnMyF2Changed));
        private static void OnMyF2Changed(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var uc = (UserControl2)obj;
            if (uc != null)
            {
                int keta = (int)e.NewValue;
                int front = uc.MyKetaFront;
                //新しい桁数が0未満 or 整数桁数が0以下なら書式変更しない
                if (keta < 0 || front < 0) return;

                if (keta == 0)
                {
                    uc.MyStringFormat = new string('0', uc.MyKetaFront);
                }
                else
                {
                    uc.MyStringFormat = new string('0', uc.MyKetaFront) + '.' + new string('0', keta);
                }
            }
        }
        #endregion stringformat

        #region MinMax


        public decimal MyMin
        {
            get { return (decimal)GetValue(MyMinProperty); }
            set { SetValue(MyMinProperty, value); }
        }

        public static readonly DependencyProperty MyMinProperty =
            DependencyProperty.Register("MyMin", typeof(decimal), typeof(UserControl2),
                new PropertyMetadata(decimal.MinValue, OnMyMinChanged));
        private static void OnMyMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //矛盾の解消
            //今の値と設定された最小値に矛盾があれば、今の値を最小値と同じ値に変更
            var uc = (UserControl2)d;
            var value = (decimal)e.NewValue;
            if (uc.MyValue < value)
                uc.MyValue = value;
        }


        public decimal MyMax
        {
            get { return (decimal)GetValue(MyMaxProperty); }
            set { SetValue(MyMaxProperty, value); }
        }

        public static readonly DependencyProperty MyMaxProperty =
            DependencyProperty.Register("MyMax", typeof(decimal), typeof(UserControl2),
                new PropertyMetadata(decimal.MaxValue, OnMyMaxChanged));
        private static void OnMyMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //矛盾の解消
            //今の値と設定された最大値に矛盾があれば、今の値を最大値と同じ値に変更
            var uc = (UserControl2)d;
            var value = (decimal)e.NewValue;
            if (uc.MyValue > value)
                uc.MyValue = value;
        }



        #endregion MinMax

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
                MyValue -= MyLargeChange;
            }
            else
            {
                MyValue += MyLargeChange;
            }
        }

        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
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


        #endregion クリックとかのイベント処理

      
    }


    //public class MyFormatConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        int i1 = (int)values[0];
    //        int i2 = (int)values[1];
    //        string format = new string('0', i1) + '.' + new string('0', i2);
    //        return format;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        string str = (string)value;
    //        int i = str.IndexOf('.');
    //        int l = str.Length;

    //        int front, rear;
    //        if (i == -1)
    //        {
    //            front = 0;
    //            rear = 0;
    //        }
    //        else
    //        {
    //            front = i - 1;
    //            rear = l - i;
    //        }
    //        return new object[] { front, rear };

    //        //throw new NotImplementedException();
    //    }
    //}

    public class MyStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            decimal d = (decimal)values[0];
            string s = (string)values[1];
            return d.ToString(s);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            //UserControl2 uc2 = (UserControl2)parameter;
            string f = (string)value;
            decimal d = decimal.Parse(f);

            return new object[] { d };
            //string s = uc2.MyStringFormat;
            //return new object[] { d, s };
        }
    }
}
