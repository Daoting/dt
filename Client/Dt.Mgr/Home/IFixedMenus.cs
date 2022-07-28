#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-07-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Model;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 默认主页的固定菜单项
    /// </summary>
    public interface IFixedMenus
    {
        /// <summary>
        /// 获取默认主页(DefaultHome)的固定菜单项
        /// </summary>
        IList<OmMenu> GetFixedMenus();
    }
}
