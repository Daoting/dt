#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 收藏菜单项
    /// </summary>
    [Sqlite("state")]
    public class MenuFav : Entity
    {
        #region 构造方法
        MenuFav() { }

        public MenuFav(
            long UserID,
            long MenuID = default,
            int Clicks = default)
        {
            AddCell("UserID", UserID);
            AddCell("MenuID", MenuID);
            AddCell("Clicks", Clicks);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        /// <summary>
        /// 用户标识
        /// </summary>
        [PrimaryKey]
        public long UserID
        {
            get { return (long)this["UserID"]; }
            set { this["UserID"] = value; }
        }

        /// <summary>
        /// 菜单标识
        /// </summary>
        [PrimaryKey]
        public long MenuID
        {
            get { return (long)this["MenuID"]; }
            set { this["MenuID"] = value; }
        }

        /// <summary>
        /// 点击次数，在常用列表中排序用，可以被重置
        /// </summary>
        public int Clicks
        {
            get { return (int)this["Clicks"]; }
            set { this["Clicks"] = value; }
        }
    }
}
