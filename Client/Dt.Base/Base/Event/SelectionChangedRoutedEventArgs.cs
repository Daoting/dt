#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;

#endregion

namespace Dt.Base
{
    /// <summary>
    /// Provides data for the SelectionChanged event.
    /// </summary>
    public class SelectionChangedRoutedEventArgs : BaseRoutedEventArgs
    {
        object[] addedItems;
        object[] removedItems;

        /// <summary>
        /// 初始化PhiSelectionChangedEventArgs类的一个新实例
        /// </summary>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="removedItems">在发生此事件期间，未选定的项。</param>
        /// <param name="addedItems">在发生此事件期间，选定的项。</param>
        public SelectionChangedRoutedEventArgs(BaseRoutedEvent routedEvent, IList removedItems, IList addedItems)
            : base(routedEvent)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException("routedEvent", "必须指定路由事件的类型！");
            }
            if (removedItems == null)
            {
                throw new ArgumentNullException("removedItems", "RadSelectionChangedEventArgsRemovedItemsArgumentNullException");
            }
            if (addedItems == null)
            {
                throw new ArgumentNullException("addedItems", "RadSelectionChangedEventArgsAddedItemsArgumentNullException");
            }
            this.removedItems = new object[removedItems.Count];
            removedItems.CopyTo(this.removedItems, 0);
            this.addedItems = new object[addedItems.Count];
            addedItems.CopyTo(this.addedItems, 0);
        }

        /// <summary>
        /// 初始化PhiSelectionChangedEventArgs类的一个新实例
        /// </summary>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="source">对引发事件的对象的引用</param>
        /// <param name="removedItems">在发生此事件期间，未选定的项。</param>
        /// <param name="addedItems">在发生此事件期间，未选定的项。</param>
        public SelectionChangedRoutedEventArgs(BaseRoutedEvent routedEvent, object source, IList removedItems, IList addedItems)
            : base(routedEvent, source)
        {
            if (removedItems == null)
            {
                throw new ArgumentNullException("removedItems", "RadSelectionChangedEventArgsRemovedItemsArgumentNullException");
            }
            if (addedItems == null)
            {
                throw new ArgumentNullException("addedItems", "RadSelectionChangedEventArgsAddedItemsArgumentNullException");
            }
            this.removedItems = new object[removedItems.Count];
            removedItems.CopyTo(this.removedItems, 0);
            this.addedItems = new object[addedItems.Count];
            addedItems.CopyTo(this.addedItems, 0);
        }

        /// <summary>
        /// 获取包含选定项的列表。
        /// </summary>
        /// <value>
        /// 上次发生 SelectionChanged 事件以来选定的项。
        /// </value>
        public IList AddedItems
        {
            get { return  this.addedItems; }
        }

        /// <summary>
        /// 获取包含未选定项的列表。 
        /// </summary>
        /// <value>
        /// 上次发生 SelectionChanged 事件以来未选定的项。
        /// </value>
        public IList RemovedItems
        {
            get { return  this.removedItems; }
        }
    }
}

