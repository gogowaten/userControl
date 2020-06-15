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

        #region �ˑ��v���p�e�B
        //        WPF �ˑ��v���p�e�B�̍��� - Qiita
        //https://qiita.com/tricogimmick/items/62cd9f5deca365a83858

        public decimal MyValue
        {
            get
            {
                return (decimal)GetValue(MyValueProperty);
            }

            set
            {
                //�����ōŏ��l�ő�l�̔���
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


        //�\�������w��
        //����
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

        //�����_�ȉ�
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


        #region �ŏ��l�ƍő�l
        //���Ȃ�������
//        �ˑ��֌W�v���p�e�B�̃R�[���o�b�N�ƌ��� - WPF | Microsoft Docs
//https://docs.microsoft.com/ja-jp/dotnet/framework/wpf/advanced/dependency-property-callbacks-and-validation

        //�ŏ��l���ύX���ꂽ�Ƃ��A���̒l���傫���l���ŏ��l�Ƃ��Đݒ肳�ꂽ�Ƃ��́A���̒l���ŏ��l�ɕύX����
        //�ŏ��l�F-100�A���̒l�F-20�������Ƃ��āA�V���ɍŏ��l��-10�ɕύX����Ƃ��A���̂܂܂���
        //�ŏ��l�F-10�A���̒l�F-20�ɂȂ邯�ǁA���̂܂܂��ƕs���R�Ȃ̂�
        //�ŏ��l�F-10�A���̒l�F-10�ɂ���A���̏��������Ă��郁�\�b�h��
        //OnMyMinimumChanged

        //��������ő�l���傫�Ȓl���ŏ��l�ɐݒ肳�ꂽ�Ƃ��̏���
        //�ő�l < �ŏ��l���Ɩ������Ă���̂ōő�l�Ɠ����l�ɕύX����A�����ɍ��̒l�������l�ɂ���
        //���̏��������Ă���̂�
        //CoerceMyValue
        //���ꂪ�悭�킩���

        //����2����������鏇�Ԃ�CoerceMyValue����ŁA���̌��OnMyMinimumChanged�����s�����
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
                    new CoerceValueCallback(CoerceMyValue)//��������
                ));

        //�ŏ��l�̕ύX���ɍ��̒l�����؁A�ݒ�
        private static void OnMyMinimumChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            UserControl1 uc = obj as UserControl1;
            if (uc != null)
            {
                var min = (decimal)e.NewValue;
                if (uc.MyValue < min)
                    uc.MyValue = min;
                //���ł������H
                //if (uc.MyValue < uc.MyMinimum)
                //    uc.MyValue = uc.MyMinimum;
            }
        }
        //��������
        private static object CoerceMyValue(DependencyObject obj, object value)
        {
            //�w�肳�ꂽ�ŏ��l�̌���
            //�ő�l���傫��������A�ő�l�Ɠ����l�ɕύX����
            var uc = (UserControl1)obj;
            var min = (decimal)value;
            if (min > uc.MyMaximum) min = uc.MyMaximum;
            
            return min;
        }

        //�ő�l
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
        //�ő�l�̕ύX���ɍ��̒l�����؁A�ݒ�
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

        #endregion �ŏ��l�ƍő�l



        #endregion �ˑ��v���p�e�B


        #region �N���b�N�Ƃ��̃C�x���g����

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
        #endregion �N���b�N�Ƃ��̃C�x���g����


    }



    //���l�𕶎���ɕϊ�����Ƃ��ɁA�w�茅���ŕϊ�����p
    public class MyConverterMulti : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            decimal d = (decimal)values[0];//���l
            int i = (int)values[1];//�����̕\������
            int m = (int)values[2];//�����_�ȉ��̕\������

            //            ��������w��񐔌J��Ԃ�����������擾���� - .NET Tips(VB.NET, C#...)
            //https://dobon.net/vb/dotnet/string/repeat.html
            //�����ݒ�
            string str = new string('0', i) + "." + new string('0', m);

            return d.ToString(str);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}