#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 查询面板历史
    /// </summary>
    [Sqlite("state")]
    public partial class SearchHistoryX : EntityX<SearchHistoryX>
    {
        #region 构造方法
        SearchHistoryX() { }

        public SearchHistoryX(
            long ID,
            string BaseUri = default,
            string Content = default)
        {
            Add("ID", ID);
            Add("BaseUri", BaseUri);
            Add("Content", Content);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey]
        new public long ID
        {
            get { return (long)this["ID"]; }
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
