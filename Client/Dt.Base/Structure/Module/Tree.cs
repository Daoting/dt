#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 内含Tv的Tab：
    /// 减少无用查询、统一查询方法、创建默认菜单、统一触发事件
    /// </summary>
    [ContentProperty(Name = nameof(Tv))]
    public partial class Tree : Tab
    {
        #region 静态成员
        public readonly static DependencyProperty TvProperty = DependencyProperty.Register(
            "Tv",
            typeof(Tv),
            typeof(Tree),
            new PropertyMetadata(null, OnTvChanged));

        static void OnTvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tab = (Tree)d;
            if (e.OldValue is Tv oldTv)
            {
                oldTv.SelectionChanged -= tab.OnSelectionChanged;
                if (Kit.IsPhoneUI)
                    oldTv.ItemClick -= tab.OnItemClick;
            }

            // 附加事件
            // SelectionChanged触发Msg事件
            // ItemClick在PhoneUI模式触发Navi事件
            if (e.NewValue is Tv newTv)
            {
                newTv.SelectionChanged += tab.OnSelectionChanged;
                if (Kit.IsPhoneUI)
                    newTv.ItemClick += tab.OnItemClick;
                tab.Content = newTv;
            }
        }
        #endregion
        
        #region 变量
        protected long? _parentID;
        bool _needRefresh;
        bool _toggling;
        long? _selectID;
        #endregion

        #region 构造方法
        public Tree()
        { }
        #endregion
        
        #region 事件
        /// <summary>
        /// 切换选择节点、点击增加、编辑菜单项等触发的事件
        /// </summary>
        public event Action<TvMsgArgs> Msg;

        /// <summary>
        /// PhoneUI模式点击行向前导航事件
        /// </summary>
        public event Action Navi;
        #endregion

        #region 属性
        /// <summary>
        /// 主Tv
        /// </summary>
        public Tv Tv
        {
            get { return (Tv)GetValue(TvProperty); }
            set { SetValue(TvProperty, value); }
        }
        #endregion
        
        #region 外部方法
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
                Kit.Debug("Tree跳过加载");
                return;
            }

            try
            {
                _toggling = true;

                long? lastID = (Tv.SelectionMode == SelectionMode.Multiple) ? null : (_selectID ?? Tv.SelectedRow?.ID);

                // 切换太快时加载的并非最后切换，不需处理
                await OnQuery();

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

        /// <summary>
        /// 查询所有子节点数据
        /// </summary>
        /// <param name="p_parentID">父ID</param>
        public void Query(long p_parentID)
        {
            if (_parentID == p_parentID)
                return;

            _parentID = p_parentID;
            _ = Refresh();
        }
        #endregion

        #region 重写
        protected override async void OnSelected()
        {
            // 切换到可见时更新数据
            if (_needRefresh)
            {
                await Refresh();
                _needRefresh = false;
            }
        }
        #endregion

        #region 虚方法
        /// <summary>
        /// 查询
        /// </summary>
        protected virtual Task OnQuery()=> Task.CompletedTask;

        /// <summary>
        /// 增加
        /// </summary>
        protected virtual void OnAdd(Mi e)
        {
            if (Msg != null)
            {
                Msg(new TvMsgArgs
                {
                    ID = -1,
                    ParentID = _parentID,
                    Event = TvEventType.Add
                });
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEdit(Mi e)
        {
            if (Msg != null)
            {
                var r = e.Row;
                Msg(new TvMsgArgs
                {
                    ID = r.ID,
                    Data = r,
                    ParentID = _parentID,
                    Event = TvEventType.Edit,
                });
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="e"></param>
        protected virtual async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (Tv.SelectionMode == SelectionMode.Multiple)
            {
                var ls = Tv.SelectedItems.Cast<Entity>().ToList();
                if (await ls.Delete())
                {
                    await Refresh();
                }
            }
            else if (e.Data is Entity entity)
            {
                if (await entity.Delete())
                    await Refresh();
            }
        }

        /// <summary>
        /// 切换选择行事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tv.SelectionMode != SelectionMode.Multiple
                && Msg != null)
            {
                var r = Tv.SelectedRow;
                if (r != null)
                {
                    Msg(new TvMsgArgs
                    {
                        ID = r.ID,
                        Data = r,
                        ParentID = _parentID,
                        Event = TvEventType.SelectionChanged,
                    });
                }
                else
                {
                    Msg(new TvMsgArgs
                    {
                        // 无选择节点
                        ID = null,
                        ParentID = _parentID,
                        Event = TvEventType.SelectionChanged,
                    });
                }
            }
        }

        /// <summary>
        /// 单击行事件处理
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnItemClick(ItemClickArgs e)
        {
            if (Tv.SelectionMode != SelectionMode.Multiple)
            {
                Navi?.Invoke();
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 添加默认菜单项：展开、折叠、刷新，若提供Menu则添加默认项，否则创建新菜单
        /// </summary>
        /// <param name="p_menu">若提供Menu则添加默认项，否则创建新菜单</param>
        /// <param name="refresh">是否含刷新菜单项</param>
        /// <returns></returns>
        protected Menu CreateMenu(Menu p_menu = null, bool refresh = true)
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

            if (refresh)
            {
                mi = new Mi
                {
                    ID = "刷新",
                    Icon = Icons.刷新,
                    ShowInPhone = VisibleInPhone.Icon,
                };
                mi.Call += () => _ = Refresh();
                menu.Items.Add(mi);
            }
            
            return menu;
        }

        /// <summary>
        /// 添加Tv默认上下文菜单项：编辑、删除，若提供Menu则添加默认项，否则创建新菜单
        /// </summary>
        /// <param name="p_menu">若提供Menu则添加默认项，否则创建新菜单</param>
        /// <param name="add"></param>
        /// <param name="edit"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        protected Menu CreateContextMenu(Menu p_menu = null, bool add = true, bool edit = true, bool del = true)
        {
            Menu menu = p_menu ?? new Menu();
            if (add)
                menu.Items.Add(Mi.增加(click: OnAdd));
            
            if (edit)
                menu.Items.Add(Mi.编辑(click: OnEdit));

            if (del)
                menu.Items.Add(Mi.删除(click: OnDel));
            return menu;
        }

        /// <summary>
        /// 触发Msg事件
        /// </summary>
        /// <param name="p_args"></param>
        protected void OnMsg(TvMsgArgs p_args)
        {
            Msg?.Invoke(p_args);
        }
        #endregion
    }
}
