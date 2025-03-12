#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
#endregion

namespace Demo.UI
{
    public partial class DlgDragDemo : Win
    {
        public DlgDragDemo()
        {
            InitializeComponent();
        }

        void OnPointerPressed1(object sender, PointerRoutedEventArgs e)
        {
            e.StartDrag(OnStopDrag);
        }

        void OnPointerPressed2(object sender, PointerRoutedEventArgs e)
        {
            e.StartDrag(OnStopDrag, OnDragging);
        }

        void OnPointerPressed3(object sender, PointerRoutedEventArgs e)
        {
            e.StartDrag(OnStopDrag, OnDragging, OnSetting);
        }

        void OnStopDrag(Point e)
        {
            if (!_bdTgt.ContainPoint(e))
                Kit.Msg("不在目标区域！");
            else
                Kit.Msg("在目标区域！" + e.ToString());
        }

        void OnDragging(Dlg dlg, Point e)
        {
            dlg.Foreground = _bdTgt.ContainPoint(e) ? Res.亮红 : Res.深灰2;
        }

        void OnSetting(Dlg dlg)
        {
            dlg.Title = "\uE004";
        }
    }
}