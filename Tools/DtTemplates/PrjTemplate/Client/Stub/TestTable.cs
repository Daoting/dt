#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
#endregion

namespace $ext_safeprojectname$
{
    /// <summary>
    /// 客户端Cookie字典
    /// </summary>
    [Sqlite("project")]
    public class TestTable : Entity
    {
        #region 构造方法
        ClientCookie() { }

        public ClientCookie(
            string Key,
            string Val = default)
        {
            AddCell("Key", Key);
            AddCell("Val", Val);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        /// <summary>
        /// 键
        /// </summary>
        [PrimaryKey]
        public string Key
        {
            get { return (string)this["Key"]; }
            set { this["Key"] = value; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public string Val
        {
            get { return (string)this["Val"]; }
            set { this["Val"] = value; }
        }
    }
}