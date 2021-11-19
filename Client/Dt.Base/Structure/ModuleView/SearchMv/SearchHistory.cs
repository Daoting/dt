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

namespace Dt.Base.ModuleView
{
    /// <summary>
    /// 查询面板历史
    /// </summary>
    [Sqlite("state")]
    public class SearchHistory : Entity
    {
        #region 构造方法
        SearchHistory() { }

        public SearchHistory(
            string BaseUri = default,
            string Content = default)
        {
            AddCell("ID", 0);
            AddCell("BaseUri", BaseUri);
            AddCell("Content", Content);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        new public int ID
        {
            get { return (int)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 控件所属的xaml位置
        /// </summary>
        public string BaseUri
        {
            get { return (string)this["BaseUri"]; }
            set { this["BaseUri"] = value; }
        }

        /// <summary>
        /// 搜索内容
        /// </summary>
        public string Content
        {
            get { return (string)this["Content"]; }
            set { this["Content"] = value; }
        }
    }
}
