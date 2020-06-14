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
        }

        public decimal Value
        {
            get => (decimal)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(UserControl1), new PropertyMetadata(0m));

        public decimal SmallChange
        {
            get => (decimal)GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(decimal), typeof(UserControl1), new PropertyMetadata(1m));


        public decimal LargeChange
        {
            get => (decimal)GetValue(LargeChangeProperty);
            set => SetValue(LargeChangeProperty, value);
        }
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register(nameof(LargeChange), typeof(decimal), typeof(UserControl1), new PropertyMetadata(10m));







        private void RepeatButtonUp_Click(object sender, RoutedEventArgs e)
        {
            this.Value += SmallChange;
        }

        private void RepeatButtonDown_Click(object sender, RoutedEventArgs e)
        {
            this.Value -= SmallChange;
        }

        private void RepeatButton_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                Value -= LargeChange;
            }
            else
            {
                Value += LargeChange;
            }
        }

        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0) Value -= SmallChange;
            else Value += SmallChange;
        }
    }
}