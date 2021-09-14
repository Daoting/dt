#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
#endregion

namespace Dt.App.Model
{
    #region 自动生成
    [Tbl("cm_rolemenu")]
    public partial class RoleMenuObj : Entity
    {
        #region 构造方法
        RoleMenuObj() { }

        public RoleMenuObj(
            long RoleID,
            long MenuID)
        {
            AddCell<long>("RoleID", RoleID);
            AddCell<long>("MenuID", MenuID);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["RoleID"]; }
            set { this["RoleID"] = value; }
        }

        /// <summary>
        /// 菜单标识
        /// </summary>
        public long MenuID
        {
            get { return (long)this["MenuID"]; }
            set { this["MenuID"] = value; }
        }

        new public long ID { get { return -1; } }
        #endregion
    }
    #endregion
}
