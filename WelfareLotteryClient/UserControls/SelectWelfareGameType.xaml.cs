using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// SelectWelfareGameType.xaml 的交互逻辑
    /// </summary>
    public partial class SelectWelfareGameType : UserControl
    {
        public SelectWelfareGameType()
        {
            InitializeComponent();
            GameTypeSelectedCollection = FindResource("GameTypeSelectedCollection") as StringCollection;
        }

        public SelectWelfareGameType(string[] collection)
        {
            InitializeComponent();
            GameTypeSelectedCollection = FindResource("GameTypeSelectedCollection") as StringCollection;
            InitSelectedCollection(collection);
        }

        public void InitSelectedCollection(string[] collection)
        {
            GameTypeSelectedCollection.AddRange(collection);
        }

        /// <summary>
        /// 结果集是否更改过
        /// </summary>
        public bool ChangedResult { get; set; }

        public StringCollection GameTypeSelectedCollection { get; set; }

        // 游戏类型选择
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox) sender;
            string aa = Convert.ToString(chkZone.Tag);
            if (chkZone.IsChecked != null && (!GameTypeSelectedCollection.Contains(aa) && chkZone.IsChecked.Value))
                GameTypeSelectedCollection.Add(aa);
            else
                GameTypeSelectedCollection.Remove(aa);
            ChangedResult = true;
        }

        private void CheckBox_Initialized(object sender, EventArgs e)
        {
            CheckBox box=sender as CheckBox;
            box.IsEnabled = Tools.LoginUserHasRights();
        }
    }

    public class BindCheckBox : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringCollection s = parameter as StringCollection;
            return s.Cast<string>().Any(temp => System.Convert.ToString(value) == temp);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
