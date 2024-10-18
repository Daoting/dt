#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 接口
    /// </summary>
    public partial class Lv : IViewItemHost, IMenuHost, IDestroy
    {
        #region IViewItemHost
        bool IViewItemHost.IsCustomItemStyle => ItemStyle != null;

        void IViewItemHost.SetItemStyle(ViewItem p_item)
        {
            ItemStyle?.Invoke(new ItemStyleArgs(p_item));
        }
        #endregion

        #region IMenuHost
        /// <summary>
        /// 切换上下文菜单或修改触发事件种类时通知宿主刷新
        /// </summary>
        void IMenuHost.UpdateContextMenu()
        {
            ReloadPanelContent();
        }
        #endregion

        #region IDestroy
        public void Destroy()
        {
            _panel?.Destroy();

            if (_rows.Count > 0)
            {
                while (_rows.Count > 0)
                {
                    ((ILvDestroy)_rows[0]).Destroy();
                    _rows.RemoveAt(0);
                }
            }

            if (_selectedLvItems.Count > 0)
            {
                _selectedLvItems.CollectionChanged -= OnSelectedItemsChanged;
                _selectedLvItems.Clear();
            }

            if (GroupRows != null)
            {
                while (GroupRows.Count > 0)
                {
                    ((ILvDestroy)GroupRows[0]).Destroy();
                    GroupRows.RemoveAt(0);
                }
                GroupRows = null;
            }

            if (MapRows != null)
            {
                MapRows.Clear();
                MapRows = null;
            }

            if (_dataView != null)
            {
                _dataView.Destroy();
                _dataView = null;
            }
        }
        #endregion
    }
}