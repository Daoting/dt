#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 拖动过程参数
    /// </summary>
    internal class DragInfoEventArgs : BaseRoutedEventArgs
    {
        public DragInfoEventArgs(BaseRoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        public DragInfoEventArgs(BaseRoutedEvent routedEvent, PointerRoutedEventArgs args)
            : this(routedEvent)
        {
            PointerArgs = args;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            DragInfoEventHandler handler = (DragInfoEventHandler)genericHandler;
            handler(genericTarget, this);
        }

        /// <summary>
        /// 鼠标参数
        /// </summary>
        public PointerRoutedEventArgs PointerArgs { get; set; }
    }

    internal delegate void DragInfoEventHandler(object sender, DragInfoEventArgs e);
}

