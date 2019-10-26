#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
#endregion

namespace Dt.Core.Model
{
    /// <summary>
    /// 角色关联的菜单
    /// </summary>
    [Table]
    public class RoleMenu
    {
        /// <summary>
        /// 角色标识
        /// </summary>
        [Indexed]
        public long RoleID { get; set; }

        /// <summary>
        /// 菜单标识
        /// </summary>
        public long MenuID { get; set; }
    }
}
