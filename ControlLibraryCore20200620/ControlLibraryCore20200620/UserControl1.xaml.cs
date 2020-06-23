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

        //�t�H�[�J�X�������A�s���R�ȕ������폜
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
            if (text == "-" || text == ".")
                text = "";

            tb.Text = text;
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
            DependencyProperty.Register(nameof(MyValue), typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(0m));



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



        #endregion



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