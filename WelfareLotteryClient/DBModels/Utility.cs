using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media.Imaging;

namespace WelfareLotteryClient.DBModels
{
    public class Utility
    {
        /// <summary>
        /// 将图片数据转换为Base64字符串
        /// </summary>
        //public string ToBase64(object img)
        //{
        //    //Image img = this.pictureBox.Image;
        //    BinaryFormatter binFormatter = new BinaryFormatter();
        //    MemoryStream memStream = new MemoryStream();
        //    binFormatter.Serialize(memStream, img);
        //    byte[] bytes = memStream.GetBuffer();
        //    return Convert.ToBase64String(bytes);
        //    //this.richTextBox.Text = base64;
        //}

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
    }
}
