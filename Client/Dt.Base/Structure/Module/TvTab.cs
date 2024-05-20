#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-26 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 内部含Tv，数据源为Table的Tab，
    /// 主要为减少无用的数据查询，不可见不查询
    /// </summary>
    public abstract partial class TvTab : Tab
    {
        #region 变量
        bool _needRefresh;
        bool _toggling;
        long? _selectID;
        #endregion

        /// <summary>
        /// 刷新数据
        /// <para>1. Tab不可见不查询，切换到可见时再查询</para>
        /// <para>2. 上次查询未结束，放弃后续的查询</para>
        /// </summary>
        /// <param name="p_selectID">刷新后，null: 选择上次选择行；大于0选择对应行；≤0无选择行</param>
        /// <returns></returns>
        public async Task Refresh(long? p_selectID = null)
        {
            _selectID = p_selectID;

            // 不可见不查询
            if (!IsSelected)
            {
                _needRefresh = true;
                return;
            }

            // 加载数据，屏蔽切换太快上次未更新的情况
            if (_toggling)
            {
                Kit.Debug("TvTab跳过加载");
                return;
            }

            try
            {
                _toggling = true;

                long? lastID = (Tv.SelectionMode == SelectionMode.Multiple) ? null : (_selectID ?? Tv.SelectedRow?.ID);

                // 切换太快时加载的并非最后切换，不需处理
                await Query();

                if (lastID > 0 && Tv.Data is Table tbl)
                {
                    var sel = (from ti in Tv.RootItems.GetAllItems()
                               where ti.Data is Row r && r.ID == lastID
                               select ti).FirstOrDefault();

                    if (sel != null)
                    {
                        // 选择但不触发SelectionChanged事件
                        Tv.OnToggleSelected(sel, false);
                    }
                    else
                    {
                        Tv.OnClearSelection();
                    }
                }
            }
            finally
            {
                _toggling = false;
                _needRefresh = false;
            }
        }

        protected override async void OnSelected()
        {
            // 切换到可见时更新数据
            if (_needRefresh)
            {
                await Refresh();
                _needRefresh = false;
            }
        }

        /// <summary>
        /// 添加默认菜单项，若提供Menu则添加默认项，否则创建新菜单
        /// </summary>
        /// <param name="p_menu">若提供Menu则添加默认项，否则创建新菜单</param>
        /// <returns></returns>
        protected Menu AddMenu(Menu p_menu = null)
        {
            Menu menu = p_menu ?? new Menu();
            Mi mi = new Mi
            {
                ID = "展开",
                Icon = Icons.展开,
                ShowInPhone = VisibleInPhone.Icon,
            };
            mi.Call += () => Tv.ExpandAll();
            menu.Items.Add(mi);

            mi = new Mi
            {
                ID = "折叠",
                Icon = Icons.收起,
                ShowInPhone = VisibleInPhone.Icon,
            };
            mi.Call += () => Tv.CollapseAll();
            menu.Items.Add(mi);
            
            return menu;
        }

        /// <summary>
        /// 内部Tv
        /// </summary>
        protected abstract Tv Tv { get; }

        /// <summary>
        /// 查询
        /// </summary>
        protected abstract Task Query();
    }
}
