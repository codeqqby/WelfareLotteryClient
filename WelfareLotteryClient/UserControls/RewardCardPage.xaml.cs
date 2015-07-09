using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// RewardCardPage.xaml 的交互逻辑
    /// </summary>
    public partial class RewardCardPage : Window
    {
        private readonly LotteryStation station;
        WelfareLotteryEntities entities;
        public RewardCardPage(LotteryStation station)
        {
            InitializeComponent();
            this.station = station;
            _rewardCardInfos=new ObservableCollection<RewardCardInfo>(station.RewardCardInfoes);
            lvRewardCard.ItemsSource = _rewardCardInfos;
            entities = (WelfareLotteryEntities)Application.Current.Resources["WelfareLotteryEntities"];
        }

        private readonly ObservableCollection<RewardCardInfo> _rewardCardInfos; 

        private void btnDelRewardCard_Click(object sender, RoutedEventArgs e)
        {
            RewardCardInfo lvc = (RewardCardInfo)lvRewardCard.SelectedItem;
            if (lvc == null)
            {
                Button btn = sender as Button;
                lvc = station.RewardCardInfoes.Single(p => p.Id == Convert.ToInt32(btn.Tag));
                if (lvc != null)
                {
                    goto perform;
                }
                MessageBox.Show("请选中您要删除的奖励卡");
                return;
            }

            perform:
            if (MessageBox.Show("您确定要删除？", "提示", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
            station.RewardCardInfoes.Remove(lvc);
            LoginedUserInfo us = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = us.UGuid,
                Username = us.UName,
                Memo = $"删除编号为【{lvc.Id}】-姓名为【{lvc.CardName}】的奖励卡信息",
                OptType = (int)OptType.删除,
                OptTime = DateTime.Now
            });
            entities.SaveChanges();
            _rewardCardInfos.Remove(lvc);
        }

        private void lvGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RewardCardInfo info=lvRewardCard.SelectedItem as RewardCardInfo;
            if (info == null) return;

            txtAddedCardNo.Text = info.CardNum;
            txtAddedIdentity.Text= info.CardIdentityNo;
            txtAddedName.Text = info.CardName;
        }

        private void btnAddRewardCard_Click(object sender, RoutedEventArgs e)
        {
            Button btn = ((Button)sender);
            btn.IsEnabled = false;

            string addedName = txtAddedName.GetTextBoxText();
            string addedIdentity = txtAddedIdentity.GetTextBoxText();
            string addedNo = txtAddedCardNo.GetTextBoxText();

            if (addedName.IsNullOrEmpty() || addedIdentity.IsNullOrEmpty() || addedNo.IsNullOrEmpty())
            {
                MessageBox.Show("请正确输入奖励卡数据！");
                btn.IsEnabled = true;
                return;
            }

            bool hasName=station.RewardCardInfoes.Count(info => info.CardIdentityNo == addedIdentity) > 0;

            if (hasName)
            {
                if (MessageBox.Show("已经有此身份证号，是否继续？", "提示",MessageBoxButton.OKCancel)==MessageBoxResult.Cancel)
                {
                    btn.IsEnabled = true;
                    return;
                }
            }

            var obj = new RewardCardInfo
            {
                CardName = addedName,
                CardIdentityNo = addedIdentity,
                CardNum = addedNo

            };

            station.RewardCardInfoes.Add(obj);
            entities.SaveChanges();
            LoginedUserInfo us = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = us.UGuid,
                Username = us.UName,
                Memo = $"添加编号为【{obj.Id}】的奖励卡信息",
                OptType = (int)OptType.新增,
                OptTime = DateTime.Now
            });
            entities.SaveChanges();
            _rewardCardInfos.Add(obj);
            btn.IsEnabled = true;
        }

        //public bool IsChanged { get; set; }

        private void btnChangeRewardCard_Click(object sender, RoutedEventArgs e)
        {
            RewardCardInfo card = lvRewardCard.SelectedItem as RewardCardInfo;
            if (card == null)
            {
                MessageBox.Show("请选择需要更改的奖励卡");
                return;
            }
            
            Button btn = ((Button)sender);
            btn.IsEnabled = false;

            string addedName = txtAddedName.GetTextBoxText();
            string addedIdentity = txtAddedIdentity.GetTextBoxText();
            string addedNo = txtAddedCardNo.GetTextBoxText();

            if (addedName.IsNullOrEmpty() || addedIdentity.IsNullOrEmpty() || addedNo.IsNullOrEmpty())
            {
                MessageBox.Show("请正确输入奖励卡数据！");
                btn.IsEnabled = true;
                return;
            }

            bool hasName = station.RewardCardInfoes.Count(info => info.CardIdentityNo == addedIdentity) > 0;

            if (hasName)
            {
                if (MessageBox.Show("已经有此身份证号，是否继续？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    btn.IsEnabled = true;
                    return;
                }
            }

            int index=_rewardCardInfos.IndexOf(card);
            _rewardCardInfos.Remove(card);
            card.CardIdentityNo = addedIdentity;
            card.CardName = addedName;
            card.CardNum = addedNo;
            LoginedUserInfo us = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = us.UGuid,
                Username = us.UName,
                Memo = $"编辑编号为【{card.Id}】的奖励卡信息",
                OptType = (int)OptType.修改,
                OptTime = DateTime.Now
            });
            entities.SaveChanges();
            _rewardCardInfos.Insert(index,card);
            btn.IsEnabled = true;
        }
    }
}
