#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022/8/4 13:04:15 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.UIDemo
{
    /// <summary>
    /// 本地字典表
    /// </summary>
    [Sqlite("local")]
    public class LocalDict : Entity
    {
        #region 构造方法
        LocalDict() { }

        public LocalDict(
            string Key,
            string Val = default)
        {
            AddCell("Key", Key);
            AddCell("Val", Val);
            IsAdded = true;
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