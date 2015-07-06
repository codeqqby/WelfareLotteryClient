using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using WelfareLotteryClient.DBModels;
using WelfareLotteryClient.UserControls.UploadImage;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// LotteryStation.xaml 的交互逻辑
    /// </summary>
    public partial class LotteryStationInfo : UserControl
    {
        WelfareLotteryEntities entities;
        private readonly List<LotteryStation> alLotteryStations;

        public LotteryStationInfo()
        {
            InitializeComponent();

            entities = (WelfareLotteryEntities) Application.Current.Resources["WelfareLotteryEntities"];

            alLotteryStations = entities.LotteryStations.ToList();
            lvStationInfo.ItemsSource = alLotteryStations;
            lvStationInfo.LoadingRow +=DataGridSoftware_LoadingRow;
        }

        private void DataGridSoftware_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1; 
        }

        //查看销售员
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn= sender as Button;
            LotteryStation station = alLotteryStations.FirstOrDefault(p => p.Id == Convert.ToInt32(btn.Tag));
            SalesclerkInfo info = new SalesclerkInfo(entities, station)
            {
                Owner = Application.Current.MainWindow
            };
            info.clerkListBox.IsEnabled = Tools.LoginUserHasRights();
            info.ShowDialog();
        }

        Utility u=new Utility();

        //查看网点照片
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            LotteryStation station = alLotteryStations.FirstOrDefault(p => p.Id == Convert.ToInt32(btn.Tag));

            var result=JsonConvert.DeserializeObject<Dictionary<string, string>>(station.StationPicListSerialized);

            PhotoCollection photo=new PhotoCollection();
            
            result.Keys.ToList().ForEach(k=>photo.Add(new Photo(k) {Image = u.ByteArrayToBitmapImage(Convert.FromBase64String(result[k]))}));
            
            ImageViewer viewer=new ImageViewer(photo) {ImageAllowDrop = Tools.LoginUserHasRights()};
            Window win = new Window
            {
                Width = 400,
                Height = 400,
                Content = viewer,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow
            };
            win.ShowDialog();

            if (!viewer.IsChanged) return;
            if (MessageBox.Show("是否提交更改变？", "提交", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
            var dicAllPhoto = viewer.Photos.ToDictionary(ph => ph.PhotoName, ph => ph.base64Value);
            station.StationPicListSerialized= JsonConvert.SerializeObject(dicAllPhoto);
            entities.SaveChanges();
        }

        //奖励卡信息
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            LotteryStation station = alLotteryStations.FirstOrDefault(p => p.Id == Convert.ToInt32(btn.Tag));

            RewardCardPage page = new RewardCardPage(station)
            {
                Owner = Application.Current.MainWindow,
                adGameType = {IsEnabled = Tools.LoginUserHasRights()}
            };
            page.ShowDialog();
            if (page.IsChanged)
            {
                entities.SaveChanges();
            }
        }

        //网点变更信息
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            LotteryStation station = alLotteryStations.FirstOrDefault(p => p.Id == Convert.ToInt32(btn.Tag));
            StationChangedInfo changedInfo = new StationChangedInfo(station)
            {
                Owner = Application.Current.MainWindow,
                adGameType = {IsEnabled = Tools.LoginUserHasRights()}
            };
            changedInfo.ShowDialog();
        }

        //福彩游戏类型
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            LotteryStation s = lvStationInfo.SelectedItem as LotteryStation;

            Button btn=sender as Button;
            if (null == s)
            {
                TextBlock block=VisualTreeHelper.GetChild(VisualTreeHelper.GetParent(btn), 0) as TextBlock;
                s = alLotteryStations.FirstOrDefault(p => p.Id == Convert.ToInt32(block.Text));
            }
            var aaaa= JsonConvert.DeserializeObject<string[]>(Convert.ToString(btn.Tag));
            SelectWelfareGameType type=new SelectWelfareGameType(aaaa);
            Window win = new Window {Content = type,SizeToContent = SizeToContent.Width,WindowStartupLocation = WindowStartupLocation.CenterOwner,Owner = Application.Current.MainWindow,MinWidth = 300,Height = 300};
            type.lvGameType.ItemsSource = entities.WelfareLotteryGameTypes.ToList();
            type.GameTypeSelectedCollection = JsonConvert.DeserializeObject<StringCollection>(Convert.ToString(btn.Tag));
            win.ShowDialog();

            if (!type.ChangedResult) return;
            var result = type.GameTypeSelectedCollection;
            string serialize= JsonConvert.SerializeObject(result);
            s.WelfareGameTypeListSerialized = serialize;
            entities.SaveChanges();
            btn.Tag = serialize;
        }

        //违规信息
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            LotteryStation station = alLotteryStations.FirstOrDefault(p => p.Id == Convert.ToInt32(btn.Tag));
            if (null == station) return;
            DockPanel panel =new DockPanel();

            TextBox resultBox = new TextBox
            {
                TextWrapping = TextWrapping.Wrap,
                IsEnabled = Tools.LoginUserHasRights(),
                Text =station.Violation,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            panel.Children.Add(resultBox);

            Window win = new Window
            {
                SizeToContent = SizeToContent.Manual,Title = "违规信息",Icon = new DrawingImage(),
                Content = panel,Width = 300,Height = 300,Owner = Application.Current.MainWindow,WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            win.Closing += (a, b) =>
            {
                //如数据库中已存储的与编辑的不同
                if (station.Violation == resultBox.Text) return;
                if (MessageBoxResult.OK != MessageBox.Show("是否提交更改", "提示", MessageBoxButton.OKCancel)) return;
                station.Violation = resultBox.Text;
                entities.SaveChanges();
            };
            win.ShowDialog();
        }

        //查看担保人
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            LotteryStation station = alLotteryStations.FirstOrDefault(p => p.Id == Convert.ToInt32(btn.Tag));
            if (null == station) return;
            
            Grid grid=new Grid();
            RowDefinition definition = new RowDefinition {Height = GridLength.Auto};
            grid.RowDefinitions.Add(definition);
            grid.RowDefinitions.Add(new RowDefinition ());

            WrapPanel panel=new WrapPanel();
            Grid.SetRow(panel,0);
            
            TextBox inputBox=new TextBox
            {
                MinWidth = 150,
                Text = station.GuaranteeName,
                Margin = new Thickness(10, 5, 5, 0),
                Style = this.FindResource("TextBoxCircleBorder") as Style
            };
            Button btnSumbit = new Button {Content = "完成修改", Margin = new Thickness(0, 5,0, 0) };
            
            panel.Children.Add(new TextBlock { Text = "担保人姓名", Margin = new Thickness(0, 5, 5, 0), VerticalAlignment = VerticalAlignment.Center });
            panel.Children.Add(inputBox);
            panel.Children.Add(btnSumbit);
            grid.Children.Add(panel);

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(station.GuaranteeBase64IdentityPic);
            PhotoCollection photo = new PhotoCollection();
            result.Keys.ToList().ForEach(k => photo.Add(new Photo(k) { Image = u.ByteArrayToBitmapImage(Convert.FromBase64String(result[k])) }));

            ImageViewer viewer = new ImageViewer(photo) {MaxDropPhoto = 2};
            Grid.SetRow(viewer,1);
            grid.Children.Add(viewer);

            Window win = new Window
            {
                Title = "担保人信息",
                Width = 320,
                Height = 300,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = grid
            };
            btnSumbit.Click += (s, er) =>
            {
                if (inputBox.GetTextBoxText() == station.GuaranteeName && !viewer.IsChanged) return;
                if (viewer.IsChanged)
                    station.GuaranteeBase64IdentityPic =JsonConvert.SerializeObject(viewer.Photos.ToDictionary(key => key.PhotoName,value => value.base64Value));
                btn.Content = station.GuaranteeName = inputBox.GetTextBoxText();
                entities.SaveChanges(); win.Close();
            };

            panel.IsEnabled = viewer.ImageAllowDrop = Tools.LoginUserHasRights();

            win.ShowDialog();
        }

        //机主信息
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            LotteryStation station = alLotteryStations.FirstOrDefault(p => p.Id == Convert.ToInt32(btn.Tag));
            if (null == station) return;
            string[] hostPhoneNum = station.HostPhoneNum.Split('⊙');

            HostInformation hostInfo = new HostInformation
            {
                txtHostPhoneNum1 = {Text = hostPhoneNum[0]},
                txtHostPhoneNum2 = {Text = hostPhoneNum[0]},
                txtHostIdentityAddress = {Text = station.HostIdentityAddress},
                txtHostName = {Text = station.HostName},
                txtHostIdentityNo = {Text = station.HostIdentityNo},
                HostBase64Pic = {Source = u.ByteArrayToBitmapImage(Convert.FromBase64String(station.HostBase64Pic))}
            };
            hostInfo.EnableAllControl();

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(station.HostBase64IdentityPic);

            PhotoCollection photo = hostInfo.hostIdentityPic.Photos;
            result.Keys.ToList().ForEach(k => photo.Add(new Photo(k) { Image = u.ByteArrayToBitmapImage(Convert.FromBase64String(result[k])) }));
            
            Window win = new Window
            {
                Width = 350,
                Height =450,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = hostInfo
            };
            win.Closing += (a, b) =>
            {
                string hostPhoneNum1 = hostInfo.txtHostPhoneNum1.GetTextBoxText();
                string hostPhoneNum2 = hostInfo.txtHostPhoneNum2.GetTextBoxText();

                station.HostName =hostInfo.txtHostName.GetTextBoxText();
                station.HostPhoneNum = $"{hostPhoneNum1}⊙{hostPhoneNum2}";
                station.HostIdentityNo = hostInfo.txtHostIdentityNo.GetTextBoxText();
                station.HostIdentityAddress = hostInfo.txtHostIdentityAddress.GetTextBoxText();
                if (null != hostInfo.HostPic) station.HostBase64Pic = hostInfo.HostPic;
                if (hostInfo.hostIdentityPic.IsChanged) station.HostBase64IdentityPic = hostInfo.Identity;

                entities.SaveChanges();
                btn.Content = station.HostName;
            };
            win.ShowDialog();
        }

        //双击查看详情
        private void lvStationInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LotteryStation s = lvStationInfo.SelectedItem as LotteryStation;
            if (null == s) return;

            StationInfoSingleDetails details = new StationInfoSingleDetails(s) { Owner = Application.Current.MainWindow };
            details.IsEnabledControl();
            details.ShowDialog();
            lvStationInfo.SelectedItem = null; //防止点击过一次后点击空白处还会自发 只有再次重新选择后才可以
            e.Handled = true;
        }
    }

    public class BoolTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string type = System.Convert.ToString(parameter);
            bool v = System.Convert.ToBoolean(value);
            switch (type)
            {
                case "MachineType":
                    return v ? "双机" : "单机";
                case "CommunicationType":
                    return v ? "CDMA" : "ADSL";
                case "PropertyRight":
                    return v ? "自有" : "租赁";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
