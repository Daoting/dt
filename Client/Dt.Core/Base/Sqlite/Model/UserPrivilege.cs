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
    /// 用户具有的权限
    /// </summary>
    [Sqlite("state")]
    public class UserPrivilege : Entity
    {
        #region 构造方法
        UserPrivilege() { }

        public UserPrivilege(string Prv)
        {
            AddCell("Prv", Prv);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        /// <summary>
        /// 权限
        /// </summary>
        [PrimaryKey]
        public string Prv
        {
            get { return (string)this["Prv"]; }
            set { this["Prv"] = value; }
        }
    }
}
