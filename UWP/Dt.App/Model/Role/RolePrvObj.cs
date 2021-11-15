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
    [Tbl("cm_roleprv")]
    public partial class RolePrvObj : Entity
    {
        #region 构造方法
        RolePrvObj() { }

        public RolePrvObj(
            long RoleID,
            string PrvID)
        {
            AddCell<long>("RoleID", RoleID);
            AddCell<string>("PrvID", PrvID);
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
        /// 权限标识
        /// </summary>
        public string PrvID
        {
            get { return (string)this["PrvID"]; }
            set { this["PrvID"] = value; }
        }

        new public long ID { get { return -1; } }
        #endregion
    }
    #endregion
}
