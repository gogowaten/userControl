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

        //フォーカス消失時、不自然な文字を削除
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

        //
        private void MyTextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //貼り付け無効
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        //focusしたときにテキストを全選択
        private void MyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
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

        //要の値
        public decimal MyValue
        {
            get { return (decimal)GetValue(MyValueProperty); }
            set { SetValue(MyValueProperty, value); }
        }

        public static readonly DependencyProperty MyValueProperty =
            DependencyProperty.Register(nameof(MyValue), typeof(decimal), typeof(NumericUpDown),
                new PropertyMetadata(0m, OnMyValuePropertyChanged, CoerceMyValue));

        //MyValueの変更直後の動作
        private static void OnMyValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //することない
        }

        //MyValueの変更直前の動作、値の検証、矛盾があれば値を書き換えて解消
        //入力された値が下限値より小さい場合は下限値に書き換え
        //入力された値が上限値より大きい場合は上限値に書き換え
        private static object CoerceMyValue(DependencyObject d, object basaValue)
        {
            var ud = (NumericUpDown)d;
            var m = (decimal)basaValue;
            if (m < ud.MyMinValue) m = ud.MyMinValue;
            if (m > ud.MyMaxValue) m = ud.MyMaxValue;
            return m;
        }



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



        #endregion 依存関係プロパティ



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
}