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
    /// 用户缓存数据的版本号
    /// </summary>
    [Sqlite("state")]
    public class DataVersion : Entity
    {
        #region 构造方法
        DataVersion() { }

        public DataVersion(
            string ID,
            string Ver = default)
        {
            AddCell("ID", ID);
            AddCell("Ver", Ver);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        /// <summary>
        /// 数据类型，如菜单、权限等
        /// </summary>
        [PrimaryKey]
        new public string ID
        {
            get { return (string)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Ver
        {
            get { return (string)this["Ver"]; }
            set { this["Ver"] = value; }
        }
    }
}
