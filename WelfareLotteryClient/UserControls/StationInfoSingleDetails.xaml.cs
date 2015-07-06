using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// StationInfoSingleDetails.xaml 的交互逻辑
    /// </summary>
    public partial class StationInfoSingleDetails : Window
    {
        Dictionary<string, List<string>> manageTypeDictionary;
        private WelfareLotteryEntities entities;
        private LotteryStation station;
        public StationInfoSingleDetails(LotteryStation station)
        {
            InitializeComponent();

            entities = (WelfareLotteryEntities)Application.Current.Resources["WelfareLotteryEntities"];

            this.station = station;

            cmbStationRegion.ItemsSource = entities.StationRegions.ToList();

            manageTypeDictionary = entities.StationManageTypes.ToDictionary(key => key.TypeName, value => JsonConvert.DeserializeObject<List<string>>(value.DetailsListSerialized));
            cmbStationManageType.ItemsSource = manageTypeDictionary.Keys;
            cmbStationManageType.SelectionChanged += CmbStationManageType_SelectionChanged;
            cmbStationRegion.ItemsSource = entities.StationRegions.ToList();

            cboAdmin.ItemsSource = entities.Administrators.ToList();

            cbxListBox.lvGameType.ItemsSource = entities.WelfareLotteryGameTypes.ToList();

            InitControlValue();

        }

        private void CmbStationManageType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbStationManageTypeChild.ItemsSource = manageTypeDictionary[Convert.ToString(cmbStationManageType.SelectedValue)];
        }
        private void InitControlValue()
        {
            txtStationCode.Text = station.StationCode;
            txtStationSpecificAddress.Text= station.StationSpecificAddress;
            txtStationTarget.Text = station.StationTarget;
            txtStationPhoneNo.Text= station.StationPhoneNo;
            cmbStationRegion.SelectedItem = station.StationRegion;
            cmbStationManageType.Text = station.ManageTypeName;
            cmbStationManageTypeChild.Text = station.ManageTypeProgencyListSerialized;
            txtUsableArea.Text = station.UsableArea;
            txtRentDiscount.Text = station.RentDiscount;
            datePickerCtl.SelectedDate = station.EstablishedTime;
            cmbPropertyRight.Text = station.PropertyRight ? "自有" : "租赁";
            cmbMachineType.Text=station.MachineType ? "双机" : "单机";
            cmbCommunicationType.Text=station.CommunicationType ? "CDMA" : "ADSL";
            cboAdmin.SelectedItem = station.Administrator;

            var aaaa = JsonConvert.DeserializeObject<string[]>(station.WelfareGameTypeListSerialized);
            cbxListBox.InitSelectedCollection(aaaa);

            txtRelatedPhoneNetNum.Text = station.RelatedPhoneNetNum;
            txtAgencyNum.Text = station.AgencyNum;
            txtDepositCardNo.Text = station.DepositCardNo;
            txtViolation.Text = station.Violation;
            txtViolation.ToolTip = station.Violation;
            txtGuaranteeName.Text = station.GuaranteeName;

            Utility u = new Utility();

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(station.GuaranteeBase64IdentityPic);
            PhotoCollection photo = GuaranteeIdentity.Photos;
            result.Keys.ToList().ForEach(k => photo.Add(new Photo(k) { Image = u.ByteArrayToBitmapImage(Convert.FromBase64String(result[k])) }));

            HostInformation.txtHostName.Text = station.HostName;
            HostInformation.txtHostIdentityNo.Text = station.HostIdentityNo;
            HostInformation.txtHostIdentityAddress.Text = station.HostIdentityAddress;
            string[] phoneNum = station.HostPhoneNum.Split('⊙');
            HostInformation.txtHostPhoneNum1.Text = phoneNum[0];
            HostInformation.txtHostPhoneNum2.Text = phoneNum[1];
            HostInformation.HostBase64Pic.Source =u.ByteArrayToBitmapImage(Convert.FromBase64String(station.HostBase64Pic));
            var hostResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(station.HostBase64IdentityPic);
            PhotoCollection hostCollection = HostInformation.hostIdentityPic.Photos;
            hostResult.Keys.ToList().ForEach(k => hostCollection.Add(new Photo(k) { Image = u.ByteArrayToBitmapImage(Convert.FromBase64String(hostResult[k])) }));

            clerkListBox.ItemsSource = station.Salesclerks.ToList();

            var stationPic= JsonConvert.DeserializeObject<Dictionary<string, string>>(station.StationPicListSerialized);
            PhotoCollection photoCollection = ViewerStationPic.Photos;
            stationPic.Keys.ToList().ForEach(p=>photoCollection.Add(new Photo(p) {Image = u.ByteArrayToBitmapImage(Convert.FromBase64String(stationPic[p]))}));


        }

        public void IsEnabledControl()
        {
            gridStation.Children.OfType<TextBox>().ToList().ForEach(p=>p.IsEnabled= Tools.LoginUserHasRights());
            txtViolation.IsEnabled = true;
            gridStation.Children.OfType<ComboBox>().ToList().ForEach(p => p.IsEnabled = Tools.LoginUserHasRights());
            GuaranteeIdentity.ImageAllowDrop = Tools.LoginUserHasRights();

            HostInformation.EnableAllControl();
            
            clerkListBox.IsEnabled = Tools.LoginUserHasRights();

            ViewerStationPic.ImageAllowDrop = Tools.LoginUserHasRights();
        }

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

            StationRegion region = cmbStationRegion.SelectedItem as StationRegion;
            if (null == region)
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
            string guaranteePicBase64 = GuaranteeIdentity.IsChanged?JsonConvert.SerializeObject(guaranteeCollection.ToDictionary(key => key.PhotoName, value => value.base64Value)):null;

            #endregion

            #region 机主信息

            string hostName = HostInformation.txtHostName.GetTextBoxText();
            string hostPhoneNum1 = HostInformation.txtHostPhoneNum1.GetTextBoxText();
            string hostPhoneNum2 = HostInformation.txtHostPhoneNum2.GetTextBoxText();
            string hostIdentityNo = HostInformation.txtHostIdentityNo.GetTextBoxText();
            string hostIdentityAddress = HostInformation.txtHostIdentityAddress.GetTextBoxText();

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

            string allPhotoBase64 = ViewerStationPic.IsChanged?JsonConvert.SerializeObject(collection.ToDictionary(photo => photo.PhotoName, photo => photo.base64Value)):null;

            station.Administrator = admin;
              
               // Salesclerks = _addedSalesclerks,
                station.StationCode = stationCode;
                station.StationPhoneNo = stationPhoneNo;
                station.StationSpecificAddress = stationSpecificAddress;
                station.StationTarget = stationTarget;
                station.StationRegion = region;
                //RegionId = region.Id;
                station.MachineType = machineTpe;
                station.ManageTypeName = manageType;
                station.ManageTypeProgencyListSerialized = manageTypeChild;//现没有多选 所以没有序列化
                if(!allPhotoBase64.IsNullOrEmpty())station.StationPicListSerialized = allPhotoBase64;
                station.WelfareGameTypeListSerialized = JsonConvert.SerializeObject(cbxListBox.GameTypeSelectedCollection);
                
                station.AgencyNum = agencyNum;
                station.UsableArea = usableArea;
                station.EstablishedTime = establishedTime;
                station.RentDiscount = rentDiscount;
                station.RelatedPhoneNetNum = relatedPhoneNetNum;
                station.Violation = violation;
                station.PropertyRight = propertyRight;
                station.CommunicationType = communicationType;
                station.DepositCardNo = depositCardNo;
                station.HostName = hostName;
                station.HostIdentityNo = hostIdentityNo;
                station.HostPhoneNum = $"{hostPhoneNum1}⊙{hostPhoneNum2}";
                station.HostIdentityAddress = hostIdentityAddress;
                if(!HostInformation.HostPic.IsNullOrEmpty()) station.HostBase64Pic = HostInformation.HostPic;
            if (HostInformation.hostIdentityPic.IsChanged) station.HostBase64IdentityPic = HostInformation.Identity;
                station.GuaranteeName = guaranteeName;
                if(!guaranteePicBase64.IsNullOrEmpty()) station.GuaranteeBase64IdentityPic = guaranteePicBase64;

            //如不用同一个上下文  这时会出现An entity object cannot be referenced by multiple instances of IEntityChangeTracker 或一个实体对象不能由多个 IEntityChangeTracker 实例引用的异常
            entities.LotteryStations.AddOrUpdate(station);
            entities.SaveChanges();
            "更新成功".MessageBoxDialog();
        }
    }
}
