#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 支持内部导航的内容接口
    /// </summary>
    public interface INaviContent
    {
        /// <summary>
        /// 将内容添加到宿主容器
        /// </summary>
        /// <param name="p_host">宿主容器</param>
        void AddToHost(INaviHost p_host);
    }
}
