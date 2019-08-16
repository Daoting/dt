#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Base.Docking
{
    internal class PinChangeEventArgs : BaseRoutedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="source"></param>
        /// <param name="isPinned"></param>
        public PinChangeEventArgs(BaseRoutedEvent routedEvent, object source, bool isPinned)
            : base(routedEvent, source)
        {
            IsPinned = isPinned;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPinned { get; set; }
    }
}

