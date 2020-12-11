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

        static void OnItemsChanged(object sender, ItemListChangedArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemInserted || e.CollectionChange == CollectionChange.ItemChanged)
            {
                var info = ((ItemList<NotifyInfo>)sender)[e.Index];
                SysVisual.InsertNotifyItem(e.Index, new NotifyItem(info));
            }
            else if (e.CollectionChange == CollectionChange.ItemRemoved)
            {
                SysVisual.RemoveNotifyItem(e.Index);
            }
            else
            {
                SysVisual.ClearAllNotify();
            }
        }
    }
}
