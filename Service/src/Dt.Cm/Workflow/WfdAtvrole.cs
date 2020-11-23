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

namespace Dt.Cm.Workflow
{
    #region 自动生成
    [Tbl("cm_wfd_atvrole")]
    public partial class WfdAtvrole : Entity
    {
        #region 构造方法
        WfdAtvrole() { }

        public WfdAtvrole(
            long AtvID,
            long RoleID)
        {
            AddCell<long>("AtvID", AtvID);
            AddCell<long>("RoleID", RoleID);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 活动标识
        /// </summary>
        public long AtvID
        {
            get { return (long)this["AtvID"]; }
            set { this["AtvID"] = value; }
        }

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["RoleID"]; }
            set { this["RoleID"] = value; }
        }

        new public long ID { get { return -1; } }
        #endregion
    }
    #endregion
}