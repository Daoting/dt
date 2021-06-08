#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自定义视图启动
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// 视图启动入口
        /// </summary>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">启动参数</param>
        void Run(string p_title, Icons p_icon, object p_params);
    }
}