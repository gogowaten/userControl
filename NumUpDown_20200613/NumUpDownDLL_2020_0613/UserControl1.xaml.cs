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
        //        WPF 依存プロパティの作り方 - Qiita
        //https://qiita.com/tricogimmick/items/62cd9f5deca365a83858

        public decimal MyValue
        {
            get
            {
                return (decimal)GetValue(MyValueProperty);
            }

            set
            {
                //ここで最小値最大値の判定
                if (value > MyMaximum) value = MyMaximum;
                if (value < MyMinimum) value = MyMinimum;
                SetValue(MyValueProperty, value);
            }
        }
        public static readonly DependencyProperty MyValueProperty =
            DependencyProperty.Register(nameof(MyValue), typeof(decimal), typeof(UserControl1),
                new PropertyMetadata(0m));

        

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


        #region 最小値と最大値
        //かなり難しかった
//        依存関係プロパティのコールバックと検証 - WPF | Microsoft Docs
//https://docs.microsoft.com/ja-jp/dotnet/framework/wpf/advanced/dependency-property-callbacks-and-validation

        //最小値が変更されたとき、今の値より大きい値が最小値として設定されたときは、今の値を最小値に変更する
        //最小値：-100、今の値：-20だったとして、新たに最小値が-10に変更されとき、そのままだと
        //最小値：-10、今の値：-20になるけど、このままだと不自然なので
        //最小値：-10、今の値：-10にする、この処理をしているメソッドが
        //OnMyMinimumChanged

        //もう一つが最大値より大きな値が最小値に設定されたときの処理
        //最大値 < 最小値だと矛盾しているので最大値と同じ値に変更する、同時に今の値も同じ値にする
        //この処理をしているのが
        //CoerceMyValue
        //これがよくわからん

        //この2つが処理される順番はCoerceMyValueが先で、その後にOnMyMinimumChangedが実行される
        public decimal MyMinimum
        {
            get => (decimal)GetValue(MyMinimumProperty);
            set => SetValue(MyMinimumProperty, value);
        }

        public static readonly DependencyProperty MyMinimumProperty =
            DependencyProperty.Register(
                nameof(MyMinimum), typeof(decimal), typeof(UserControl1),
                new PropertyMetadata(
                    0m,
                    new PropertyChangedCallback(OnMyMinimumChanged),
                    new CoerceValueCallback(CoerceMyValue)//強制検証
                ));

        //最小値の変更時に今の値を検証、設定
        private static void OnMyMinimumChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            UserControl1 uc = obj as UserControl1;
            if (uc != null)
            {
                var min = (decimal)e.NewValue;
                if (uc.MyValue < min)
                    uc.MyValue = min;
                //↓でも同じ？
                //if (uc.MyValue < uc.MyMinimum)
                //    uc.MyValue = uc.MyMinimum;
            }
        }
        //強制検証
        private static object CoerceMyValue(DependencyObject obj, object value)
        {
            //指定された最小値の検証
            //最大値より大きかったら、最大値と同じ値に変更する
            var uc = (UserControl1)obj;
            var min = (decimal)value;
            if (min > uc.MyMaximum) min = uc.MyMaximum;
            
            return min;
        }

        //最大値
        public decimal MyMaximum
        {
            get => (decimal)GetValue(MyMaximumProperty);
            set => SetValue(MyMaximumProperty, value);
        }

        public static readonly DependencyProperty MyMaximumProperty =
            DependencyProperty.Register(nameof(MyMaximum), typeof(decimal), typeof(UserControl1),
                new PropertyMetadata(100m,
                    new PropertyChangedCallback(OnMyMaximumChanged),
                    new CoerceValueCallback(CoerceMaxMyValue)));
        //最大値の変更時に今の値を検証、設定
        private static void OnMyMaximumChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            UserControl1 uc = obj as UserControl1;
            if (uc != null)
            {
                var max = (decimal)e.NewValue;
                if (uc.MyValue > max)
                    uc.MyValue = max;
                //if (uc.MyValue > uc.MyMaximum)
                //    uc.MyValue = uc.MyMaximum;
            }
        }
        private static object CoerceMaxMyValue(DependencyObject obj, object value)
        {

            var uc = (UserControl1)obj;
            var current = (decimal)value;
            if (current < uc.MyMinimum) current = uc.MyMinimum;
            return current;
        }

        #endregion 最小値と最大値



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
            int m = (int)values[2];//小数点以下の表示桁数

            //            文字列を指定回数繰り返した文字列を取得する - .NET Tips(VB.NET, C#...)
            //https://dobon.net/vb/dotnet/string/repeat.html
            //書式設定
            string str = new string('0', i) + "." + new string('0', m);

            return d.ToString(str);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}