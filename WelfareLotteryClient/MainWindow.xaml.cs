using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WelfareLotteryClient.DBModels;
using WelfareLotteryClient.UserControls;
using LotteryStation = WelfareLotteryClient.UserControls.LotteryStation;

namespace WelfareLotteryClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

           // FullOrMin(this);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void x_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void m_Click(object sender, RoutedEventArgs e)
        {
            FullOrMin(this);
        }

        public static void FullOrMin(Window window)
        {
            //如果是全屏,则最小化
            if (window.WindowState == WindowState.Maximized)
            {
                window.Topmost = false;
                window.WindowState = WindowState.Normal;
               // window.WindowStyle = WindowStyle.SingleBorderWindow;

               // window.ResizeMode = ResizeMode.CanResizeWithGrip;//设置为可调整窗体大小
               // window.Width = 800;
               // window.Height = 600;

                //获取窗口句柄
                var handle = new WindowInteropHelper(window).Handle;
                //获取当前显示器屏幕
                //Screen screen = Screen.FromHandle(handle);

                //window.Left = (screen.Bounds.Width - window.Width) / 2;
                //window.Top = (screen.Bounds.Height - window.Height) / 2;

                window.Top=(SystemParameters.WorkArea.Height - window.Height) / 2;                
                window.Left = (SystemParameters.WorkArea.Width - window.Width) / 2;

                return;
            }

            //如果是窗口,则全屏
            if (window.WindowState == WindowState.Normal)
            {
                //变成无边窗体
                window.WindowState = WindowState.Normal;//假如已经是Maximized，就不能进入全屏，所以这里先调整状态
                window.WindowStyle = WindowStyle.None;
                window.ResizeMode = ResizeMode.NoResize;
                window.Topmost = true;//最大化后总是在最上面

                //获取窗口句柄
                var handle = new WindowInteropHelper(window).Handle;

                //获取当前显示器屏幕
                //Screen screen = Screen.FromHandle(handle);

                ////调整窗口最大化,全屏的关键代码就是下面3句
                //window.MaxWidth = screen.Bounds.Width;
                //window.MaxHeight = screen.Bounds.Height;
                //window.WindowState = WindowState.Maximized;

                window.MaxHeight =SystemParameters.WorkArea.Height;
                window.MaxWidth =SystemParameters.WorkArea.Width;
                window.WindowState = WindowState.Maximized;

                //解决切换应用程序的问题
                window.Activated += new EventHandler(window_Activated);
                window.Deactivated += new EventHandler(window_Deactivated);
            }

        }

        static void window_Deactivated(object sender, EventArgs e)
        {
            var window = sender as Window;
            window.Topmost = false;
        }

        static void window_Activated(object sender, EventArgs e)
        {
            var window = sender as Window;
            window.Topmost = true;
        }

        private void ___Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FullOrMin(this);
        }

        private void Station_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn==null)
            {
                return;
            }

            this.StationContainer.Children.Clear();
            switch (btn.Name)
            {
                case "StationInfo":
                    this.StationContainer.Children.Add(new LotteryStation());
                    break;
                case "AddStation":                    
                    this.StationContainer.Children.Add(new AddLotteryStation());
                    break;
                case "AddGameType":

                    break;
                case "ChangeStation":

                    break;
                case "StationType":

                    break;
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show(e.Parameter.ToString());
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(e.ClickCount.ToString());
        }

        readonly OperateAdmin optAdmin =new OperateAdmin();
        readonly OperateWelfareLotteryGameType optWelfareLotteryGameType=new OperateWelfareLotteryGameType();
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var source = e.OriginalSource as TabControl;
            if (source!=null)
            {
                var temp = (TabItem)source.SelectedItem;

                switch (temp.Header.ToString())
                {
                    case "其它":
                        if (cboAllAdmin.Items.Count == 0)
                        {
                            //optAdmin.AddAdmin(new Administrator {AdminName = "小明"});
                            RefreshListview();

                            RefreshWelfareLotteryGameType();

                            RefreshSportGameTypeListView();
                        }
                        break;
                }
            }
            e.Handled = true;
        }

        #region 管理员操作

        private void btnAddAdmin_Click(object sender, RoutedEventArgs e)
        {
            Button btn = ((Button)sender);
            btn.IsEnabled = false;
            if (string.IsNullOrEmpty(txtAddedAdminName.Text.Trim()))
            {
                MessageBox.Show("请输入新增的管理员"); btn.IsEnabled = true;
                return;
            }

            if (optAdmin.IfExistAdmin(txtAddedAdminName.Text.Trim()))
            {
                MessageBox.Show("已存在，重新输入!", "提示"); btn.IsEnabled = true;
                return;
            }

            optAdmin.AddAdmin(new Administrator { AdminName = txtAddedAdminName.Text.Trim() });
            RefreshListview(); btn.IsEnabled = true;
        }

        private void RefreshListview()
        {
            cboAllAdmin.ItemsSource = optAdmin.GetAllAdministrator();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            Button btn = ((Button)sender);
            btn.IsEnabled = false;
            Administrator lvc = (Administrator)cboAllAdmin.SelectedItem;
            if (lvc == null)
            {
                MessageBox.Show("请选择需要修改的管理员", "提示");
                btn.IsEnabled = true;
                return;
            }
            if (string.IsNullOrEmpty(txtAddedAdminName.Text.Trim()))
            {
                MessageBox.Show("输入管理员名称", "提示");
                btn.IsEnabled = true;
                return;
            }

            if (optAdmin.IfExistAdmin(txtAddedAdminName.Text.Trim()))
            {
                MessageBox.Show("已存在，重新输入!", "提示"); btn.IsEnabled = true;
                return;
            }
            lvc.AdminName = txtAddedAdminName.Text.Trim();

            optAdmin.EditorAdmin(lvc);
            RefreshListview(); btn.IsEnabled = true;
        }

        private void cboAllAdmin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Administrator lvc = (Administrator)cboAllAdmin.SelectedItem;
            if (lvc != null)
            {
                txtAddedAdminName.Text = lvc.AdminName;
            }
        }

        private void btnDelAdmin_Click(object sender, RoutedEventArgs e)
        {
            Administrator lvc = (Administrator)cboAllAdmin.SelectedItem;
            if (lvc == null)
            {
                Button btn = sender as Button;
                List<Administrator> orginal = (List<Administrator>)cboAllAdmin.Items.SourceCollection;

                lvc = orginal.Find(p => p.Id == Convert.ToInt32(btn.Tag));
                if (lvc != null)
                {
                    goto perform;
                }
                MessageBox.Show("请选中您要删除的管理员");
                return;
            }

            perform:
            if (MessageBox.Show("您确定要删除？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                optAdmin.DelAdmin(lvc);
                RefreshListview();
            }

        }
        #endregion

        //刷新福彩游戏类型Listview
        private void RefreshWelfareLotteryGameType()
        {
            List<WelfareLotteryGameType> gameTypes = optWelfareLotteryGameType.GetAllWelfareLotteryGameTypes();
            lvGameType.ItemsSource = gameTypes;
        }

        private void lvGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WelfareLotteryGameType type = (WelfareLotteryGameType) lvGameType.SelectedItem;
            if (type==null)
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
                List<WelfareLotteryGameType> orginal = (List<WelfareLotteryGameType>)cboAllAdmin.Items.SourceCollection;

                lvc = orginal.Find(p => p.Id == Convert.ToInt32(btn.Tag));
                if (lvc != null)
                {
                    goto perform;
                }
                MessageBox.Show("请选中您要删除的游戏类型");
                return;
            }

            perform:
            if (MessageBox.Show("您确定要删除？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                optWelfareLotteryGameType.DelGameType(lvc);
               RefreshWelfareLotteryGameType();
            }
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

            WelfareLotteryGameType type=new WelfareLotteryGameType
            {
                GameType = txtAddedGameType.Text,
                Rebate = txtAddedGameRebate.Text
            };

            optWelfareLotteryGameType.AddGameType(type);
            RefreshWelfareLotteryGameType();
            btn.IsEnabled = true;
        }

        private void btnManageType_Click(object sender, RoutedEventArgs e)
        {
            StationManageTypeOpr stationManageType=new StationManageTypeOpr();

            stationManageType.Owner = this;

            stationManageType.ShowDialog();

        }
        #region 体彩类型操作

        readonly OperateSportGameType optSportGameType=new OperateSportGameType();

        //删除体彩类型
        private void btnDelSportGameType_Click(object sender, RoutedEventArgs e)
        {
            var lvc = lvSportGameType.SelectedItem as SportLotteryGameType;
            if (lvc == null)
            {
                Button btn = sender as Button;
                List<SportLotteryGameType> orginal = (List<SportLotteryGameType>)lvSportGameType.Items.SourceCollection;

                lvc = orginal.Find(p => p.Id == Convert.ToInt32(btn.Tag));
                if (lvc != null)
                {
                    goto perform;
                }
                MessageBox.Show("请选中您要删除的体彩类型");
                return;
            }

            perform:
            if (MessageBox.Show("您确定要删除？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                optSportGameType.DelSportGameType(lvc);
                RefreshSportGameTypeListView();
            }
        }

        //修改体彩类型
        private void btnModifySportGameType_Click(object sender, RoutedEventArgs e)
        {
            var temp = lvSportGameType.SelectedItem as SportLotteryGameType;
            if (temp == null)
            {
                MessageBox.Show("选择需要修改的体彩类型");
                return;
            }
            string inputResult = txtsportGameType.Text.Trim();
            if (string.IsNullOrEmpty(inputResult))
            {
                MessageBox.Show("请添写修改后的体彩类型");
                return;
            }

            if (optSportGameType.IfExist(inputResult))
            {
                MessageBox.Show("体彩类型已存在！");
                return;
            }

            temp.GameType = inputResult;

            optSportGameType.EditSportGameType();
            RefreshSportGameTypeListView();
        }

        //新增体彩类型
        private void btnAddSportGameType_Click(object sender, RoutedEventArgs e)
        {
            string result = txtsportGameType.Text.Trim();
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("输入体彩类型！");
                return;
            }

            if (optSportGameType.IfExist(result))
            {
                MessageBox.Show("体彩类型已存在！");
                return;
            }

            optSportGameType.AddSportGameType(new SportLotteryGameType
            {
                GameType =txtsportGameType.Text.Trim()
            });
            RefreshSportGameTypeListView();
        }

        private void RefreshSportGameTypeListView()
        {
            lvSportGameType.ItemsSource = optSportGameType.GetAllSportLotteryGameTypes();
        }

        private void lvSportGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp =lvSportGameType.SelectedItem as SportLotteryGameType;
            if (temp == null) return;
            txtsportGameType.Text = temp.GameType;
        }

        #endregion
    }
}
