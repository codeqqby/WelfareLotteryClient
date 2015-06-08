using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareLotteryClient.DBModels
{
    /// <summary>
    /// 操作管理员  游戏类型  销售站类型 ect.
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

}
