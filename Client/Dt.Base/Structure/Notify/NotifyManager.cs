#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-06-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Windows.Foundation.Collections;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 提示信息显示管理
    /// </summary>
    internal static class NotifyManager
    {
        static NotifyManager()
        {
            SysVisual.NotifyList.ItemsChanged += OnItemsChanged;
        }

        /// <summary>
        /// 初始化提示管理
        /// </summary>
        public static void Init()
        {
        }

        static void OnItemsChanged(ItemList<NotifyInfo> sender, ItemListChangedArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemRemoved || e.CollectionChange == CollectionChange.ItemChanged)
                SysVisual.RemoveNotifyItem(e.Index);

            if (e.CollectionChange == CollectionChange.ItemInserted || e.CollectionChange == CollectionChange.ItemChanged)
            {
                var info = sender[e.Index];
                SysVisual.InsertNotifyItem(e.Index, new NotifyItem(info));
            }
            else if (e.CollectionChange == CollectionChange.Reset)
            {
                SysVisual.ClearAllNotify();
            }
        }
    }
}
