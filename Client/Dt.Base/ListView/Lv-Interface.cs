#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Dt.Base.ListView;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 接口
    /// </summary>
    public partial class Lv : IViewItemHost, IMenuHost, IWinCleaner
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

        #region IWinCleaner
        void IWinCleaner.Unload()
        {
            _panel?.Unload();
            
            if (_rows.Count > 0)
            {
                while (_rows.Count > 0)
                {
                    ((ILvCleaner)_rows[0]).Unload();
                    _rows.RemoveAt(0);
                }
                _rows = null;
            }

            if (_selectedLvItems.Count > 0)
                _selectedLvItems.Clear();

            if (GroupRows != null)
            {
                while (GroupRows.Count > 0)
                {
                    ((ILvCleaner)GroupRows[0]).Unload();
                    GroupRows.RemoveAt(0);
                }
                GroupRows = null;
            }

            if (MapRows != null)
            {
                MapRows.Clear();
                MapRows = null;
            }
        }
        #endregion
    }
}