#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 用户具有的权限
    /// </summary>
    [Sqlite("lob")]
    public class UserPermissionX : EntityX<UserPermissionX>
    {
        #region 构造方法
        UserPermissionX() { }

        public UserPermissionX(long ID)
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
