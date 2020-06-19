using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

//WPFで数字とハイフンとピリオドだけ入力できるテキストボックス、-0.0に意味はある？ - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2020/06/19/152006


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

        //整数桁数
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
                int keta = (int)e.NewValue;
                int rear = uc.MyKetaRear;//小数桁数
                if (rear < 0) rear = 0;
                if (keta < 0) keta = 0;

                //整数桁、小数桁ともに0以下なら書式設定なしにする
                if (rear == 0 && keta == 0)
                {
                    uc.MyStringFormat = "";
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

        //小数桁数
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
                //0未満なら0に変更して処理していく
                if (front < 0)
                    front = 0;
                if (keta < 0)
                    keta = 0;

                //整数桁、小数桁ともに0以下なら書式設定なしにする
                if (front == 0 && keta == 0)
                {
                    uc.MyStringFormat = "";
                    return;
                }

                if (keta == 0)
                {
                    uc.MyStringFormat = new string('0', front);
                }
                else
                {
                    uc.MyStringFormat = new string('0', front) + '.' + new string('0', keta);
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



        #region クリックやキー入力での数値加減

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

        //方向キーでの数値加減
        private void MyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //スペースキーが押されたときは無効にする
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                return;
            }

            //ctrl+up、ctrl+downでLargeChange            
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.Up)
                {
                    this.MyValue += MyLargeChange;
                    //e.Handled = true;
                    //MyTextBox.SelectAll();
                }
                else if (e.Key == Key.Down)
                {
                    this.MyValue -= MyLargeChange;
                    //e.Handled = true;
                }
            }
            //up、downでSmallChange
            else if (e.Key == Key.Up)
                this.MyValue += MySmallChange;
            else if (e.Key == Key.Down)
                this.MyValue -= MySmallChange;
        }


        #endregion クリックやキー入力での数値加減


        #region テキストボックスの入力制限
        private void MyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textbox = (TextBox)sender;
            string str = textbox.Text;//文字列
            var inputStr = e.Text;//入力された文字

            //正規表現で入力文字の判定、数字とピリオド、ハイフンならtrue
            bool neko = new System.Text.RegularExpressions.Regex("[0-9.-]").IsMatch(inputStr);

            //入力文字が数値とピリオド、ハイフン以外だったら無効
            if (neko == false)
            {
                e.Handled = true;//無効
                return;//終了
            }

            //キャレット(カーソル)位置が先頭(0)じゃないときの、ハイフン入力は無効
            if (textbox.CaretIndex != 0 && inputStr == "-") { e.Handled = true; return; }

            //2つ目のハイフン入力は無効(全選択時なら許可)
            if (textbox.SelectedText != str)
            {
                if (str.Contains("-") && inputStr == "-") { e.Handled = true; return; }
            }

            //2つ目のピリオド入力は無効
            if (str.Contains(".") && inputStr == ".") { e.Handled = true; return; }
        }

        private void MyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //ピリオドの削除
            //先頭か末尾にあった場合は削除
            var tb = (TextBox)sender;
            string text = tb.Text;
            if (text.StartsWith('.') || text.EndsWith('.'))
            {
                text = text.Replace(".", "");
            }

            // -. も変なのでピリオドだけ削除
            text = text.Replace("-.", "-");

            //数値がないのにハイフンやピリオドがあった場合は削除
            if (text == "-" || text == ".")
                text = "";

            tb.Text = text;
        }

        private void MyTextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //貼り付け無効
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        #endregion テキストボックスの入力制限



        //        | オールトの雲
        //http://ooltcloud.sakura.ne.jp/blog/201311/article_30013700.html
        //クリックしたときにテキストを全選択
        private void MyTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb.IsFocused == false)
            {
                tb.Focus();
                e.Handled = true;
            }
        }

        //focusしたときにテキストを全選択
        private void MyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            tb.SelectAll();
        }


    }

    //テキストボックスの文字列と数値の変換用
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

            //数字とハイフンとピリオドだけ抜き出す
            System.Text.RegularExpressions.MatchCollection ss =
                new System.Text.RegularExpressions.Regex("[0-9.-]").Matches(f);
            string str = "";
            for (int i = 0; i < ss.Count; i++)
            {
                str += ss[i];
            }

            if (str == "")
                str = "0";
            decimal d = decimal.Parse(str);

            return new object[] { d };
            //string s = uc2.MyStringFormat;
            //return new object[] { d, s };
        }
    }


    //数値にならない値がテキストボックスにあるときに枠を赤くしたいけど
    //なぜか動かない
    //マルチBindingの場合は、また方法が違うみたい
    public class MyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string text = (string)value;
            if(decimal.TryParse(text,out decimal m))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, null);
            }
        }
    }
}
