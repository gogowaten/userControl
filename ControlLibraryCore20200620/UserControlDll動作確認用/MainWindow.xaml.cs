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

namespace UserControlDll動作確認用
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            nume.MyMinValue = -10;
            nume.MyMaxValue = 20;
            nume.MyValue = 5;
        }


        private void ButtonMin_Click(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            nume.MyMinValue = decimal.Parse(b.Tag.ToString());
        }

        private void ButtonMax_Click(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            nume.MyMaxValue = decimal.Parse(b.Tag.ToString());
        }
    }
}
