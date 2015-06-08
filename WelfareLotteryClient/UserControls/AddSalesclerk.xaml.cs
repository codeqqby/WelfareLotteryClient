using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.Win32;
using WelfareLotteryClient.Annotations;
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

                var tep=(WrapPanel) VisualTreeHelper.GetParent(btn);
                 Image image=(Image)tep.Children[0];

                image.Source = u.ByteArrayToBitmapImage(Convert.FromBase64String(base64));

                DBModels.Salesclerk result = FindResource("InputResult") as DBModels.Salesclerk;
                result.HeadPortraitBase64Pic = base64;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult==true)
            {
                return;
            }
            if (MessageBox.Show("您确定放弃这次添加？", "提示", MessageBoxButton.OKCancel)==MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void btnAddComplete_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DBModels.Salesclerk result = TryFindResource("InputResult") as DBModels.Salesclerk;

            if (result == null)
            {
                e.CanExecute = false;
            }
            else
            {
                int isNanTemp;
                if (string.IsNullOrEmpty(txtIdentityAddress.Text.Trim()))
                {
                    e.CanExecute = false;
                }
                else if (string.IsNullOrEmpty(txtIdentityNo.Text.Trim()))// || !int.TryParse(txtIdentityNo.Text, out isNanTemp)  因有的身份证号码有X进行占位
                {
                    e.CanExecute = false;
                }
                else if (string.IsNullOrEmpty(txtPhone.Text.Trim()) || !int.TryParse(txtPhone.Text,out isNanTemp))
                {
                    e.CanExecute = false;
                }
                else if (string.IsNullOrEmpty(txtName.Text.Trim()))
                {
                    e.CanExecute = false;
                }
                else if (string.IsNullOrEmpty(result.HeadPortraitBase64Pic))
                {
                    e.CanExecute = false;
                }
                else
                    e.CanExecute = true;
            }
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
