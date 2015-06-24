using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// WelfareLotteryGameTypeOpr.xaml 的交互逻辑
    /// </summary>
    public partial class WelfareLotteryGameTypeOpr : UserControl
    {
        readonly OperateWelfareLotteryGameType optWelfareLotteryGameType = new OperateWelfareLotteryGameType();

        public WelfareLotteryGameTypeOpr()
        {
            InitializeComponent(); RefreshWelfareLotteryGameType();
        } 
        
        //刷新福彩游戏类型Listview
        private void RefreshWelfareLotteryGameType()
        {
            List<WelfareLotteryGameType> gameTypes = optWelfareLotteryGameType.GetAllWelfareLotteryGameTypes();
            lvGameType.ItemsSource = gameTypes;
        }

        private void lvGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WelfareLotteryGameType type = (WelfareLotteryGameType)lvGameType.SelectedItem;
            if (type == null)
            {
                return;
            }

            txtAddedGameType.Text = type.GameType;
            txtAddedGameRebate.Text = type.Rebate;
        }

        private void btnDelGameType_Click(object sender, RoutedEventArgs e)
        {
            WelfareLotteryGameType lvc = (WelfareLotteryGameType)lvGameType.SelectedItem;
            if (lvc == null)
            {
                Button btn = sender as Button;
                List<WelfareLotteryGameType> orginal = (List<WelfareLotteryGameType>)lvGameType.Items.SourceCollection;

                lvc = orginal.Find(p => p.Id == Convert.ToInt32(btn.Tag));
                if (lvc != null)
                {
                    goto perform;
                }
                MessageBox.Show("请选中您要删除的游戏类型");
                return;
            }

            perform:
            if (MessageBox.Show("您确定要删除？", "提示", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
            optWelfareLotteryGameType.DelGameType(lvc);
            RefreshWelfareLotteryGameType();
        }

        private void btnChangeGameType_Click(object sender, RoutedEventArgs e)
        {
            Button btn = ((Button)sender);
            btn.IsEnabled = false;
            WelfareLotteryGameType type = (WelfareLotteryGameType)lvGameType.SelectedItem;
            if (type == null)
            {
                MessageBox.Show("请选择需要删除的游戏类型！"); btn.IsEnabled = true;
                return;
            }
            if (string.IsNullOrEmpty(txtAddedGameType.Text.Trim()) || string.IsNullOrEmpty(txtAddedGameRebate.Text.Trim()))
            {
                MessageBox.Show("请正确输入游戏数据！"); btn.IsEnabled = true;
                return;
            }

            type.GameType = txtAddedGameType.Text;
            type.Rebate = txtAddedGameRebate.Text;

            optWelfareLotteryGameType.EditGameType(type);
            RefreshWelfareLotteryGameType(); btn.IsEnabled = true;
        }

        private void btnAddGameType_Click(object sender, RoutedEventArgs e)
        {
            Button btn = ((Button)sender);
            btn.IsEnabled = false;
            if (string.IsNullOrEmpty(txtAddedGameType.Text.Trim()) || string.IsNullOrEmpty(txtAddedGameRebate.Text.Trim()))
            {
                MessageBox.Show("请正确输入游戏数据！");
                btn.IsEnabled = true;
                return;
            }

            if (optWelfareLotteryGameType.IfExistGameType(txtAddedGameType.Text.Trim()))
            {
                MessageBox.Show("游戏类型已存在，重新输入！");
                btn.IsEnabled = true;
                return;
            }

            WelfareLotteryGameType type = new WelfareLotteryGameType
            {
                GameType = txtAddedGameType.Text,
                Rebate = txtAddedGameRebate.Text
            };

            optWelfareLotteryGameType.AddGameType(type);
            RefreshWelfareLotteryGameType();
            btn.IsEnabled = true;
        }
    }
}
