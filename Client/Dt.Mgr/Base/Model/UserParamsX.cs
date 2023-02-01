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
    /// 用户参数
    /// </summary>
    [Sqlite("lob")]
    public class UserParamsX : EntityX<UserParamsX>
    {
        #region 构造方法
        UserParamsX() { }

        public UserParamsX(
            string ID,
            string Val = default)
        {
            AddCell("ID", ID);
            AddCell("Val", Val);
            IsAdded = true;
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
