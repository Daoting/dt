#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 主页窗口支持开始菜单的接口
    /// </summary>
    public interface IStartMenu
    {
        /// <summary>
        /// 显示开始菜单
        /// </summary>
        void ShowStartMenu();
    }
}