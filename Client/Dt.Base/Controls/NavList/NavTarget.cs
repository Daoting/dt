#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-10-18 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 加载内容的目标位置
    /// </summary>
    public enum NavTarget
    {
        /// <summary>
        /// 在当前Win的主区加载内容，并将内容缓存以便切换后再次加载
        /// </summary>
        WinMain,

        /// <summary>
        /// 在新Win中加载，和打开视图或菜单项相同
        /// </summary>
        NewWin
    }
}
