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
using System.Windows.Shapes;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls.UploadImage
{
    /// <summary>
    /// PhotoView.xaml 的交互逻辑
    /// </summary>
    public partial class PhotoView : Window
    {
        Photo _photo;

        public PhotoView()
        {
            InitializeComponent();
        }

        public Photo SelectedPhoto
        {
            get { return _photo; }
            set { _photo = value; }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ViewedPhoto.Source = _photo.Image;
           // ViewedCaption.Content = _photo.Source;
        }

        private void Rotate(object sender, RoutedEventArgs e)
        {
            RotatePhoto();
        }

        private void RotatePhoto()
        {
            BitmapSource img = (BitmapSource) (_photo.Image);

            CachedBitmap cache = new CachedBitmap(img, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            _photo.Image = BitmapFrame.Create(new TransformedBitmap(cache, new RotateTransform(90.0)));

            ViewedPhoto.Source = _photo.Image;
        }

        private void Crop(object sender, RoutedEventArgs e)
        {
            BitmapSource img = (BitmapSource)(_photo.Image);

            int halfWidth = img.PixelWidth / 2;
            int halfHeight = img.PixelHeight / 2;
            _photo.Image = BitmapFrame.Create(new CroppedBitmap(img, new Int32Rect((halfWidth - (halfWidth / 2)), (halfHeight - (halfHeight / 2)), halfWidth, halfHeight)));

            ViewedPhoto.Source = _photo.Image;
        }

        private void BlackAndWhite(object sender, RoutedEventArgs e)
        {
            BitmapSource img = (BitmapSource)(_photo.Image);
            _photo.Image = BitmapFrame.Create(new FormatConvertedBitmap(img, PixelFormats.Gray8, BitmapPalettes.Gray256, 1.0));

            ViewedPhoto.Source = _photo.Image;
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RotatePhoto();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState.Minimized==this.WindowState)
            {
                WindowState=WindowState.Normal;
            }
        }
    }
}
