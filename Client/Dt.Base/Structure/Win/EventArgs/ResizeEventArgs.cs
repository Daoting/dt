#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 调整大小事件参数
    /// </summary>
    public class ResizeEventArgs : BaseRoutedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="source"></param>
        public ResizeEventArgs(BaseRoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            AvailableSize = Size.Empty;
            MinSize = Size.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genericHandler"></param>
        /// <param name="genericTarget"></param>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            EventHandler<ResizeEventArgs> handler = (EventHandler<ResizeEventArgs>)genericHandler;
            handler(genericTarget, this);
        }

        /// <summary>
        /// 要调整大小的目标元素
        /// </summary>
        internal FrameworkElement ResizedTgt { get; set; }

        /// <summary>
        /// 影响到的父容器
        /// </summary>
        internal FrameworkElement AffectedTgt { get; set; }

        internal Size AvailableSize { get; set; }

        internal Size MinSize { get; set; }
    }
}

