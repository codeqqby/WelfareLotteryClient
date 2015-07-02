using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Newtonsoft.Json;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// HostInformation.xaml 的交互逻辑
    /// </summary>
    public partial class HostInformation : UserControl
    {
        public HostInformation()
        {
            InitializeComponent();
        }
        
        //机主照片和身份证正反 存入图片base64对应值
        public string HostPic { get; set; }

        public string Identity=> JsonConvert.SerializeObject(hostIdentityPic.Photos.ToDictionary(p => p.PhotoName, p => p.base64Value));
    
        /// <summary>
        /// 已经托入的图片数量
        /// </summary>
        public int DropImageCount => hostIdentityPic.Photos.Count;

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog
            {
                Title = "打开文件",
                Filter = "图片(*.jpg;*.png;*.gif;*.bmp;*.jpeg)|*.jpg;*.png;*.gif;*.bmp;*.jpeg"
            }; //定义打开文本框实体
            //对话框标题
            //文件扩展名
            if (!open.ShowDialog().GetValueOrDefault()) return;

            Utility u = new Utility();

            byte[] b = u.GetPictureData(open.FileName);

            string base64 = Convert.ToBase64String(b);

            BitmapImage myimg = u.ByteArrayToBitmapImage(Convert.FromBase64String(base64));

            HostBase64Pic.Source = myimg;
            HostPic = base64;
        }
    }
}
