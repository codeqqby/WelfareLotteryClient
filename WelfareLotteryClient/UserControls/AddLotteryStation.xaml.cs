using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// AddLotteryStation.xaml 的交互逻辑
    /// </summary>
    public partial class AddLotteryStation : UserControl
    {
        OperateAdmin opa=new OperateAdmin();

        public AddLotteryStation()
        {
            InitializeComponent();
            this.clerkListBox.ItemsSource = _addedSalesclerks;

                cboAdmin.ItemsSource = opa.GetAllAdministrator();
            cboAdmin.DisplayMemberPath = "AdminName";
            cboAdmin.SelectedValuePath = "Id";
        }

        //存入图片base64对应值
        Dictionary<string, string> base64PicValue = new Dictionary<string, string> { {"hostPic","" }, {"Zheng","" }, { "Fan",""} };

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
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

                 byte[] b = u. GetPictureData(open.FileName);

                string base64 = Convert.ToBase64String(b);

                 BitmapImage myimg = u. ByteArrayToBitmapImage(Convert.FromBase64String(base64));
                switch (btn.Tag.ToString())
                {
                    case "hostPic":
                        this.HostBase64Pic.Source = myimg;
                        base64PicValue["hostPic"] = base64;
                        break;
                    case "Zheng":
                        this.HostBase64IdentityPicZheng.Source = myimg;
                        base64PicValue["Zheng"] = base64;
                        break;
                    case "Fan":
                        this.HostBase64IdentityPicFan.Source = myimg;
                        base64PicValue["Fan"] = base64;
                        break;
                    case "GuaranteeZheng":
                        this.GuaranteeBase64IdentityPicZheng.Source = myimg;
                        break;
                        case "GuaranteeFan":
                        GuaranteeBase64IdentityPicFam.Source = myimg;
                        break;
                }

            }
        }

        readonly ObservableCollection<DBModels.Salesclerk> _addedSalesclerks=new ObservableCollection<DBModels.Salesclerk>(); 

        private void btnSalesclerk_Click(object sender, RoutedEventArgs e)
        {
            AddSalesclerk salesclerk = new AddSalesclerk {Owner = Application.Current.MainWindow};

            bool? b= salesclerk.ShowDialog();
            if (!b.GetValueOrDefault()) return;
            DBModels.Salesclerk result = salesclerk.TryFindResource("InputResult") as DBModels.Salesclerk;
            _addedSalesclerks.Add(result);
        }
    }

    public class ConvertBase64ToImage:IValueConverter
    {
        readonly Utility _u=new Utility();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _u.ByteArrayToBitmapImage(System.Convert.FromBase64String((string) value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
