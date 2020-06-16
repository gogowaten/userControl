﻿using System;
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
using System.Text.RegularExpressions;


namespace _20200616_textbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string prefix = "個数：";
        public MainWindow()
        {
            InitializeComponent();

            MyTextBox.Text = prefix + MyTextBox.Text;
            MyTextBox.GotFocus += MyTextBox_GotFocus;
            MyTextBox.LostFocus += MyTextBox_LostFocus;
            MyTextBox.PreviewKeyDown += MyTextBox_PreviewKeyDown;
        }

        private void MyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //スペースキーが押されたときは、それを無効にする
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void MyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string str = MyTextBox.Text;
            str = str.Substring(3);
            MyTextBox.Text = str;
            //MyTextBox.SelectAll();
        }

        private void MyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            
            MyTextBox.Text = prefix + MyTextBox.Text;
        }


//        Visual Studio / WPF > link > TextBoxに数値しか入力できなくする > PreviewTextInput使用 | e.Handled = true; - Qiita
//https://qiita.com/7of9/items/04793406f94d229a6c4d
//            TextBoxに数値のみを入力する[C# WPF]
//https://vdlz.xyz/Csharp/WPF/Control/EditableControl/TextBox/TextBoxNumberOnly.html
//            [Tips][TextBox] キャレットの位置を取得する | HIROs.NET Blog
//https://blog.hiros-dot.net/?p=1594

        private void MyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //ピリオドとマイナス記号は1個
            //マイナス記号の位置は先頭
            //ピリオドの位置は先頭と末尾以外
            var tbox = (TextBox)sender;
            string str = tbox.Text;//文字列
            var txt = e.Text;//入力された文字

            bool neko;
            //入力文字が数値とピリオド、マイナス記号以外だったら無効にして、終了
            neko = new Regex("[0-9.-]").IsMatch(txt);
            if (neko == false)
            {
                e.Handled = true;//無効
                return;//終了
            }

            //マイナス記号が先頭以外に入力なら無効にして終了
            if (txt == "-" && tbox.CaretIndex != 0) { e.Handled = true; return; }

            //マイナス記号があるのに、さらに入力なら無効にして終了
            neko = str.Contains("-");
            if (neko == true && txt == "-") { e.Handled = true; return; }

            //2つ目のピリオド入力は無効にして終了
            neko = str.Contains(".");
            if (neko && txt == ".") { e.Handled = true; return; }

            //ピリオドの位置


            //var reg = new Regex("[0-9.-]");
            //var match = reg.IsMatch(e.Text);

            //e.Handled = !new Regex("[0-9.-]").IsMatch(e.Text);
            //e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }
    }
}