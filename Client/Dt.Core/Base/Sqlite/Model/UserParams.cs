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
    /// 用户参数
    /// </summary>
    [Sqlite("state")]
    public class UserParams : Entity
    {
        #region 构造方法
        UserParams() { }

        public UserParams(
            string ID,
            string Val = default)
        {
            AddCell("ID", ID);
            AddCell("Val", Val);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        [PrimaryKey]
        new public string ID
        {
            get { return (string)this["ID"]; }
            set { this["ID"] = value; }
        }

        public string Val
        {
            get { return (string)this["Val"]; }
            set { this["Val"] = value; }
        }
    }
}
