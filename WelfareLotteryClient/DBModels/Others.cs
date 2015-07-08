using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;

namespace WelfareLotteryClient.DBModels
{
    /// <summary>
    /// 操作管理员
    /// </summary>
    public class OperateAdmin
    {
        //readonly LotteryStationDb _db=new LotteryStationDb();

        WelfareLotteryEntities entities=new WelfareLotteryEntities();

        /// <summary>
        /// 获取所有管理员
        /// </summary>
        /// <returns></returns>
        public List<Administrator> GetAllAdministrator()
        {
            //entities.Configuration.ProxyCreationEnabled = false;

            return entities.Administrators.OrderByDescending(p=>p.Id).ToList();
        }

        public bool IfExistAdmin(string adminName)
        {
            var re = entities.Administrators.Count(p => p.AdminName == adminName);
            return re> 0;
        }

        /// <summary>
        /// 编辑一位管理员
        /// </summary>
        /// <param name="admin"></param>
        public void EditorAdmin(Administrator admin)
        {
            var temp= entities.Administrators.FirstOrDefault(p => p.Id == admin.Id);
            if (temp == null) return;
            temp.AdminName = admin.AdminName;
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"编辑编号为【{admin.Id}】的管理员",
                OptType = (int)OptType.修改,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }

        public void DelAdmin(Administrator admin)
        {
            entities.Administrators.Remove(admin);
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"删除编号为【{admin.Id}】-名称为【{admin.AdminName}】的管理员",
                OptType = (int)OptType.删除,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="admin"></param>
        public void AddAdmin(Administrator admin)
        {
            entities.Administrators.Add(admin);
            entities.SaveChanges();
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"添加编号为【{admin.Id}】的管理员",
                OptType = (int)OptType.新增,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }
    }

    /// <summary>
    /// 操作福彩游戏类型
    /// </summary>
    public class OperateWelfareLotteryGameType
    {
        WelfareLotteryEntities entities = new WelfareLotteryEntities();
        public List<WelfareLotteryGameType> GetAllWelfareLotteryGameTypes()
        {
            return entities.WelfareLotteryGameTypes.ToList();
        }

        public void AddGameType(WelfareLotteryGameType welfareLotteryGameType)
        {
            entities.WelfareLotteryGameTypes.Add(welfareLotteryGameType);

            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.SaveChanges();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"新增编号为【{welfareLotteryGameType.Id}】的福彩游戏类型",
                OptType = (int)OptType.新增,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }

        public void EditGameType(WelfareLotteryGameType type)
        {
            WelfareLotteryGameType welfare = entities.WelfareLotteryGameTypes.FirstOrDefault(p => p.Id == type.Id);
            if (welfare == null) return;
            welfare.GameType = type.GameType;
            welfare.Rebate = type.Rebate;
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"编辑编号为【{type.Id}】的福彩游戏类型",
                OptType = (int)OptType.修改,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }

        public bool IfExistGameType(string typeName)
        {
            return entities.WelfareLotteryGameTypes.Count(p => p.GameType == typeName) > 0;
        }

        public void DelGameType(WelfareLotteryGameType type)
        {
            entities.WelfareLotteryGameTypes.Remove(type);
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"删除编号为【{type.Id}】-名称为【{type.GameType}】的福彩游戏类型",
                OptType = (int)OptType.删除,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }
    }

    /// <summary>
    /// 操作销售站类型
    /// </summary>
    public class OperateStationManageType
    {
        readonly WelfareLotteryEntities _walfareLotteryEntities=new WelfareLotteryEntities();
        public void AddManageType(List<StationManageType> types )
        {
            types.ForEach(p=> _walfareLotteryEntities.StationManageTypes.AddOrUpdate(p));
            //_walfareLotteryEntities.StationManageType.AddOrUpdate(types);
            _walfareLotteryEntities.SaveChanges();
        }

        public List<StationManageType> GetAllManageType()
        {
           return _walfareLotteryEntities.StationManageTypes.ToList();
        }

