#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// 自定义视图启动
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// 视图启动入口
        /// </summary>
        /// <param name="p_obj">启动参数，如菜单</param>
        void Run(object p_obj);
    }
}