using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Newtonsoft.Json;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// AddLotteryStation.xaml 的交互逻辑
    /// </summary>
    public partial class AddLotteryStation : UserControl
    {
        WelfareLotteryEntities entities;

        Dictionary<string,List<string>> manageTypeDictionary; 

        public AddLotteryStation()
        {
            InitializeComponent();

            entities = (WelfareLotteryEntities)Application.Current.Resources["WelfareLotteryEntities"];

            this.clerkListBox.ItemsSource = _addedSalesclerks;

            cboAdmin.ItemsSource = entities.Administrators.ToList();
            cboAdmin.DisplayMemberPath = "AdminName";
            cboAdmin.SelectedValuePath = "Id";

            manageTypeDictionary = entities.StationManageTypes.ToDictionary(key=>key.TypeName,value=>JsonConvert.DeserializeObject<List<string>>(value.DetailsListSerialized));


            cmbStationManageType.ItemsSource = manageTypeDictionary.Keys;
            cmbStationManageType.SelectionChanged += CmbStationManageType_SelectionChanged;

            cmbStationRegion.ItemsSource = entities.StationRegions.ToList();

            cbxListBox.lvGameType.ItemsSource = entities.WelfareLotteryGameTypes.ToList();
        }

        private void CmbStationManageType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbStationManageTypeChild.ItemsSource = manageTypeDictionary[Convert.ToString(cmbStationManageType.SelectedValue)];
        }

        readonly ObservableCollection<Salesclerk> _addedSalesclerks=new ObservableCollection<Salesclerk>(); 

        private void btnSalesclerk_Click(object sender, RoutedEventArgs e)
        {
            AddSalesclerk salesclerk = new AddSalesclerk {Owner = Application.Current.MainWindow};

            bool? b= salesclerk.ShowDialog();
            if (!b.GetValueOrDefault()) return;
            Salesclerk result = salesclerk.TryFindResource("InputResult") as Salesclerk;
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

            if (stationCode.IsNullOrEmpty())
            {
                "输入网点编号".MessageBoxDialog();
                return;
            }
            if (stationSpecificAddress.IsNullOrEmpty())
            {
                "输入销售站详细地址".MessageBoxDialog();
                return;
            }
            if (stationTarget.IsNullOrEmpty())
            {
                "输入标的物".MessageBoxDialog();
                return;
            }
            if (stationPhoneNo.IsNullOrEmpty())
            {
                "输入网点电话".MessageBoxDialog();
                return;
            }

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
            if (usableArea.IsNullOrEmpty())
            {
                "输入店面经营使用面积".MessageBoxDialog();
                return;
            }
            string rentDiscount = txtRentDiscount.GetTextBoxText();
            if (usableArea.IsNullOrEmpty())
            {
                "输入租金折价".MessageBoxDialog();
                return;
            }
            DateTime? establishedTime = datePickerCtl.SelectedDate;
            if (!establishedTime.HasValue)
            {
                "选择设立日期".MessageBoxDialog();
                return;
            }
            //销售站店面产权[0:租赁 1:自有]
            bool propertyRight = Convert.ToBoolean(cmbPropertyRight.SelectedIndex);
            //站点单双机情况[0:单机 1:双机]
            bool machineTpe = Convert.ToBoolean(cmbMachineType.SelectedIndex);
            //通讯类型[0:ADSL 1:CDMA]
            bool communicationType = Convert.ToBoolean(cmbCommunicationType.SelectedIndex);
            string relatedPhoneNetNum = txtRelatedPhoneNetNum.GetTextBoxText();
            if (relatedPhoneNetNum.IsNullOrEmpty())
            {
                "输入关联电话网络号".MessageBoxDialog();
                return;
            }
            //代销证编号
            string agencyNum = txtAgencyNum.GetTextBoxText();
            if (agencyNum.IsNullOrEmpty())
            {
                "输入代销证编号".MessageBoxDialog();
                return;
            }
            string depositCardNo = txtDepositCardNo.GetTextBoxText();
            if (depositCardNo.IsNullOrEmpty())
            {
                "输入交款卡卡号".MessageBoxDialog();
                return;
            }
            string violation = txtViolation.GetTextBoxText();//可不添

            Administrator admin = cboAdmin.SelectedItem as Administrator;
            if (null == admin)
            {
                "请选择管理员".MessageBoxDialog();
                return;
            }

            string guaranteeName = txtGuaranteeName.GetTextBoxText();
            if (guaranteeName.IsNullOrEmpty())
            {
                "输入担保人姓名".MessageBoxDialog();
                return;
            }
            PhotoCollection guaranteeCollection = GuaranteeIdentity.Photos;
            if (guaranteeCollection.Count != 2)
            {
                "请添加担保人身份证正反面照片！".MessageBoxDialog();
                return;
            }
            _guaranteePicBase64 =JsonConvert.SerializeObject(guaranteeCollection.ToDictionary(key => key.PhotoName,value => value.base64Value));

            #endregion

            #region 机主信息

            string hostName = HostInformation.txtHostName.GetTextBoxText();
            string hostPhoneNum1= HostInformation.txtHostPhoneNum1.GetTextBoxText();
            string hostPhoneNum2= HostInformation.txtHostPhoneNum2.GetTextBoxText();
            string hostIdentityNo= HostInformation.txtHostIdentityNo.GetTextBoxText();
            string hostIdentityAddress= HostInformation.txtHostIdentityAddress.GetTextBoxText();

            if (hostName.IsNullOrEmpty() || hostPhoneNum1.IsNullOrEmpty() || hostPhoneNum2.IsNullOrEmpty() || hostIdentityNo.IsNullOrEmpty() || hostIdentityAddress.IsNullOrEmpty())
            {
                "添写机主的完整信息".MessageBoxDialog();
                return;
            }

           // PhotoCollection hostPicCol = HostInformation.hostIdentityPic.Photos;
            if (HostInformation.DropImageCount != 2)
            {
                "请添加机主身份证正反面照片！".MessageBoxDialog();
                return;
            }
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
                Salesclerks = _addedSalesclerks,
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
                WelfareGameTypeListSerialized = JsonConvert.SerializeObject(cbxListBox.GameTypeSelectedCollection),

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
                HostBase64Pic = HostInformation.HostPic,
                HostBase64IdentityPic =HostInformation.Identity,

                GuaranteeName = guaranteeName,
                GuaranteeBase64IdentityPic = _guaranteePicBase64
                
            };

            //如不用同一个上下文  这时会出现An entity object cannot be referenced by multiple instances of IEntityChangeTracker 或一个实体对象不能由多个 IEntityChangeTracker 实例引用的异常
            entities.LotteryStations.Add(lottery);
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"增加网点编号为【{stationCode}】的网点",
                OptType =(int)OptType.新增,
                OptTime = DateTime.Now
            });
            entities.SaveChanges();
            "录入成功".MessageBoxDialog();
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
