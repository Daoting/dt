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

namespace Dt.Base.FormView
{
    /// <summary>
    /// 查询面板历史
    /// </summary>
    [StateTable]
    public class SearchFvHis
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// 控件所属的xaml位置
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// 搜索内容
        /// </summary>
        public string Content { get; set; }
    }
}
