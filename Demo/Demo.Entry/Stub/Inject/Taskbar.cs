#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Tools;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Demo.Entry
{
    class MyTaskbar : ITaskbar
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
                Width = 60,
            };
            ToolTipService.SetToolTip(btn, "开始");
            btn.Click += BtnStartClick;
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
                Width = 60,
            };
            btn.Click += Btn_Click;
            return btn;
        }

        void Btn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Dlg
            {
                HideTitleBar = true,
                WinPlacement = DlgPlacement.FromTopRight,
                Width = 400,
                Height = 400,
                Content = new TextBlock { Text = "托盘内容", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
            };
            dlg.Show();
        }

        void BtnStartClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Dlg
            {
                HideTitleBar = true,
                WinPlacement = DlgPlacement.FromTopLeft,
                Width = 400,
                Height = 600,
                Content = new TextBlock { Text = "开始", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
            };
            dlg.Show();
        }
    }
}
