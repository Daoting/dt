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

namespace Dt.App
{
    /// <summary>
    /// 收藏菜单项
    /// </summary>
    [StateTable]
    public class MenuFav
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        [PrimaryKey]
        public string UserID { get; set; }

        /// <summary>
        /// 菜单标识
        /// </summary>
        [PrimaryKey]
        public string MenuID { get; set; }

        /// <summary>
        /// 点击次数，在常用列表中排序用，可以被重置
        /// </summary>
        public int Clicks { get; set; }
    }
}
