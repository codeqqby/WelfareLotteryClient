using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WelfareLotteryClient.DBModels
{
    public class Utility
    {
        /// <summary>
        /// 将图片数据转换为Base64字符串
        /// </summary>
        public byte[] ToBase64(object img)
        {
            //Image img = this.pictureBox.Image;
            BinaryFormatter binFormatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            binFormatter.Serialize(memStream, img);
            //byte[] bytes = 
              return  memStream.GetBuffer();
            //return Convert.ToBase64String(bytes);
            //this.richTextBox.Text = base64;
        }

        /// <summary>
        /// 将Base64字符串转换为图片
        /// </summary>
        //public System.Drawing.Bitmap ToImage(string base64)
        //{
        //    byte[] bytes = Convert.FromBase64String(base64);
        //    MemoryStream memStream = new MemoryStream(bytes);
        //    BinaryFormatter binFormatter = new BinaryFormatter();
        //    return (System.Drawing.Bitmap)binFormatter.Deserialize(memStream);
        //}

        /// <summary>
        /// 获取文件数组
        /// </summary>
        public byte[] GetPictureData(string imagepath)
        {
            /**/
            ////根据图片文件的路径使用文件流打开，并保存为byte[]   
            FileStream fs = new FileStream(imagepath, FileMode.Open);//可以是其他重载方法   
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            fs.Close();
            return byData;
        }

        /// <summary>
        /// 将字节数组转换为BitmapImage
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bmp = null;

            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }

            return bmp;
        }

        public byte[] BitmapImageToByteArray(BitmapImage bmp)
        {
            byte[] byteArray = null;
            
                Stream sMarket = bmp.StreamSource;

                if (sMarket != null && sMarket.Length > 0)
                {
                    //很重要，因为Position经常位于Stream的末尾，导致下面读取到的长度为0。   
                    sMarket.Position = 0;

                    using (BinaryReader br = new BinaryReader(sMarket))
                    {
                        byteArray = br.ReadBytes((int)sMarket.Length);
                    }
                }
            return byteArray;
        }

        /// <summary>
        /// 两个相同的的属性进行属性值Copy
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public object CopyProperties(object origin, object target)
        {
            Type originType = origin.GetType();
            Type targetType = target.GetType();
            
            foreach (PropertyInfo property in  originType.GetProperties())
            {
                if (!property.CanRead) continue;
                var value = property.GetValue(origin);
                var ta = targetType.GetProperty(property.Name);
                if (ta!=null && property.PropertyType==ta.PropertyType && ta.CanWrite)
                {
                    ta.SetValue(target, value); 
                }
            }
            return null;
        }
    }

    public static class Tools
    {
        public static void MessageBoxDialog(this string msg)
        {
            MessageBox.Show(msg);
        }

        public static string GetTextBoxText(this TextBox box)
        {
            return box.Text.Trim();
        }

        public static bool IsNullOrEmpty(this string msg)
        {
            return string.IsNullOrEmpty(msg);
        }

        public static bool LoginUserHasRights()
        {
            LoginedUserInfo l=Application.Current.MainWindow.Tag as LoginedUserInfo;
            return l.Permitted;
        }
    }

    /// <summary>
    /// 用户登录信息
    /// </summary>
    public class UserLoginInfo
    {
        /// <summary>
        /// 以用户名或邮箱查询用户
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public Dictionary<string, string> IsAdmitted(AspNetUser users, UploadDataCompletedEventHandler callback)
        {
            try
            {
                WebClient client = new WebClient { Encoding = Encoding.UTF8 };
                StringBuilder postData = new StringBuilder();
                postData.Append($"Id={users.UserName} ");
                postData.Append($"&pass={users.PasswordHash}");

                byte[] sendData = Encoding.UTF8.GetBytes(postData.ToString());
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("ContentLength", sendData.Length.ToString());
                //var recData = client.UploadData(ConfigurationManager.AppSettings["loginVerification"], "POST",sendData);

                client.UploadDataAsync(new Uri(ConfigurationManager.AppSettings["loginVerification"]), "POST", sendData);
                client.UploadDataCompleted += callback;

                // string aaaa = Encoding.UTF8.GetString(aa);
                //client.Dispose();

                return null;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string> { { "Errors", ex.Message } };
            }
        }
    }
    public class LoginedUserInfo
    {
        public string UGuid { get; set; }
        public string UName { get; set; }
        public string RoleName { get; set; }
        public bool Permitted { get; set; }
    }
}
