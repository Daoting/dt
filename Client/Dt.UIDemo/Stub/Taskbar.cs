#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.UIDemo
{
    class Taskbar : ITaskbar
    {
        /// <summary>
        /// 获取任务栏左侧的开始界面
        /// </summary>
        /// <returns></returns>
        public FrameworkElement GetStartUI()
        {
            var btn = new Button
            {
                Content = "\uE08E",
                Style = Res.字符按钮,
            };
            ToolTipService.SetToolTip(btn, "开始");
            return btn;
        }

        /// <summary>
        /// 获取任务栏右侧的托盘界面
        /// </summary>
        /// <returns></returns>
        public FrameworkElement GetTrayUI()
        {
            var btn = new Button
            {
                Content = "\uE004",
                Style = Res.字符按钮,
            };
            return btn;
        }
    }
}
