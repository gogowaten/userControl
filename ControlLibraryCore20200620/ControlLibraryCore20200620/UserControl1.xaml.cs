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
}