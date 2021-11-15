#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Workflow
{
    public partial class WfiAtvObj
    {
        /// <summary>
        /// 判断当前活动是否完成，发送者是否为当前活动的最后一个发送者
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsFinished()
        {
            if (InstCount == 1)
                return true;

            int count = await AtCm.GetScalar<int>("流程-工作项个数", new { atviid = ID });
            return (count + 1) >= InstCount;
        }

        /// <summary>
        /// 结束当前活动
        /// </summary>
        public void Finished()
        {
            Status = WfiAtvStatus.结束;
            Mtime = Kit.Now;
        }

        /// <summary>
        /// 获得当前活动的回退活动
        /// </summary>
        /// <returns></returns>
        public async Task<WfiAtvObj> GetRollbackAtv()
        {
            Dict dt = new Dict();
            dt["prciid"] = PrciID;
            dt["SrcAtvID"] = AtvdID;

            var atv = await AtCm.First<WfiAtvObj>("流程-回退活动实例", dt);
            if (atv != null && atv.Status != WfiAtvStatus.同步)
            {
                // 存在同步的活动，不允许进行回退。(优先级大于设置的可以回退)
                return atv;
            }
            return null;
        }
    }

    #region 自动生成
    [Tbl("cm_wfi_atv")]
    public partial class WfiAtvObj : Entity
    {
        #region 构造方法
        WfiAtvObj() { }

        public WfiAtvObj(
            long ID,
            long PrciID = default,
            long AtvdID = default,
            WfiAtvStatus Status = default,
            int InstCount = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell("ID", ID);
            AddCell("PrciID", PrciID);
            AddCell("AtvdID", AtvdID);
            AddCell("Status", Status);
            AddCell("InstCount", InstCount);
            AddCell("Ctime", Ctime);
            AddCell("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 流程实例标识
        /// </summary>
        public long PrciID
        {
            get { return (long)this["PrciID"]; }
            set { this["PrciID"] = value; }
        }

        /// <summary>
        /// 活动模板标识
        /// </summary>
        public long AtvdID
        {
            get { return (long)this["AtvdID"]; }
            set { this["AtvdID"] = value; }
        }

        /// <summary>
        /// 活动实例的状态 0活动 1结束 2终止 3同步活动
        /// </summary>
        public WfiAtvStatus Status
        {
            get { return (WfiAtvStatus)this["Status"]; }
            set { this["Status"] = value; }
        }

        /// <summary>
        /// 活动实例在流程实例被实例化的次数
        /// </summary>
        public int InstCount
        {
            get { return (int)this["InstCount"]; }
            set { this["InstCount"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }

        /// <summary>
        /// 最后一次状态改变的时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["Mtime"]; }
            set { this["Mtime"] = value; }
        }
        #endregion
    }
    #endregion
}