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

//WPFにもNumericUpDownみたいなのをユーザーコントロールで、その1 - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2020/06/20/205352

//WPFにもNumericUpDownみたいなのをユーザーコントロールで、その7 - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2020/07/13/001236


namespace ControlLibraryCore20200620
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public NumericUpDown()
        {
            InitializeComponent();
        }

        #region 入力制限
        //スペースキーが押されたのを無効にする
        private void MyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        //入力の制限、数字とハイフンとピリオドだけ通す
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

        //フォーカス消失時、不自然な文字を削除と書式適用
        private void MyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            string text = tb.Text;

            //ピリオドが先頭か末尾にあった場合は削除
            if (text.StartsWith('.') || text.EndsWith('.'))
            {
                text = text.Replace(".", "");
            }


            //数値に変換できない文字列なら直前の数値に戻す            
            if (decimal.TryParse(text, out decimal m) == false)
            {
                var old = MyValue;
                tb.Text = old.ToString(MyStringFormat);
                MyValue = old;
                return;
            }
            //数値に変換できるなら、その値をセット
            else
            {
                tb.Text = m.ToString(MyStringFormat);
                MyValue = m;
            }



            //// -. も変なのでピリオドだけ削除
            //text = text.Replace("-.", "-");

            ////数値がないのにハイフンやピリオドがあった場合は削除
            //if (text == "-" || text == "." || text == "-0")
            //    text = "0";
            //tb.Text = text;
            //if (decimal.TryParse(text, out decimal m))
            //{
            //    MyValue = m;
            //}

            //tb.Text = MyValue.ToString(MyStringFormat);
            //MyValue = deci;


        }

        //
        private void MyTextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //貼り付け無効
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        //focusしたとき
        //MyValueからText生成して表示してテキストを全選択
        private void MyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            tb.Text = MyValue.ToString();
            tb.SelectAll();
        }

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
        #endregion 入力制限


        #region 依存関係プロパティ
        #region MyValue, MyText
        //要の値
        public decimal MyValue
        {
            get { return (decimal)GetValue(MyValueProperty); }
            set
            {
                //CoerceMyValueだけでは不十分だったのでここでも、
                //新しい値が上限下限を超えていたら書き換えてからSetする
                if (value > MyMaxValue)
                {
                    value = MyMaxValue;
                }
                if (value < MyMinValue)
                {
                    value = MyMinValue;
                }
                //Set
                SetValue(MyValueProperty, value);
            }
        }

        public static readonly DependencyProperty MyValueProperty =
            DependencyProperty.Register(nameof(MyValue), typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0m, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnMyValuePropertyChanged, CoerceMyValue));

        #region ValueChangedイベント
        public delegate void MyValueChangedEventHndler(object sender, MyValuechangedEventArgs e);
        protected virtual void OnMyValueChanged(MyValuechangedEventArgs e)
        {
            RaiseEvent(e);
        }
        public static readonly RoutedEvent MyValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MyValueChanged),
                RoutingStrategy.Bubble,
                typeof(MyValueChangedEventHndler),
                typeof(NumericUpDown));

        public event MyValueChangedEventHndler MyValueChanged
        {
            add { AddHandler(MyValueChangedEvent, value); }
            remove { RemoveHandler(MyValueChangedEvent, value); }
        }
        #endregion ValueChangedイベント

        //MyValueの変更直後の動作
        private static void OnMyValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ud = d as NumericUpDown;

            decimal newValue = (decimal)e.NewValue;
            decimal ovlValue = (decimal)e.OldValue;
            if (ud.MyTextBox.IsFocused)
            {
                ud.MyText = newValue.ToString();
            }
            else
            {
                var text = newValue.ToString(ud.MyStringFormat);
                ud.MyText = text;
                ud.MyValue = newValue;//重要！！！！！！！！！！！！！！！！！！！
            }
            //追加したValueChangedイベント用処理
            ud.OnMyValueChanged(new MyValuechangedEventArgs(MyValueChangedEvent, newValue, ovlValue));
        }

        //MyValueの変更直前の動作、値の検証、矛盾があれば値を書き換えて解消
        //入力された値が下限値より小さい場合は下限値に書き換え
        //入力された値が上限値より大きい場合は上限値に書き換え
        private static object CoerceMyValue(DependencyObject d, object basaValue)
        {
            var ud = d as NumericUpDown;
            var m = (decimal)basaValue;
            if (m < ud.MyMinValue) m = ud.MyMinValue;
            if (m > ud.MyMaxValue) m = ud.MyMaxValue;
            return m;
        }


        //        wpf - 標準依存プロパティ | wpf Tutorial
        //https://riptutorial.com/ja/wpf/example/9857/%E6%A8%99%E6%BA%96%E4%BE%9D%E5%AD%98%E3%83%97%E3%83%AD%E3%83%91%E3%83%86%E3%82%A3
        //より
        //バインドでMode=TwoWay （ TextBox.Textの動作に類似）を指定する必要性を排除するには、 
        //PropertyMetadata代わりにFrameworkPropertyMetadataを使用し、適切なフラグを指定するようにコードを更新します。

        //テキストボックとBindingする
        protected internal string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }

        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register(nameof(MyText), typeof(string), typeof(NumericUpDown),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnMyTextPropertyChanged));

        private static void OnMyTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ud = d as NumericUpDown;
            var s = (string)e.NewValue;
            if (s == "-0" || s == "-0.") return;
            if (decimal.TryParse(s, out decimal m))
            {
                ud.MyValue = m;
            }
        }

        #endregion MyValue, MyText

        #region StringFormat
        //リアルタイム更新で書式指定は無理がある？

        //書式指定用の文字列型依存関係プロパティ
        public string MyStringFormat
        {
            get { return (string)GetValue(MyStringFormatProperty); }
            set { SetValue(MyStringFormatProperty, value); }
        }

        public static readonly DependencyProperty MyStringFormatProperty =
            DependencyProperty.Register(nameof(MyStringFormat), typeof(string), typeof(NumericUpDown),
                new PropertyMetadata("", OnMyStrinfFormatChanged, CoerceMyStrinfFormatValue));

        private static void OnMyStrinfFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ud = d as NumericUpDown;
            var sf = (string)e.NewValue;
            decimal m = ud.MyValue;

            ud.MyText = ud.MyValue.ToString(sf);
            ud.MyValue = m;

        }

        //新しい書式の判定、不適切な場合は古い書式で上書きする
        private static object CoerceMyStrinfFormatValue(DependencyObject d, object baseValue)
        {
            var ud = d as NumericUpDown;
            var format = (string)baseValue;//新しい書式

            //正の値、負の値、0のときで書式を変えられる
            //セミコロンで区切る
            //(正の値書式 ; 負の値書式 ; 0の書式)

            //無限ループになる書式は数値に変換できる、かつ
            // ┣ 0以外
            // ┣ 「,.」が含まれている
            // ┣ 正の値の書式の先頭がハイフン
            // ┗ 負の値の書式の先頭がハイフンではない            
            //MyValueとMyTextのCallback間で無限ループになってStackOverflowになってしまう

            //正、負、0、それぞれを判定するため「;」で文字列を分割
            var neko = format.Split(";");
            for (int i = 0; i < neko.Length; i++)
            {
                string sf = neko[i];
                if (decimal.TryParse(sf, out decimal m))
                {
                    if (Math.Abs(m) != 0 || sf.Contains(",."))
                    {
                        return ud.MyStringFormat;
                    }
                    if (i == 0 && sf.StartsWith("-"))
                    {
                        return ud.MyStringFormat;
                    }
                    if (i == 1 && sf.StartsWith("-") == false)
                    {
                        return ud.MyStringFormat;
                    }
                }
                try
                {
                    //新しい書式適用してみてエラーならcatchに飛ぶ
                    var text = ud.MyValue.ToString(format);

                    //正の値が負の値に変化するような書式も弾く
                    decimal dc = 1m;
                    if (decimal.TryParse(dc.ToString(format), out decimal dd))
                    {
                        //「-.」や「-,.」などは「-」と同じ効果で、正の値が負の値に反転する
                        if (dd == -1m)
                        {
                            return ud.MyStringFormat;
                        }
                    }
                }
                catch (Exception)
                {
                    return ud.MyStringFormat;
                }
            }

            return format;
        }
        #endregion StringFormat

        #region Small, Large, Min, Max
        //小変更値
        public decimal MySmallChange
        {
            get { return (decimal)GetValue(MySmallChangeProperty); }
            set { SetValue(MySmallChangeProperty, value); }
        }
        public static readonly DependencyProperty MySmallChangeProperty =
            DependencyProperty.Register(nameof(MySmallChange), typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(1m));


        //大変更値
        public decimal MyLargeChange
        {
            get { return (decimal)GetValue(MyLargeChangeProperty); }
            set { SetValue(MyLargeChangeProperty, value); }
        }
        public static readonly DependencyProperty MyLargeChangeProperty =
            DependencyProperty.Register(nameof(MyLargeChange), typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(10m));



        //下限値
        public decimal MyMinValue
        {
            get { return (decimal)GetValue(MyMinValueProperty); }
            set { SetValue(MyMinValueProperty, value); }
        }
        public static readonly DependencyProperty MyMinValueProperty =
            DependencyProperty.Register(nameof(MyMinValue), typeof(decimal), typeof(NumericUpDown),
                new PropertyMetadata(decimal.MinValue, OnMyMinValuePropertyChanged, CoerceMyMinValue));

        //PropertyChangedコールバック、プロパティ値変更"直後"に実行される
        //変更された下限値と今の値での矛盾を解消
        //変更された新しい下限値と、今の値(MyValue)で矛盾が生じた(下限値 < 今の値)場合は、今の値を下限値に変更する
        private static void OnMyMinValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ud = (NumericUpDown)d;
            var min = (decimal)e.NewValue;//変更後の新しい下限値
            if (min > ud.MyValue) ud.MyValue = min;
        }

        //値の検証と変更
        //CoerceValueコールバック、プロパティ値変更"直前"に実行される
        //設定された値を強制(Coerce)的に変更できるので、矛盾があれば変更して解消する
        //入力された下限値と、今の上限値で矛盾が生じる(下限値 > 上限値)場合は、下限値を上限値に書き換える
        private static object CoerceMyMinValue(DependencyObject d, object baseValue)
        {
            var ud = (NumericUpDown)d;
            var min = (decimal)baseValue;//入力された下限値
            if (min > ud.MyMaxValue) min = ud.MyMaxValue;
            return min;
        }


        //上限値
        public decimal MyMaxValue
        {
            get { return (decimal)GetValue(MyMaxValueProperty); }
            set { SetValue(MyMaxValueProperty, value); }
        }
        public static readonly DependencyProperty MyMaxValueProperty =
            DependencyProperty.Register(nameof(MyMaxValue), typeof(decimal), typeof(NumericUpDown),
                new PropertyMetadata(decimal.MaxValue, OnMyMaxValuePropertyChanged, CoerceMyMaxValue));

        //上限値の変更直後の動作。上限値より今の値が大きい場合は、今の値を上限値に変更する
        private static void OnMyMaxValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ud = (NumericUpDown)d;
            var max = (decimal)e.NewValue;
            if (max < ud.MyValue) ud.MyValue = max;
        }

        //上限値変更直前の動作。入力された上限値が今の下限値より小さくなる場合は、上限値を下限値に書き換える
        private static object CoerceMyMaxValue(DependencyObject d, object baseValue)
        {
            var ud = (NumericUpDown)d;
            var max = (decimal)baseValue;
            if (max < ud.MyMinValue) max = ud.MyMinValue;
            return max;
        }
        #endregion Small, Large, Min, Max


        //ボタンの▲と▼の色指定、Colorsの色名or#xxxxxxを指定
        public Brush MyButtonMarkColor
        {
            get { return (Brush)GetValue(MyButtonMarkColorProperty); }
            set { SetValue(MyButtonMarkColorProperty, value); }
        }

        public static readonly DependencyProperty MyButtonMarkColorProperty =
            DependencyProperty.Register(nameof(MyButtonMarkColor), typeof(Brush), typeof(NumericUpDown),
                new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));


        //数字の表示位置、TextAlignmentを指定、既定値は右寄せ
        public TextAlignment MyTextAlignment
        {
            get { return (TextAlignment)GetValue(textAlignmentProperty); }
            set { SetValue(textAlignmentProperty, value); }
        }

        public static readonly DependencyProperty textAlignmentProperty =
            DependencyProperty.Register(nameof(MyTextAlignment), typeof(TextAlignment), typeof(NumericUpDown),
                new PropertyMetadata(TextAlignment.Right));




        #endregion 依存関係プロパティ



        //public double MyWidth
        //{
        //    get { return (double)GetValue(MyWidthProperty); }
        //    set { SetValue(MyWidthProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for MyWidth.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty MyWidthProperty =
        //    DependencyProperty.Register("MyWidth", typeof(double), typeof(NumericUpDown), new PropertyMetadata(16.0));

        //private static void OnMyWidthPropertyChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
        //{

        //}




        public override string ToString()
        {
            string s = "MyText = " + MyText + ", MyValue = " + MyValue.ToString();
            return s;
        }


        private void RepeatButtonUp_Click(object sender, RoutedEventArgs e)
        {
            MyValue += MySmallChange;
        }

        private void RepeatButtonDown_Click(object sender, RoutedEventArgs e)
        {
            MyValue -= MySmallChange;
        }

        private void RepeatButton_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0) MyValue -= MyLargeChange;
            else MyValue += MyLargeChange;
        }

        private void MyTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0) MyValue -= MySmallChange;
            else MyValue += MySmallChange;
        }


    }


    //値変更時のイベント用クラス
    public class MyValuechangedEventArgs : RoutedEventArgs
    {
        public MyValuechangedEventArgs(RoutedEvent id, decimal myNewValue, decimal myOldValue)
        {
            this.RoutedEvent = id;
            MyNewValue = myNewValue;
            MyOldValue = myOldValue;
        }
        public decimal MyNewValue { get; }
        public decimal MyOldValue { get; }
    }




    //RepeatButtonのWidthはUserControl全体のHeightから計算する用のコンバータ
    public class MyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value;//全体のHeight
            double width = d / 3.0;//1/2以下だと無限ループになる、1/2.1～1/3あたりがいい
            return width;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    ////未使用、これ使うと-0.とか0.が入力できない
    //public class MyValueRule : ValidationRule
    //{
    //    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    //    {
    //        if(decimal.TryParse((string)value,out decimal m))
    //        {
    //            //return new ValidationResult(true, null);
    //            return ValidationResult.ValidResult;
    //        }
    //        else
    //        {
    //            return new ValidationResult(false, "数値じゃない");
    //        }
    //    }
    //}
}