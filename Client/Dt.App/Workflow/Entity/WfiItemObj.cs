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
    public partial class WfiItemObj
    {
        public static async Task<WfiItemObj> Create(
            long p_atviID,
            DateTime p_date,
            bool p_isRole,
            long p_receiver,
            string p_note,
            bool p_isBack)
        {
            WfiItemObj item = new WfiItemObj(
                ID: await AtCm.NewID(),
                AtviID: p_atviID,
                AssignKind: (p_isBack ? WfiItemAssignKind.回退 : WfiItemAssignKind.普通指派),
                Status: WfiItemStatus.活动,
                IsAccept: false,
                Sender: Kit.UserName,
                Stime: p_date,
                Ctime: p_date,
                Mtime: p_date,
                Note: p_note,
                Dispidx: await AtCm.NewSeq("sq_wfi_item"));

            if (p_isRole)
                item.RoleID = p_receiver;
            else
                item.UserID = p_receiver;
            return item;
        }

        public void Finished()
        {
            Status = WfiItemStatus.结束;
            Mtime = Kit.Now;
            UserID = Kit.UserID;
        }
    }

    #region 自动生成
    [Tbl("cm_wfi_item")]
    public partial class WfiItemObj : Entity
    {
        #region 构造方法
        WfiItemObj() { }

        public WfiItemObj(
            long ID,
            long AtviID = default,
            WfiItemStatus Status = default,
            WfiItemAssignKind AssignKind = default,
            string Sender = default,
            DateTime Stime = default,
            bool IsAccept = default,
            DateTime? AcceptTime = default,
            long? RoleID = default,
            long? UserID = default,
            string Note = default,
            int Dispidx = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell("ID", ID);
            AddCell("AtviID", AtviID);
            AddCell("Status", Status);
            AddCell("AssignKind", AssignKind);
            AddCell("Sender", Sender);
            AddCell("Stime", Stime);
            AddCell("IsAccept", IsAccept);
            AddCell("AcceptTime", AcceptTime);
            AddCell("RoleID", RoleID);
            AddCell("UserID", UserID);
            AddCell("Note", Note);
            AddCell("Dispidx", Dispidx);
            AddCell("Ctime", Ctime);
            AddCell("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 活动实例标识
        /// </summary>
        public long AtviID
        {
            get { return (long)this["AtviID"]; }
            set { this["AtviID"] = value; }
        }

        /// <summary>
        /// 工作项状态 0活动 1结束 2终止 3同步活动
        /// </summary>
        public WfiItemStatus Status
        {
            get { return (WfiItemStatus)this["Status"]; }
            set { this["Status"] = value; }
        }

        /// <summary>
        /// 指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派
        /// </summary>
        public WfiItemAssignKind AssignKind
        {
            get { return (WfiItemAssignKind)this["AssignKind"]; }
            set { this["AssignKind"] = value; }
        }

        /// <summary>
        /// 发送者
        /// </summary>
        public string Sender
        {
            get { return (string)this["Sender"]; }
            set { this["Sender"] = value; }
        }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Stime
        {
            get { return (DateTime)this["Stime"]; }
            set { this["Stime"] = value; }
        }

        /// <summary>
        /// 是否签收此项任务
        /// </summary>
        public bool IsAccept
        {
            get { return (bool)this["IsAccept"]; }
            set { this["IsAccept"] = value; }
        }

        /// <summary>
        /// 签收时间
        /// </summary>
        public DateTime? AcceptTime
        {
            get { return (DateTime?)this["AcceptTime"]; }
            set { this["AcceptTime"] = value; }
        }

        /// <summary>
        /// 执行者角色标识
        /// </summary>
        public long? RoleID
        {
            get { return (long?)this["RoleID"]; }
            set { this["RoleID"] = value; }
        }

        /// <summary>
        /// 执行者用户标识
        /// </summary>
        public long? UserID
        {
            get { return (long?)this["UserID"]; }
            set { this["UserID"] = value; }
        }

        /// <summary>
        /// 工作项备注
        /// </summary>
        public string Note
        {
            get { return (string)this["Note"]; }
            set { this["Note"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
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