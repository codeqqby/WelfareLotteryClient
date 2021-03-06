//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WelfareLotteryClient.DBModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class LotteryStation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LotteryStation()
        {
            this.RewardCardInfoes = new HashSet<RewardCardInfo>();
            this.Salesclerks = new HashSet<Salesclerk>();
            this.StationModifiedInfoes = new HashSet<StationModifiedInfo>();
        }
    
        public Nullable<int> AdminId { get; set; }
        public string AgencyNum { get; set; }
        public bool CommunicationType { get; set; }
        public string DepositCardNo { get; set; }
        public Nullable<System.DateTime> EstablishedTime { get; set; }
        public string GuaranteeBase64IdentityPic { get; set; }
        public string GuaranteeName { get; set; }
        public string HostBase64IdentityPic { get; set; }
        public string HostBase64Pic { get; set; }
        public string HostIdentityAddress { get; set; }
        public string HostIdentityNo { get; set; }
        public string HostName { get; set; }
        public string HostPhoneNum { get; set; }
        public int Id { get; set; }
        public bool MachineType { get; set; }
        public string ManageTypeName { get; set; }
        public string ManageTypeProgencyListSerialized { get; set; }
        public bool PropertyRight { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string RelatedPhoneNetNum { get; set; }
        public string RentDiscount { get; set; }
        public Nullable<int> SportLotteryInfoId { get; set; }
        public string StationCode { get; set; }
        public string StationPhoneNo { get; set; }
        public string StationPicListSerialized { get; set; }
        public string StationSpecificAddress { get; set; }
        public string StationTarget { get; set; }
        public string UsableArea { get; set; }
        public string Violation { get; set; }
        public string WelfareGameTypeListSerialized { get; set; }
    
        public virtual Administrator Administrator { get; set; }
        public virtual SportLottery SportLottery { get; set; }
        public virtual StationRegion StationRegion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RewardCardInfo> RewardCardInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Salesclerk> Salesclerks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StationModifiedInfo> StationModifiedInfoes { get; set; }
    }
}
