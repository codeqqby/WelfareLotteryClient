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
    
    public partial class Salesclerk
    {
        public string HeadPortraitBase64Pic { get; set; }
        public int Id { get; set; }
        public string IdentityAddress { get; set; }
        public string IdentityNo { get; set; }
        public Nullable<int> LotteryStationId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    
        public virtual LotteryStation LotteryStation { get; set; }
    }
}
