using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// AddSalesclerk.xaml 的交互逻辑
    /// </summary>
    public partial class AddSalesclerk : Window
    {
        public AddSalesclerk()
        {
            InitializeComponent();
            _result = TryFindResource("InputResult") as Salesclerk;
        }

        /// <summary>
        /// 需将参数中的Salesckerk属性值赋值给resource的需要此构造
        /// </summary>
        /// <param name="clerk"></param>
        public AddSalesclerk(Salesclerk clerk):this()
        {
            new Utility().CopyProperties(clerk, _result);
        }

        private void btnClearkIcon_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog
            {
                Title = "打开文件",
                Filter = "图片(*.jpg;*.png;*.gif;*.bmp;*.jpeg)|*.jpg;*.png;*.gif;*.bmp;*.jpeg"
            }; //定义打开文本框实体
            //对话框标题
            //文件扩展名
            if ((bool)open.ShowDialog().GetValueOrDefault())//打开
            {
                Button btn = sender as Button;

                Utility u = new Utility();

                byte[] b = u.GetPictureData(open.FileName);

                string base64 = Convert.ToBase64String(b);

                var tep = (WrapPanel)VisualTreeHelper.GetParent(btn);
                Image image = (Image)tep.Children[0];

                image.Source = u.ByteArrayToBitmapImage(Convert.FromBase64String(base64));
                _result.HeadPortraitBase64Pic = base64;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DialogResult == true)
            {
                return;
            }
            if (MessageBox.Show("您确定放弃这次操作？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void btnAddComplete_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        readonly Regex rex = new Regex(@"^\d+$");
        readonly Salesclerk _result;

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIdentityAddress.GetTextBoxText()))
            {
                e.CanExecute = false;
            }
            else if (string.IsNullOrEmpty(txtIdentityNo.GetTextBoxText())) //  因有的身份证号码有X进行占位 所以不完全都是数字的
            {
                e.CanExecute = false;
            }
            else if (string.IsNullOrEmpty(txtPhone.GetTextBoxText()) || !rex.IsMatch(txtPhone.GetTextBoxText()))
            {
                e.CanExecute = false;
            }
            else if (string.IsNullOrEmpty(txtName.GetTextBoxText()))
            {
                e.CanExecute = false;
            }
            else if (string.IsNullOrEmpty(_result.HeadPortraitBase64Pic))
            {
                e.CanExecute = false;
            }
            else
                e.CanExecute = true;
            //路由终止，提高系统性能  
            e.Handled = true;
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }


        //propdp

        //public List<Salesclerk> Salesclerks
        //{
        //    get { return (List<Salesclerk>)GetValue(SalesclerksProperty); }
        //    set { SetValue(SalesclerksProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Salesclerks.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SalesclerksProperty =
        //    DependencyProperty.Register("Salesclerks", typeof(List<Salesclerk>), typeof(List<Salesclerk>), new PropertyMetadata(0));

    }
}
