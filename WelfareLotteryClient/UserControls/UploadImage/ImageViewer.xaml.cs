using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls.UploadImage
{
    /// <summary>
    /// ImageViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewer : UserControl
    {
        public PhotoCollection Photos;
        public ImageViewer()
        {
            InitializeComponent();
            Photos = (PhotoCollection)(Resources["Photos"] as ObjectDataProvider).Data;
            gridContainer.DataContext = Photos;
        }

        private void OnPhotoClick(object sender, RoutedEventArgs e)
        {
            PhotoView pvWindow = new PhotoView
            {
                SelectedPhoto = (Photo) PhotosListBox.SelectedItem,
                Owner =Application.Current.MainWindow
            };
            pvWindow.ShowDialog();
        }

        private void GroupBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            //去掉文件夹 和 不是以图片后缀结果的文件

            var aa= ((string[]) e.Data.GetData(DataFormats.FileDrop)).Where(b => !Directory.Exists(b) && Extension(b)).ToList();

            if (!aa.Any())
            {
                MessageBox.Show("只能托入" + string.Join("", _extension) + "结尾的图片");
                return;
            }

            if (MaxDropPhoto.HasValue && aa.Count > MaxDropPhoto.Value)
            {
                MessageBox.Show($"最多只能添加{MaxDropPhoto.Value}张图片");
                return;
            }

            Photos.Update(aa);

        }

        readonly string[] _extension={".jpg",".png", ".gif", ".bmp", ".jpeg"};

        public bool Extension(string path)
        {
            return _extension.Any(p => p == Path.GetExtension(path)?.ToLower());
        }

        //propdp
        /// <summary>
        /// 可托拽最大文件数
        /// </summary>
        public int? MaxDropPhoto
        {
            get { return (int?)GetValue(MaxDropPhotoProperty); }
            set { SetValue(MaxDropPhotoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxDropPhoto.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxDropPhotoProperty =
            DependencyProperty.Register("MaxDropPhoto", typeof(int?), typeof(ImageViewer), new PropertyMetadata(null));

        //FrameworkPropertyMetadata

        //propa

        public Visibility ZoomVisibility
        {
            get { return (Visibility)GetValue(ZoomVisibilityProperty); }
            set { SetValue(ZoomVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomVisibilityProperty =
            DependencyProperty.Register("ZoomVisibility", typeof(Visibility), typeof(ImageViewer), new PropertyMetadata(Visibility.Visible));
        
    }
       
}
