#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.ComponentModel;
using Windows.UI.Xaml;
#endregion

namespace Dt.Charts
{
    public class AnnoCreatedEventArgs : CancelEventArgs
    {
        internal AnnoCreatedEventArgs(Windows.UI.Xaml.Controls.Canvas canvas, UIElement label, int index, double value)
        {
            Canvas = canvas;
            Label = label;
            Index = index;
            Value = value;
        }

        public Windows.UI.Xaml.Controls.Canvas Canvas { get; internal set; }

        public int Index { get; internal set; }

        public UIElement Label { get; set; }

        public double Value { get; internal set; }
    }
}

