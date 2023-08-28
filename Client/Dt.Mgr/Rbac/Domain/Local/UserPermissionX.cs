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

        public UserPermissionX(string Name)
        {
            Add("Name", Name);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 权限名称
        /// </summary>
        [PrimaryKey]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }
    }
}
