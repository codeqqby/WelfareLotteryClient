using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// SalesclerkInfo.xaml 的交互逻辑
    /// </summary>
    public partial class SalesclerkInfo : Window
    {
        public SalesclerkInfo()
        {
            InitializeComponent();
            this.Icon=new DrawingImage();
        }
        
        private WelfareLotteryEntities entities;
        private LotteryStation station;
        private ObservableCollection<DBModels.Salesclerk> collection;
        public SalesclerkInfo(WelfareLotteryEntities entities,LotteryStation lottery) : this()
        {
            this.entities = entities;
            station = lottery;
            collection = new ObservableCollection<DBModels.Salesclerk>(lottery.Salesclerk);
             clerkListBox.ItemsSource = collection;
        }

        private readonly Utility u = new Utility();

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item=sender as MenuItem;

            DBModels.Salesclerk salesclerk=clerkListBox.SelectedItem as DBModels.Salesclerk;
            
            switch (item.Header.ToString())
            {
                case "增加":
                    AddSalesclerk cler = new AddSalesclerk {Owner = this};
                    var resurnValue=cler.ShowDialog();
                    if (!resurnValue.GetValueOrDefault()) return;
                    DBModels.Salesclerk value= cler.TryFindResource("InputResult") as DBModels.Salesclerk;
                    station.Salesclerk.Add(value);
                    entities.SaveChanges();
                    collection.Add(value);
                    break;
                case "修改":
                    AddSalesclerk clerk = new AddSalesclerk(salesclerk)
                    {
                        txtName = {Text = salesclerk.Name},
                        txtPhone = {Text = salesclerk.Phone},
                        txtIdentityNo = {Text = salesclerk.IdentityNo},
                        txtIdentityAddress = {Text = salesclerk.IdentityAddress},
                        HeadPortraitBase64Pic =
                        {
                            Source =u.ByteArrayToBitmapImage(Convert.FromBase64String(salesclerk.HeadPortraitBase64Pic))
                        },
                        Owner = this
                    };

                    bool? b = clerk.ShowDialog();
                    if (!b.GetValueOrDefault()) return;
                    DBModels.Salesclerk result = clerk.FindResource("InputResult") as DBModels.Salesclerk;

                    entities.Salesclerk.AddOrUpdate(result);
                    entities.SaveChanges();

                    var aaa=collection.FirstOrDefault(p => p.Id == result.Id);
                    var index = collection.IndexOf(aaa);
                    collection.Remove(aaa);
                    collection.Insert(index,result);
                    break;
                case "删除":
                    if (MessageBox.Show("您确定要删除？","提示",MessageBoxButton.OKCancel)==MessageBoxResult.OK)
                    {
                        station.Salesclerk.Remove(salesclerk);
                        entities.SaveChanges();
                        collection.Remove(salesclerk);
                    }
                    break;
            }
            e.Handled = true;
        }
    }
}
