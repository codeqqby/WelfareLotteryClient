using System;
using System.Windows;
using System.Windows.Input;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// SportLottery.xaml 的交互逻辑
    /// </summary>
    public partial class SportLottery : Window
    {
        public SportLottery()
        {
            InitializeComponent();
        }

        public SportLottery(DBModels.SportLottery lottery):this()
        {
            Sport = lottery;
            dpIncoming.SelectedDate = lottery.IncomingDate;
            cboGameType.SelectedItem = lottery.SportLotteryGameType;
            cbIsInstalled.IsChecked = lottery.IsInstall;
            txtHostName.Text = lottery.SportLotteryHostName;
            txtPhone.Text = lottery.PhoneNumber;
            txtCode.Text = lottery.LotteryCode;
            cboRelation.Text = lottery.Relation;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (null == Sport)
            {
                Sport = new DBModels.SportLottery
                {
                    IncomingDate = dpIncoming.SelectedDate,
                    SportLotteryGameType = cboGameType.SelectedItem as SportLotteryGameType,
                    IsInstall = cbIsInstalled.IsChecked.Value,
                    SportLotteryHostName = txtHostName.Text,
                    PhoneNumber = txtPhone.Text,
                    LotteryCode = txtCode.Text,
                    Relation = cboRelation.Text
                };
            }
            else
            {
                Sport.IncomingDate = dpIncoming.SelectedDate;
                Sport.SportLotteryGameType = cboGameType.SelectedItem as SportLotteryGameType;
                Sport.IsInstall = cbIsInstalled.IsChecked.Value;
                Sport.SportLotteryHostName = txtHostName.Text;
                Sport.PhoneNumber = txtPhone.Text;
                Sport.LotteryCode = txtCode.Text;
                Sport.Relation = cboRelation.Text;
            }
            DialogResult = true;
        }
        public DBModels.SportLottery Sport{ get; set; }
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtCode.GetTextBoxText().IsNullOrEmpty()) goto f;

            DateTime? incomming = dpIncoming.SelectedDate;
            if (null == incomming) goto f;

            SportLotteryGameType gameType=cboGameType.SelectedItem as SportLotteryGameType;
            if (null == gameType) goto f;

            if (txtHostName.GetTextBoxText().IsNullOrEmpty()) goto f;
            if (txtPhone.GetTextBoxText().IsNullOrEmpty()) goto f;
            if (cboRelation.Text.IsNullOrEmpty()) goto f;
            e.CanExecute = true;
            return;
            f: e.CanExecute = false;
        }
    }
}
