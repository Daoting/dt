#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 用户可访问的菜单
    /// </summary>
    [Sqlite("lob")]
    public class UserMenuX : EntityX<UserMenuX>
    {
        #region 构造方法
        UserMenuX() { }

        public UserMenuX(long ID)
        {
            Add("ID", ID);
            IsAdded = true;
        }
        #endregion

        [PrimaryKey]
        new public long ID
        {
            get { return (long)this["ID"]; }
            set { this["ID"] = value; }
        }
    }
}
