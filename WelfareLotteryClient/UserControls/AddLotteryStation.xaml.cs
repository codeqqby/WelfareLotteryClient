using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// AddLotteryStation.xaml 的交互逻辑
    /// </summary>
    public partial class AddLotteryStation : UserControl
    {
        WelfareLotteryEntities entities = new WelfareLotteryEntities();

        Dictionary<string,List<string>> manageTypeDictionary; 

        public AddLotteryStation()
        {
            InitializeComponent();
            this.clerkListBox.ItemsSource = _addedSalesclerks;

            cboAdmin.ItemsSource = entities.Administrator.ToList();
            cboAdmin.DisplayMemberPath = "AdminName";
            cboAdmin.SelectedValuePath = "Id";

            manageTypeDictionary = entities.StationManageType.ToDictionary(key=>key.TypeName,value=>JsonConvert.DeserializeObject<List<string>>(value.DetailsListSerialized));


            cmbStationManageType.ItemsSource = manageTypeDictionary.Keys;
            cmbStationManageType.SelectionChanged += CmbStationManageType_SelectionChanged;

            cmbStationRegion.ItemsSource = entities.StationRegion.ToList();

            cbxListBox.ItemsSource = entities.WelfareLotteryGameType.ToList();
        }

        private void CmbStationManageType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbStationManageTypeChild.ItemsSource = manageTypeDictionary[Convert.ToString(cmbStationManageType.SelectedValue)];
        }

        //机主照片和身份证正反 存入图片base64对应值
        Dictionary<string, string> hostBase64PicValue = new Dictionary<string, string> { {"hostPic","" }, {"Identity","" }};

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog
            {
                Title = "打开文件",
                Filter = "图片(*.jpg;*.png;*.gif;*.bmp;*.jpeg)|*.jpg;*.png;*.gif;*.bmp;*.jpeg"
            }; //定义打开文本框实体
            //对话框标题
            //文件扩展名
            if (!open.ShowDialog().GetValueOrDefault()) return;

            Utility u = new Utility();

            byte[] b = u. GetPictureData(open.FileName);

            string base64 = Convert.ToBase64String(b);

            BitmapImage myimg = u. ByteArrayToBitmapImage(Convert.FromBase64String(base64));
           
            this.HostBase64Pic.Source = myimg;
            hostBase64PicValue["hostPic"] = base64;
        }

        readonly ObservableCollection<DBModels.Salesclerk> _addedSalesclerks=new ObservableCollection<DBModels.Salesclerk>(); 

        private void btnSalesclerk_Click(object sender, RoutedEventArgs e)
        {
            AddSalesclerk salesclerk = new AddSalesclerk {Owner = Application.Current.MainWindow};

            bool? b= salesclerk.ShowDialog();
            if (!b.GetValueOrDefault()) return;
            DBModels.Salesclerk result = salesclerk.TryFindResource("InputResult") as DBModels.Salesclerk;
            _addedSalesclerks.Add(result);
        }
        
        // 网点图片
        private string _allPhotoBase64;
        //担保人照片
        private string _guaranteePicBase64;

        private void btnAllSubmit_Click(object sender, RoutedEventArgs e)
        {
            #region 网点信息

            string stationCode = txtStationCode.GetTextBoxText();
            string stationSpecificAddress = txtStationSpecificAddress.GetTextBoxText();
            string stationTarget = txtStationTarget.GetTextBoxText();
            string stationPhoneNo = txtStationPhoneNo.GetTextBoxText();

            StationRegion region= cmbStationRegion.SelectedItem as StationRegion;
            if (null==region)
            {
                "选择区域".MessageBoxDialog();
                return;
            }

            string manageType = cmbStationManageType.Text;
            string manageTypeChild = cmbStationManageTypeChild.Text;
            if (manageType.IsNullOrEmpty() || manageTypeChild.IsNullOrEmpty())
            {
                "选择销售站类型".MessageBoxDialog();
                return;
            }

            string usableArea = txtUsableArea.GetTextBoxText();
            string rentDiscount = txtRentDiscount.GetTextBoxText();
            DateTime? establishedTime = datePickerCtl.SelectedDate;
            //销售站店面产权[0:租赁 1:自有]
            bool propertyRight = Convert.ToBoolean(cmbPropertyRight.SelectedIndex);
            //站点单双机情况[0:单机 1:双机]
            bool machineTpe = Convert.ToBoolean(cmbMachineType.SelectedIndex);
            //通讯类型[0:ADSL 1:CDMA]
            bool communicationType = Convert.ToBoolean(cmbCommunicationType.SelectedIndex);
            string relatedPhoneNetNum = txtRelatedPhoneNetNum.GetTextBoxText();

            //代销证编号
            string agencyNum = txtAgencyNum.GetTextBoxText();
            string depositCardNo = txtDepositCardNo.GetTextBoxText();
            string violation = txtViolation.GetTextBoxText();

            Administrator admin = cboAdmin.SelectedItem as Administrator;
            if (null == admin)
            {
                "请选择管理员".MessageBoxDialog();
                return;
            }

            string guaranteeName = txtGuaranteeName.GetTextBoxText();
            PhotoCollection guaranteeCollection = GuaranteeIdentity.Photos;
            if (guaranteeCollection.Count != 2)
            {
                "请添加担保人身份证正反面照片！".MessageBoxDialog();
                return;
            }
            _guaranteePicBase64 =JsonConvert.SerializeObject(guaranteeCollection.ToDictionary(key => key.PhotoName,value => value.base64Value));

            #endregion

            #region 机主信息

            string hostName = txtHostName.GetTextBoxText();
            string hostPhoneNum1=txtHostPhoneNum1.GetTextBoxText();
            string hostPhoneNum2=txtHostPhoneNum2.GetTextBoxText();
            string hostIdentityNo=txtHostIdentityNo.GetTextBoxText();
            string hostIdentityAddress=txtHostIdentityAddress.GetTextBoxText();
            PhotoCollection hostPicCol = hostIdentityPic.Photos;
            if (hostPicCol.Count != 2)
            {
                "请添加机主身份证正反面照片！".MessageBoxDialog();
                return;
            }
            hostBase64PicValue["Identity"] = JsonConvert.SerializeObject(hostPicCol.ToDictionary(p => p.PhotoName, p => p.base64Value));

            #endregion
            
            PhotoCollection collection = ViewerStationPic.Photos;

            if (collection.Count < 1)
            {
                "请添加网点照片！".MessageBoxDialog();
                return;
            }
            
            ////查看是否有相同文件名的图片文件
            //if (collection.GroupBy(p=>System.IO.Path.GetFileName(p.Source)).Any(p => p.Count()>1))
            //{
            //    "请不要托入相同文件名的图片！".MessageBoxDialog();
            //    return;
            //}

            //文件名不可能重复
            var dicAllPhoto = collection.ToDictionary(photo =>photo.PhotoName, photo =>photo.base64Value);

            _allPhotoBase64 = JsonConvert.SerializeObject(dicAllPhoto);
           
            LotteryStation lottery = new LotteryStation
            {
                Administrator = admin,
                //AdminId = admin.Id,
                Salesclerk = _addedSalesclerks,
                StationCode = stationCode,
                StationPhoneNo = stationPhoneNo,
                StationSpecificAddress = stationSpecificAddress,
                StationTarget = stationTarget,
                StationRegion = region,
                //RegionId = region.Id,
                MachineType = machineTpe,
                ManageTypeName =manageType,
                ManageTypeProgencyListSerialized = manageTypeChild,//现没有多选 所以没有序列化
                StationPicListSerialized = _allPhotoBase64,
                WelfareGameTypeListSerialized = JsonConvert.SerializeObject(gameTypeSelectedCollection),

                AgencyNum = agencyNum,
                UsableArea = usableArea,
                EstablishedTime = establishedTime,
                RentDiscount = rentDiscount,
                RelatedPhoneNetNum = relatedPhoneNetNum,
                Violation = violation,
                PropertyRight = propertyRight,
                CommunicationType = communicationType,
                DepositCardNo = depositCardNo,

                HostName = hostName,
                HostIdentityNo = hostIdentityNo,
                HostPhoneNum = $"{hostPhoneNum1}⊙{hostPhoneNum2}",
                HostIdentityAddress = hostIdentityAddress,
                HostBase64Pic = hostBase64PicValue["hostPic"],
                HostBase64IdentityPic = hostBase64PicValue["Identity"],

                GuaranteeName = guaranteeName,
                GuaranteeBase64IdentityPic = _guaranteePicBase64
                
            };

            //如不用同一个上下文  这时会出现An entity object cannot be referenced by multiple instances of IEntityChangeTracker 或一个实体对象不能由多个 IEntityChangeTracker 实例引用的异常
            entities.LotteryStation.Add(lottery);
            entities.SaveChanges();
        }

        StringCollection gameTypeSelectedCollection=new StringCollection();
        
        // 游戏类型选择
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            string aa = Convert.ToString(chkZone.Tag);
            if (chkZone.IsChecked != null && (!gameTypeSelectedCollection.Contains(aa) && chkZone.IsChecked.Value))
                gameTypeSelectedCollection.Add(aa);
            else
                gameTypeSelectedCollection.Remove(aa);
        }
    }

    public class ConvertBase64ToImage:IValueConverter
    {
        readonly Utility _u=new Utility();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _u.ByteArrayToBitmapImage(System.Convert.FromBase64String((string) value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
