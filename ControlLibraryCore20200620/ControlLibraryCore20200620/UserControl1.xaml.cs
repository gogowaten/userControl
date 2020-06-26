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

        #region ���͐���
        //�X�y�[�X�L�[�������ꂽ�̂𖳌��ɂ���
        private void MyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        //���͂̐����A�����ƃn�C�t���ƃs���I�h�����ʂ�
        private void MyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textbox = (TextBox)sender;
            string str = textbox.Text;//������
            var inputStr = e.Text;//���͂��ꂽ����            

            //���K�\���œ��͕����̔���A�����ƃs���I�h�A�n�C�t���Ȃ�true
            bool neko = new System.Text.RegularExpressions.Regex("[0-9.-]").IsMatch(inputStr);

            //���͕��������l�ƃs���I�h�A�n�C�t���ȊO�������疳��
            if (neko == false)
            {
                e.Handled = true;//����
                return;//�I��
            }

            //�L�����b�g(�J�[�\��)�ʒu���擪(0)����Ȃ��Ƃ��́A�n�C�t�����͖͂���
            if (textbox.CaretIndex != 0 && inputStr == "-") { e.Handled = true; return; }

            //2�ڂ̃n�C�t�����͖͂���(�S�I�����Ȃ狖��)
            if (textbox.SelectedText != str)
            {
                if (str.Contains("-") && inputStr == "-") { e.Handled = true; return; }
            }

            //2�ڂ̃s���I�h���͖͂���
            if (str.Contains(".") && inputStr == ".") { e.Handled = true; return; }
        }

        //�t�H�[�J�X�������A�s���R�ȕ������폜�Ə����K�p
        private void MyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //�s���I�h�̍폜
            //�擪�������ɂ������ꍇ�͍폜
            var tb = (TextBox)sender;
            string text = tb.Text;
            if (text.StartsWith('.') || text.EndsWith('.'))
            {
                text = text.Replace(".", "");
            }

            // -. ���ςȂ̂Ńs���I�h�����폜
            text = text.Replace("-.", "-");

            //���l���Ȃ��̂Ƀn�C�t����s���I�h���������ꍇ�͍폜
            if (text == "-" || text == "." || text == "-0")
                text = "0";
            MyText = text;
            tb.Text = MyValue.ToString(MyStringFormat);
        }

        //
        private void MyTextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //�\��t������
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        //focus�����Ƃ��Ƀe�L�X�g��S�I��
        private void MyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            tb.SelectAll();
        }

        //        | �I�[���g�̉_
        //http://ooltcloud.sakura.ne.jp/blog/201311/article_30013700.html
        //�N���b�N�����Ƃ��Ƀe�L�X�g��S�I��
        private void MyTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb.IsFocused == false)
            {
                tb.Focus();
                e.Handled = true;
            }
        }
        #endregion ���͐���


        #region �ˑ��֌W�v���p�e�B

        //�v�̒l
        public decimal MyValue
        {
            get { return (decimal)GetValue(MyValueProperty); }
            set { SetValue(MyValueProperty, value); }
        }

        public static readonly DependencyProperty MyValueProperty =
            DependencyProperty.Register(nameof(MyValue), typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0m,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnMyValuePropertyChanged, CoerceMyValue));

        //MyValue�̕ύX����̓���
        private static void OnMyValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ud = d as NumericUpDown;
            var m = (decimal)e.NewValue;
            if (ud.MyTextBox.IsFocused)
            {
                ud.MyText = m.ToString();
            }
            else
            {
                ud.MyText = m.ToString(ud.MyStringFormat);
            }
        }

        //MyValue�̕ύX���O�̓���A�l�̌��؁A����������Βl�����������ĉ���
        //���͂��ꂽ�l�������l��菬�����ꍇ�͉����l�ɏ�������
        //���͂��ꂽ�l������l���傫���ꍇ�͏���l�ɏ�������
        private static object CoerceMyValue(DependencyObject d, object basaValue)
        {
            var ud = (NumericUpDown)d;
            var m = (decimal)basaValue;
            if (m < ud.MyMinValue) m = ud.MyMinValue;
            if (m > ud.MyMaxValue) m = ud.MyMaxValue;
            return m;
        }


        //        wpf - �W���ˑ��v���p�e�B | wpf Tutorial
        //https://riptutorial.com/ja/wpf/example/9857/%E6%A8%99%E6%BA%96%E4%BE%9D%E5%AD%98%E3%83%97%E3%83%AD%E3%83%91%E3%83%86%E3%82%A3
        //���
        //�o�C���h��Mode=TwoWay �i TextBox.Text�̓���ɗގ��j���w�肷��K�v����r������ɂ́A 
        //PropertyMetadata�����FrameworkPropertyMetadata���g�p���A�K�؂ȃt���O���w�肷��悤�ɃR�[�h���X�V���܂��B
        //�e�L�X�g�{�b�N��Binding����
        public string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }

        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register(nameof(MyText), typeof(string), typeof(NumericUpDown),
                new FrameworkPropertyMetadata("",FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,OnMyTextPropertyChanged));

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
        //�����w��p�̕�����^�ˑ��֌W�v���p�e�B
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
            ud.MyText = ud.MyValue.ToString(sf);
        }
        private static object CoerceMyStrinfFormatValue(DependencyObject d, object baseValue)
        {
            //�V����������K�p����ƃG���[�ɂȂ�ꍇ�́A���̏����ɏ���������
            var ud = d as NumericUpDown;
            var s = (string)baseValue;//�V��������
            try
            {
                //�V���������K�p
                ud.MyValue.ToString(s);
            }
            catch (Exception)
            {
                //�G���[�Ȃ猳�̏����ɏ�������
                s = ud.MyStringFormat;
            }
            return s;
        }


        //���ύX�l
        public decimal MySmallChange
        {
            get { return (decimal)GetValue(MySmallChangeProperty); }
            set { SetValue(MySmallChangeProperty, value); }
        }
        public static readonly DependencyProperty MySmallChangeProperty =
            DependencyProperty.Register(nameof(MySmallChange), typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(1m));


        //��ύX�l
        public decimal MyLargeChange
        {
            get { return (decimal)GetValue(MyLargeChangeProperty); }
            set { SetValue(MyLargeChangeProperty, value); }
        }
        public static readonly DependencyProperty MyLargeChangeProperty =
            DependencyProperty.Register(nameof(MyLargeChange), typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(10m));



        //�����l
        public decimal MyMinValue
        {
            get { return (decimal)GetValue(MyMinValueProperty); }
            set { SetValue(MyMinValueProperty, value); }
        }
        public static readonly DependencyProperty MyMinValueProperty =
            DependencyProperty.Register(nameof(MyMinValue), typeof(decimal), typeof(NumericUpDown),
                new PropertyMetadata(decimal.MinValue, OnMyMinValuePropertyChanged, CoerceMyMinValue));

        //PropertyChanged�R�[���o�b�N�A�v���p�e�B�l�ύX"����"�Ɏ��s�����
        //�ύX���ꂽ�����l�ƍ��̒l�ł̖���������
        //�ύX���ꂽ�V���������l�ƁA���̒l(MyValue)�Ŗ�����������(�����l < ���̒l)�ꍇ�́A���̒l�������l�ɕύX����
        private static void OnMyMinValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ud = (NumericUpDown)d;
            var min = (decimal)e.NewValue;//�ύX��̐V���������l
            if (min > ud.MyValue) ud.MyValue = min;
        }

        //�l�̌��؂ƕύX
        //CoerceValue�R�[���o�b�N�A�v���p�e�B�l�ύX"���O"�Ɏ��s�����
        //�ݒ肳�ꂽ�l������(Coerce)�I�ɕύX�ł���̂ŁA����������ΕύX���ĉ�������
        //���͂��ꂽ�����l�ƁA���̏���l�Ŗ�����������(�����l > ����l)�ꍇ�́A�����l������l�ɏ���������
        private static object CoerceMyMinValue(DependencyObject d, object baseValue)
        {
            var ud = (NumericUpDown)d;
            var min = (decimal)baseValue;//���͂��ꂽ�����l
            if (min > ud.MyMaxValue) min = ud.MyMaxValue;
            return min;
        }


        //����l
        public decimal MyMaxValue
        {
            get { return (decimal)GetValue(MyMaxValueProperty); }
            set { SetValue(MyMaxValueProperty, value); }
        }
        public static readonly DependencyProperty MyMaxValueProperty =
            DependencyProperty.Register(nameof(MyMaxValue), typeof(decimal), typeof(NumericUpDown),
                new PropertyMetadata(decimal.MaxValue, OnMyMaxValuePropertyChanged, CoerceMyMaxValue));

        //����l�̕ύX����̓���B����l��荡�̒l���傫���ꍇ�́A���̒l������l�ɕύX����
        private static void OnMyMaxValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ud = (NumericUpDown)d;
            var max = (decimal)e.NewValue;
            if (max < ud.MyValue) ud.MyValue = max;
        }

        //����l�ύX���O�̓���B���͂��ꂽ����l�����̉����l��菬�����Ȃ�ꍇ�́A����l�������l�ɏ���������
        private static object CoerceMyMaxValue(DependencyObject d, object baseValue)
        {
            var ud = (NumericUpDown)d;
            var max = (decimal)baseValue;
            if (max < ud.MyMinValue) max = ud.MyMinValue;
            return max;
        }



        #endregion �ˑ��֌W�v���p�e�B



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