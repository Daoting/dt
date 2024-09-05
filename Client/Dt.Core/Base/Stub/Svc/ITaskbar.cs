#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 桌面任务栏接口
    /// </summary>
    public interface ITaskbar
    {
        /// <summary>
        /// 获取任务栏左侧的开始界面
        /// </summary>
        /// <returns></returns>
        FrameworkElement GetStartUI();

        /// <summary>
        /// 获取任务栏右侧的托盘界面
        /// </summary>
        /// <returns></returns>
        FrameworkElement GetTrayUI();
    }
}