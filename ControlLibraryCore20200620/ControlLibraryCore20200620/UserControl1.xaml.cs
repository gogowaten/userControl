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

//WPF�ɂ�NumericUpDown�݂����Ȃ̂����[�U�[�R���g���[���ŁA����1 - �ߌ��Ă�̃u���O
//https://gogowaten.hatenablog.com/entry/2020/06/20/205352

//WPF�ɂ�NumericUpDown�݂����Ȃ̂����[�U�[�R���g���[���ŁA����7 - �ߌ��Ă�̃u���O
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
            var tb = (TextBox)sender;
            string text = tb.Text;

            //�s���I�h���擪�������ɂ������ꍇ�͍폜
            if (text.StartsWith('.') || text.EndsWith('.'))
            {
                text = text.Replace(".", "");
            }


            //���l�ɕϊ��ł��Ȃ�������Ȃ璼�O�̐��l�ɖ߂�            
            if (decimal.TryParse(text, out decimal m) == false)
            {
                var old = MyValue;
                tb.Text = old.ToString(MyStringFormat);
                MyValue = old;
                return;
            }
            //���l�ɕϊ��ł���Ȃ�A���̒l���Z�b�g
            else
            {
                tb.Text = m.ToString(MyStringFormat);
                MyValue = m;
            }



            //// -. ���ςȂ̂Ńs���I�h�����폜
            //text = text.Replace("-.", "-");

            ////���l���Ȃ��̂Ƀn�C�t����s���I�h���������ꍇ�͍폜
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
            //�\��t������
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        //focus�����Ƃ�
        //MyValue����Text�������ĕ\�����ăe�L�X�g��S�I��
        private void MyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            tb.Text = MyValue.ToString();
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
        #region MyValue, MyText
        //�v�̒l
        public decimal MyValue
        {
            get { return (decimal)GetValue(MyValueProperty); }
            set
            {
                //CoerceMyValue�����ł͕s�\���������̂ł����ł��A
                //�V�����l����������𒴂��Ă����珑�������Ă���Set����
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

        #region ValueChanged�C�x���g
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
        #endregion ValueChanged�C�x���g

        //MyValue�̕ύX����̓���
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
                ud.MyValue = newValue;//�d�v�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I
            }
            //�ǉ�����ValueChanged�C�x���g�p����
            ud.OnMyValueChanged(new MyValuechangedEventArgs(MyValueChangedEvent, newValue, ovlValue));
        }

        //MyValue�̕ύX���O�̓���A�l�̌��؁A����������Βl�����������ĉ���
        //���͂��ꂽ�l�������l��菬�����ꍇ�͉����l�ɏ�������
        //���͂��ꂽ�l������l���傫���ꍇ�͏���l�ɏ�������
        private static object CoerceMyValue(DependencyObject d, object basaValue)
        {
            var ud = d as NumericUpDown;
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
        //���A���^�C���X�V�ŏ����w��͖���������H

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
            decimal m = ud.MyValue;

            ud.MyText = ud.MyValue.ToString(sf);
            ud.MyValue = m;

        }

        //�V���������̔���A�s�K�؂ȏꍇ�͌Â������ŏ㏑������
        private static object CoerceMyStrinfFormatValue(DependencyObject d, object baseValue)
        {
            var ud = d as NumericUpDown;
            var format = (string)baseValue;//�V��������

            //���̒l�A���̒l�A0�̂Ƃ��ŏ�����ς�����
            //�Z�~�R�����ŋ�؂�
            //(���̒l���� ; ���̒l���� ; 0�̏���)

            //�������[�v�ɂȂ鏑���͐��l�ɕϊ��ł���A����
            // �� 0�ȊO
            // �� �u,.�v���܂܂�Ă���
            // �� ���̒l�̏����̐擪���n�C�t��
            // �� ���̒l�̏����̐擪���n�C�t���ł͂Ȃ�            
            //MyValue��MyText��Callback�ԂŖ������[�v�ɂȂ���StackOverflow�ɂȂ��Ă��܂�

            //���A���A0�A���ꂼ��𔻒肷�邽�߁u;�v�ŕ�����𕪊�
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
                    //�V���������K�p���Ă݂ăG���[�Ȃ�catch�ɔ��
                    var text = ud.MyValue.ToString(format);

                    //���̒l�����̒l�ɕω�����悤�ȏ������e��
                    decimal dc = 1m;
                    if (decimal.TryParse(dc.ToString(format), out decimal dd))
                    {
                        //�u-.�v��u-,.�v�Ȃǂ́u-�v�Ɠ������ʂŁA���̒l�����̒l�ɔ��]����
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
        #endregion Small, Large, Min, Max


        //�{�^���́��Ɓ��̐F�w��AColors�̐F��or#xxxxxx���w��
        public Brush MyButtonMarkColor
        {
            get { return (Brush)GetValue(MyButtonMarkColorProperty); }
            set { SetValue(MyButtonMarkColorProperty, value); }
        }

        public static readonly DependencyProperty MyButtonMarkColorProperty =
            DependencyProperty.Register(nameof(MyButtonMarkColor), typeof(Brush), typeof(NumericUpDown),
                new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));


        //�����̕\���ʒu�ATextAlignment���w��A����l�͉E��
        public TextAlignment MyTextAlignment
        {
            get { return (TextAlignment)GetValue(textAlignmentProperty); }
            set { SetValue(textAlignmentProperty, value); }
        }

        public static readonly DependencyProperty textAlignmentProperty =
            DependencyProperty.Register(nameof(MyTextAlignment), typeof(TextAlignment), typeof(NumericUpDown),
                new PropertyMetadata(TextAlignment.Right));




        #endregion �ˑ��֌W�v���p�e�B



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


    //�l�ύX���̃C�x���g�p�N���X
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




    //RepeatButton��Width��UserControl�S�̂�Height����v�Z����p�̃R���o�[�^
    public class MyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value;//�S�̂�Height
            double width = d / 3.0;//1/2�ȉ����Ɩ������[�v�ɂȂ�A1/2.1�`1/3�����肪����
            return width;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    ////���g�p�A����g����-0.�Ƃ�0.�����͂ł��Ȃ�
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
    //            return new ValidationResult(false, "���l����Ȃ�");
    //        }
    //    }
    //}
}