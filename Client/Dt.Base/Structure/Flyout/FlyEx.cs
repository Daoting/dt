#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    // 移入DlgEx，使用Dlg作为浮动面板
    ///// <summary>
    ///// 浮动面板扩展方法
    ///// </summary>
    //public static class FlyEx
    //{
    //    /// <summary>
    //    /// 显示提示消息
    //    /// </summary>
    //    /// <param name="p_target"></param>
    //    /// <param name="p_content"></param>
    //    public static void Msg(this FrameworkElement p_target, string p_content)
    //    {
    //        ShowFly(p_target, p_content, Res.主蓝);
    //    }

    //    /// <summary>
    //    /// 显示警告信息
    //    /// </summary>
    //    /// <param name="p_target"></param>
    //    /// <param name="p_content"></param>
    //    public static void Warn(this FrameworkElement p_target, string p_content)
    //    {
    //        ShowFly(p_target, p_content, Res.RedBrush);
    //    }

    //    static void ShowFly(FrameworkElement p_target, string p_content, SolidColorBrush p_brush)
    //    {
    //        if (p_target == null || string.IsNullOrEmpty(p_content))
    //            return;

    //        Fly fly = new Fly();
    //        Grid grid = new Grid { Background = p_brush, Padding = new Thickness(10), MinWidth = 80d, MaxWidth = 240d };
    //        TextBlock tb = new TextBlock { Text = p_content, Foreground = Res.WhiteBrush };
    //        grid.Children.Add(tb);
    //        fly.Child = grid;
    //        fly.ShowAt(p_target);
    //    }
    //}
}
