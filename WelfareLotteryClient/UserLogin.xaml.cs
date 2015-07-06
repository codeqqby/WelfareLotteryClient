using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Newtonsoft.Json;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient
{
    /// <summary>
    /// UserLogin.xaml 的交互逻辑
    /// </summary>
    public partial class UserLogin : Window
    {
        public UserLogin()
        {
            InitializeComponent();
        }

        readonly UserLoginInfo info=new UserLoginInfo();

        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (!btn_login.IsEnabled) return;
            btn_login.IsEnabled = false;
            string name = txtUserName.GetTextBoxText();
            string pass = txtPassword.Password;

            if (name.IsNullOrEmpty() || pass.IsNullOrEmpty())
            {
                "用户名和密码不能为空".MessageBoxDialog();
                btn_login.IsEnabled = true;
                return;
            }

            AspNetUser user = new AspNetUser
            {
                UserName = name,
                PasswordHash = pass
            };
            try
            {
                info.IsAdmitted(user, ((o, args) =>
                {
                    if (args.Error != null)
                    {
                        args.Error.Message.MessageBoxDialog();
                        btn_login.IsEnabled = true;
                        return;
                    }

                    WebClient client = o as WebClient;
                    var result =JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(args.Result));
                    string msg = result["Errors"];
                    if (!msg.IsNullOrEmpty())
                    {
                        msg.MessageBoxDialog();
                    }
                    else
                    {
                        string r = result["Succeeded"];
                        if (null != r && "true" == r)
                        {
                            MainWindow main = new MainWindow();
                            Application.Current.MainWindow = main;

                            Application.Current.MainWindow.Tag = new LoginedUserInfo
                            {
                                RoleName = result["Role"],
                                Permitted = result["Role"] != "Member",//如为true则有权限进行编辑
                                UGuid = result["UGuid"],
                                UName = name
                            };
                            main.Show();
                            this.Close();
                        }
                        else
                        {
                            "密码错误".MessageBoxDialog();
                        }
                    }
                    client.Dispose();
                    btn_login.IsEnabled = true;
                }));
            }
            catch (Exception ex)
            {
                ex.Message.MessageBoxDialog();
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
        private void btn_login_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn_login = (Button)sender;
            Label lb1 = (Label)btn_login.Template.FindName("tips_for_login", btn_login);
            lb1.Foreground = new SolidColorBrush(Colors.Red);

        }

        private void btn_login_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn_login = (Button)sender;
            Label lb1 = (Label)btn_login.Template.FindName("tips_for_login", btn_login);
            lb1.Foreground = new SolidColorBrush(Colors.White);

        }
        private void tips_close_MouseEnter(object sender, MouseEventArgs e)
        {
            Label lb_1 = (Label)sender;
            try
            {
                ImageBrush ib1 = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "ResourceLibary/Images/cancel_1.png")));

                ib1.Stretch = Stretch.Fill;


                lb_1.Background = ib1;
            }
            catch (Exception ef)
            {
                MessageBox.Show("出现错误！：" + ef.ToString());
            }

        }

        private void tips_close_MouseLeave(object sender, MouseEventArgs e)
        {
            Label lb_1 = (Label)sender;
            try
            {
                ImageBrush ib1 = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "ResourceLibary/Images/cancel.png")));

                ib1.Stretch = Stretch.Fill;


                lb_1.Background = ib1;
            }
            catch (Exception ef)
            {
                MessageBox.Show("出现错误！：" + ef.ToString());
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btn_Register_Click(object sender, RoutedEventArgs e)
        {
            try { Process.Start(ConfigurationManager.AppSettings["registerUrl"]); } catch { }
        }
    }
}
