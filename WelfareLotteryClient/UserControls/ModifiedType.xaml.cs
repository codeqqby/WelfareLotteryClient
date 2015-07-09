using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// ModifiedType.xaml 的交互逻辑
    /// </summary>
    public partial class ModifiedType : Window
    {
        private WelfareLotteryEntities entities;
        public ModifiedType()
        {
            InitializeComponent();

            entities = (WelfareLotteryEntities)Application.Current.Resources["WelfareLotteryEntities"];

            lvModifiedTypes=new ObservableCollection<StationModifiedType>(entities.StationModifiedTypes.ToList());

            cboAllTypes.ItemsSource = lvModifiedTypes;
        }

        private ObservableCollection<StationModifiedType> lvModifiedTypes; 

        private void cboAllTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StationModifiedType type = cboAllTypes.SelectedItem as StationModifiedType;
            txtAddedInfo.Text = type?.TypeName;
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            string info;
            if (VerifyInput(out info,true)) return;

            StationModifiedType type = cboAllTypes.SelectedItem as StationModifiedType;
            int index= lvModifiedTypes.IndexOf(type);
            type.TypeName = info;
            LoginedUserInfo us = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = us.UGuid,
                Username = us.UName,
                Memo = $"编辑编号为【{type.Id}】的网点变更类型",
                OptType = (int)OptType.修改,
                OptTime = DateTime.Now
            });
            entities.SaveChanges();
            lvModifiedTypes.Remove(type);
            lvModifiedTypes.Insert(index,type);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string info;
            if (VerifyInput(out info)) return;

            StationModifiedType type=new StationModifiedType {TypeName = info};

            entities.StationModifiedTypes.Add(type);
            entities.SaveChanges();
            LoginedUserInfo us = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = us.UGuid,
                Username = us.UName,
                Memo = $"新增编号为【{type.Id}】的网点变更类型",//这样貌没有用
                OptType = (int)OptType.新增,
                OptTime = DateTime.Now
            });
            entities.SaveChanges();
            lvModifiedTypes.Add(type);
        }

        private bool VerifyInput(out string info,bool edit=false)
        {
            if (edit)
            {
                StationModifiedType type= cboAllTypes.SelectedItem as StationModifiedType;
                if (null == type)
                {
                    "选择您要编辑的数据行".MessageBoxDialog();
                    info = string.Empty;
                    return true;
                }
            }
            info = txtAddedInfo.GetTextBoxText();
            if (info.IsNullOrEmpty())
            {
                "输入变更类型".MessageBoxDialog();
                return true;
            }

            string s = info;
            if (entities.StationModifiedTypes.Count(p => p.TypeName == s) > 0)
            {
                "已经存在此变更类型！".MessageBoxDialog();
                return true;
            }
            return false;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            StationModifiedType lvc = (StationModifiedType)cboAllTypes.SelectedItem;
            if (lvc == null)
            {
                Button btn = sender as Button;
                lvc = lvModifiedTypes.SingleOrDefault(p => p.Id == Convert.ToInt32(btn.Tag));
                if (lvc != null)
                {
                    goto perform;
                }
                "请选中您要删除的类型".MessageBoxDialog();
                return;
            }

            perform:
            if (MessageBox.Show("您确定要删除？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                entities.StationModifiedTypes.Remove(lvc);
                LoginedUserInfo us = Tools.GetLoginedUserInfo();
                entities.Logs.Add(new Log
                {
                    UGuid = us.UGuid,
                    Username = us.UName,
                    Memo = $"删除编号为【{lvc.Id}】-名称为【{lvc.TypeName}】的网点变更类型",//这样貌没有用
                    OptType = (int)OptType.删除,
                    OptTime = DateTime.Now
                });
                entities.SaveChanges();
                lvModifiedTypes.Remove(lvc);
            }
        }
    }
}
