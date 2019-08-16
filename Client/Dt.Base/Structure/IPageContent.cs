#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// PhoneUI页面内容的接口定义
    /// </summary>
    public interface IPageContent
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 是否显示顶部导航栏
        /// </summary>
        bool ShowTopbar { get; set; }
    }
}
