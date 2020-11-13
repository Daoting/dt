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
#endregion

namespace Dt.App.Workflow
{
    #region 自动生成
    [Tbl("cm_wfd_trs")]
    public partial class WfdTrs : Entity
    {
        #region 构造方法
        WfdTrs() { }

        public WfdTrs(
            long ID,
            long PrcID = default,
            long SrcAtvID = default,
            long TgtAtvID = default,
            byte Type = default,
            long? TrsID = default)
        {
            AddCell<long>("ID", ID);
            AddCell<long>("PrcID", PrcID);
            AddCell<long>("SrcAtvID", SrcAtvID);
            AddCell<long>("TgtAtvID", TgtAtvID);
            AddCell<byte>("Type", Type);
            AddCell<long?>("TrsID", TrsID);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 流程模板标识
        /// </summary>
        public long PrcID
        {
            get { return (long)this["PrcID"]; }
            set { this["PrcID"] = value; }
        }

        /// <summary>
        /// 起始活动模板标识
        /// </summary>
        public long SrcAtvID
        {
            get { return (long)this["SrcAtvID"]; }
            set { this["SrcAtvID"] = value; }
        }

        /// <summary>
        /// 目标活动模板标识
        /// </summary>
        public long TgtAtvID
        {
            get { return (long)this["TgtAtvID"]; }
            set { this["TgtAtvID"] = value; }
        }

        /// <summary>
        /// 迁移类别 0:常规迁移 1:回退迁移
        /// </summary>
        public byte Type
        {
            get { return (byte)this["Type"]; }
            set { this["Type"] = value; }
        }

        /// <summary>
        /// 类别为回退迁移时对应的常规迁移标识
        /// </summary>
        public long? TrsID
        {
            get { return (long?)this["TrsID"]; }
            set { this["TrsID"] = value; }
        }
        #endregion

        #region 可复制
        /*
        void OnSaving()
        {
        }

        void OnDeleting()
        {
        }

        void SetID(long p_value)
        {
        }

        void SetPrcID(long p_value)
        {
        }

        void SetSrcAtvID(long p_value)
        {
        }

        void SetTgtAtvID(long p_value)
        {
        }

        void SetType(byte p_value)
        {
        }

        void SetTrsID(long? p_value)
        {
        }
        */
        #endregion
    }
    #endregion
}