        public bool IfExistManageType(string typeName)
        {
            return _walfareLotteryEntities.StationManageTypes.Count(p => p.TypeName == typeName) > 0;
        }

        public void RemoveManageType(StationManageType type)
        {
            _walfareLotteryEntities.StationManageTypes.Remove(type);
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            _walfareLotteryEntities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"删除编号为【{type.Id}】-名称为【{type.TypeName}】的销售站类型",
                OptType = (int)OptType.删除,
                OptTime = System.DateTime.Now
            });
            _walfareLotteryEntities.SaveChanges();
        }
    }

    /// <summary>
    /// 操作体彩游戏类型
    /// </summary>
    public class OperateSportGameType
    {
        WelfareLotteryEntities entities=new WelfareLotteryEntities();
        public void AddSportGameType(SportLotteryGameType gameType)
        {
            entities.SportLotteryGameTypes.Add(gameType);
            entities.SaveChanges();
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"添加编号为【{gameType.Id}】的体彩游戏类型",
                OptType = (int)OptType.新增,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }

        public bool IfExist(string typeName)
        {
            return entities.SportLotteryGameTypes.Count(p => p.GameType == typeName) > 0;
        }

        public void DelSportGameType(SportLotteryGameType type)
        {
            entities.SportLotteryGameTypes.Remove(type);
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"删除编号为【{type.Id}】-名称为【{type.GameType}】的体彩游戏类型",
                OptType = (int)OptType.删除,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }

        public void EditSportGameType(SportLotteryGameType type)
        {
            // var en = entities.SportLotteryGameType.FirstOrDefault(p => p.Id == type.Id);

            // if (en == null) return;
            //en.GameType = type.GameType;

            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"编辑名称为【{type.Id}】的体彩游戏类型",
                OptType = (int)OptType.修改,
                OptTime = System.DateTime.Now
            });

            entities.SaveChanges();
        }

        public List<SportLotteryGameType> GetAllSportLotteryGameTypes()
        {
            return entities.SportLotteryGameTypes.ToList();
        }
    }

    /// <summary>
    /// 操作区域类型
    /// </summary>
    public class OperateRegion
    {
        //readonly LotteryStationDb _db=new LotteryStationDb();

        WelfareLotteryEntities entities = new WelfareLotteryEntities();

        /// <summary>
        /// 获取所有区域类型
        /// </summary>
        /// <returns></returns>
        public List<StationRegion> GetAllStationRegions()
        {
            return entities.StationRegions.OrderByDescending(p => p.Id).ToList();
        }

        public bool IfExistRegion(string regionName)
        {
            var re = entities.StationRegions.Count(p => p.RegionName == regionName);
            return re > 0;
        }

        /// <summary>
        /// 编辑一位区域类型
        /// </summary>
        public void EditorRegion(StationRegion region)
        {
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"编辑编号为【{region.Id}】的区域类型",
                OptType = (int)OptType.修改,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }

        public void DelRegion(StationRegion region)
        {
            entities.StationRegions.Remove(region);
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"删除编号为【{region.Id}】-名称为【{region.RegionName}】的区域类型",
                OptType = (int)OptType.删除,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }

        /// <summary>
        /// 添加区域类型
        /// </summary>
        /// <param name="region"></param>
        public void AddRegion(StationRegion region)
        {
            entities.StationRegions.Add(region);
            entities.SaveChanges();
            LoginedUserInfo u = Tools.GetLoginedUserInfo();
            entities.Logs.Add(new Log
            {
                UGuid = u.UGuid,
                Username = u.UName,
                Memo = $"添加编号为【{region.Id}】的区域类型",
                OptType = (int)OptType.新增,
                OptTime = System.DateTime.Now
            });
            entities.SaveChanges();
        }
    }

    public class AddLogs
    {
        static WelfareLotteryEntities entities = (WelfareLotteryEntities) Application.Current.Resources["WelfareLotteryEntities"];

        public static void AddLog(Log log)
        {
            entities.Logs.Add(log);
            entities.SaveChanges();
        }

    }

    public enum OptType
    {
        新增,
        修改,
        删除
    };
}
