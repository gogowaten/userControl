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

namespace _20200612_WpfControlLibraryUpDown
{
    /// <summary>
    /// UserControl2.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        public UserControl2()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty =
          DependencyProperty.Register("Value", typeof(int), typeof(UserControl2), new PropertyMetadata(0));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value++;
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value--;
        }
    }
}
