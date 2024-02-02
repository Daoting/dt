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
    public partial class SearchHistoryX
    {
        public static async Task<SearchHistoryX> New(
            string BaseUri = default,
            string Content = default)
        {
            return new SearchHistoryX(
                ID: await NewID(),
                BaseUri: BaseUri,
                Content: Content);
        }
    }
}
