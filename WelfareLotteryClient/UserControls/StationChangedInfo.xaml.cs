using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// StationChangedInfo.xaml 的交互逻辑
    /// </summary>
    public partial class StationChangedInfo : Window
    {
        private WelfareLotteryEntities entities;
        private readonly LotteryStation station;
        public StationChangedInfo(LotteryStation s)
        {
            InitializeComponent();
            this.Icon=new DrawingImage();
            station = s;
            entities = (WelfareLotteryEntities)Application.Current.Resources["WelfareLotteryEntities"];

            cboAddedType.ItemsSource = entities.StationModifiedType.ToList();

            stationModifiedInfos=new ObservableCollection<StationModifiedInfo>(entities.StationModifiedInfo);
            lvChangedMemo.ItemsSource = stationModifiedInfos;
        }

        private readonly ObservableCollection<StationModifiedInfo> stationModifiedInfos; 

        private void btnAddInfo_Click(object sender, RoutedEventArgs e)
        {
            StationModifiedType type=cboAddedType.SelectedItem as StationModifiedType;
            string memo = txtAddedMemo.GetTextBoxText();
            DateTime? time = txtDateTime.SelectedDate;

            if (type == null || memo.IsNullOrEmpty() || null==time)
            {
                "输入变更的信息".MessageBoxDialog();
                return;
            }

            StationModifiedInfo info = new StationModifiedInfo
            {
                StationModifiedType = type,
                ModifiedTime = (DateTime) time,
                Memo = memo
            };

            station.StationModifiedInfo.Add(info);
            entities.SaveChanges();
            stationModifiedInfos.Add(info);
        }

        private void btnChangeInfo_Click(object sender, RoutedEventArgs e)
        {
            string info;
            DateTime? time;
            if (VerifyInput(out info,out time)) return;

            //it has default value
            StationModifiedType type = cboAddedType.SelectedItem as StationModifiedType;

            StationModifiedInfo modified=lvChangedMemo.SelectedItem as StationModifiedInfo;
            if (null==modified)
            {
                "选择需要编辑的数据行".MessageBoxDialog();
                return;
            }

            int index = stationModifiedInfos.IndexOf(modified);
            modified.Memo = info;
            modified.ModifiedTime = time.Value;
            modified.StationModifiedType = type;
            entities.SaveChanges();
            stationModifiedInfos.Remove(modified);
            stationModifiedInfos.Insert(index, modified);
        }

        private void lvChangedMemo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StationModifiedInfo modified = lvChangedMemo.SelectedItem as StationModifiedInfo;
            if (modified==null)
            {
                return;
            }

            cboAddedType.SelectedItem = modified.StationModifiedType;
            txtAddedMemo.Text = modified.Memo;
            txtDateTime.SelectedDate = modified.ModifiedTime;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            StationModifiedInfo lvc = (StationModifiedInfo)lvChangedMemo.SelectedItem;
            if (lvc == null)
            {
                Button btn = sender as Button;
                lvc = stationModifiedInfos.SingleOrDefault(p => p.Id == Convert.ToInt32(btn.Tag));
                if (lvc != null)
                {
                    goto perform;
                }
                "请选中您要删除的数据行".MessageBoxDialog();
                return;
            }

            perform:
            if (MessageBox.Show("您确定要删除？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                entities.StationModifiedInfo.Remove(lvc);
                entities.SaveChanges();
                stationModifiedInfos.Remove(lvc);
            }
        }

        private void btnAddChangeType_Click(object sender, RoutedEventArgs e)
        {
            ModifiedType type = new ModifiedType {Owner = this};
            type.ShowDialog();
            cboAddedType.ItemsSource = entities.StationModifiedType.ToList();
        }

        private bool VerifyInput(out string info,out DateTime? time)
        {
            info = txtAddedMemo.GetTextBoxText();
            if (info.IsNullOrEmpty())
            {
                "输入变更类型".MessageBoxDialog();
                time = null;
                return true;
            }

            time = txtDateTime.SelectedDate;
            if (null==time)
            {
                "选择时间".MessageBoxDialog();
                return true;
            }
            return false;
        }
    }
}
