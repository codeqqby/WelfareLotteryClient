using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

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

            return entities.Administrator.OrderByDescending(p=>p.Id).ToList();
        }

        public bool IfExistAdmin(string adminName)
        {
            var re = entities.Administrator.Count(p => p.AdminName == adminName);
            return re> 0;
        }

        /// <summary>
        /// 编辑一位管理员
        /// </summary>
        /// <param name="admin"></param>
        public void EditorAdmin(Administrator admin)
        {
            var temp= entities.Administrator.FirstOrDefault(p => p.Id == admin.Id);
            if (temp == null) return;
            temp.AdminName = admin.AdminName;
            entities.SaveChanges();
        }

        public void DelAdmin(Administrator admin)
        {
            entities.Administrator.Remove(admin);
            entities.SaveChanges();
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="admin"></param>
        public void AddAdmin(Administrator admin)
        {
            entities.Administrator.Add(admin);
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
            return entities.WelfareLotteryGameType.ToList();
        }

        public void AddGameType(WelfareLotteryGameType welfareLotteryGameType)
        {
            entities.WelfareLotteryGameType.Add(welfareLotteryGameType);
            entities.SaveChanges();
        }

        public void EditGameType(WelfareLotteryGameType type)
        {
            WelfareLotteryGameType welfare = entities.WelfareLotteryGameType.FirstOrDefault(p => p.Id == type.Id);
            if (welfare == null) return;
            welfare.GameType = type.GameType;
            welfare.Rebate = type.Rebate;

            entities.SaveChanges();
        }

        public bool IfExistGameType(string typeName)
        {
            return entities.WelfareLotteryGameType.Count(p => p.GameType == typeName) > 0;
        }

        public void DelGameType(WelfareLotteryGameType type)
        {
            entities.WelfareLotteryGameType.Remove(type);
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
            types.ForEach(p=> _walfareLotteryEntities.StationManageType.AddOrUpdate(p));
            //_walfareLotteryEntities.StationManageType.AddOrUpdate(types);
            _walfareLotteryEntities.SaveChanges();
        }

        public List<StationManageType> GetAllManageType()
        {
           return _walfareLotteryEntities.StationManageType.ToList();
        }

        public bool IfExistManageType(string typeName)
        {
            return _walfareLotteryEntities.StationManageType.Count(p => p.TypeName == typeName) > 0;
        }

        public void RemoveManageType(StationManageType type)
        {
            _walfareLotteryEntities.StationManageType.Remove(type);
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
            entities.SportLotteryGameType.Add(gameType);
            entities.SaveChanges();
        }

        public bool IfExist(string typeName)
        {
            return entities.SportLotteryGameType.Count(p => p.GameType == typeName) > 0;
        }

        public void DelSportGameType(SportLotteryGameType type)
        {
            entities.SportLotteryGameType.Remove(type);
            entities.SaveChanges();
        }

        public void EditSportGameType()
        {
           // var en = entities.SportLotteryGameType.FirstOrDefault(p => p.Id == type.Id);

           // if (en == null) return;
            //en.GameType = type.GameType;
            entities.SaveChanges();
        }

        public List<SportLotteryGameType> GetAllSportLotteryGameTypes()
        {
            return entities.SportLotteryGameType.ToList();
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
            return entities.StationRegion.OrderByDescending(p => p.Id).ToList();
        }

        public bool IfExistRegion(string regionName)
        {
            var re = entities.StationRegion.Count(p => p.RegionName == regionName);
            return re > 0;
        }

        /// <summary>
        /// 编辑一位区域类型
        /// </summary>
        public void EditorRegion()
        {
            entities.SaveChanges();
        }

        public void DelRegion(StationRegion region)
        {
            entities.StationRegion.Remove(region);
            entities.SaveChanges();
        }

        /// <summary>
        /// 添加区域类型
        /// </summary>
        /// <param name="region"></param>
        public void AddRegion(StationRegion region)
        {
            entities.StationRegion.Add(region);
            entities.SaveChanges();
        }
    }
}
