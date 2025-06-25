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
    /// 内含Lv的Tab：
    /// 减少无用查询、统一查询方法、创建默认菜单、统一触发事件
    /// </summary>
    [ContentProperty(Name = nameof(Lv))]
    public partial class List : Tab
    {
        #region 静态成员
        public readonly static DependencyProperty LvProperty = DependencyProperty.Register(
            "Lv",
            typeof(Lv),
            typeof(List),
            new PropertyMetadata(null, OnLvChanged));

        static void OnLvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tab = (List)d;
            if (e.OldValue is Lv oldLv)
            {
                oldLv.SelectionChanged -= tab.OnSelectionChanged;
                oldLv.ItemDoubleClick -= tab.OnItemDbClick;
                if (Kit.IsPhoneUI)
                    oldLv.ItemClick -= tab.OnItemClick;
            }

            // 附加Lv事件
            // ItemDoubleClick SelectionChanged触发Msg事件
            // ItemClick在PhoneUI模式触发Navi事件
            if (e.NewValue is Lv newLv)
            {
                newLv.SelectionChanged += tab.OnSelectionChanged;
                newLv.ItemDoubleClick += tab.OnItemDbClick;
                if (Kit.IsPhoneUI)
                    newLv.ItemClick += tab.OnItemClick;
                tab.Content = newLv;
            }
        }
        #endregion

        #region 变量
        protected QueryClause _clause;
        protected long? _parentID;
        bool _needRefresh;
        bool _toggling;
        long? _selectID;
        #endregion

        #region 构造方法
        public List()
        { }
        #endregion
        
        #region 事件
        /// <summary>
        /// 点击增加、编辑菜单项、双击行、切换选择行等触发的事件，参数包含源事件类型及操作
        /// </summary>
        public event Action<LvMsgArgs> Msg;

        /// <summary>
        /// PhoneUI模式点击行向前导航事件
        /// </summary>
        public event Action Navi;
        #endregion

        #region 属性
        /// <summary>
        /// 主Lv
        /// </summary>
        public Lv Lv
        {
            get { return (Lv)GetValue(LvProperty); }
            set { SetValue(LvProperty, value); }
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
                // 不可见时需关闭Form，避免误操作
                if (Msg != null)
                {
                    Msg(new LvMsgArgs
                    {
                        Event = LvEventType.NotSelectedTab,
                        Action = FormAction.Close,
                    });
                }
                return;
            }

            // 加载数据，屏蔽切换太快上次未更新的情况
            if (_toggling)
            {
                Kit.Debug("List跳过加载");
                return;
            }

            try
            {
                _toggling = true;

                long? lastID = (Lv.SelectionMode == SelectionMode.Multiple) ? null : (_selectID ?? Lv.SelectedRow?.ID);

                // 切换太快时加载的并非最后切换，不需处理
                await OnQuery();

                object sel = null;
                if (lastID > 0)
                {
                    if (Lv.Data != null && Lv.Data.Count > 0)
                    {
                        foreach (var item in Lv.Data)
                        {
                            if (item is Row row && row.ID == lastID)
                            {
                                sel = item;
                                break;
                            }
                        }
                    }
                }

                if (sel != null)
                {
                    // 选择但不触发SelectionChanged事件
                    Lv.Select(new List<object> { sel }, false);
                }
                else
                {
                    Lv.OnClearSelection();
                }
            }
            finally
            {
                _toggling = false;
                _needRefresh = false;
            }
        }

        /// <summary>
        /// 根据查询条件加载数据
        /// </summary>
        /// <param name="p_clause">查询条件</param>
        public void Query(QueryClause p_clause)
        {
            if (_clause == p_clause)
                return;

            _clause = p_clause;
            _parentID = null;
            _ = Refresh();
        }

        /// <summary>
        /// 查询所有子实体数据
        /// </summary>
        /// <param name="p_parentID">父ID</param>
        public void Query(long? p_parentID)
        {
            if (_parentID == p_parentID)
                return;

            _parentID = p_parentID;
            _clause = null;
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
        protected virtual Task OnQuery() => Task.CompletedTask;

        /// <summary>
        /// 增加
        /// </summary>
        protected virtual void OnAdd()
        {
            if (Msg != null)
            {
                Msg(new LvMsgArgs
                {
                    ID = -1,
                    ParentID = _parentID,
                    Event = LvEventType.Add
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
                Msg(new LvMsgArgs
                {
                    ID = r.ID,
                    Data = r,
                    ParentID = _parentID,
                    Event = LvEventType.Edit,
                });
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="e"></param>
        protected virtual async void OnDel(Mi e)
        {
            List<Entity> entities = null;
            if (Lv.SelectionMode == SelectionMode.Multiple)
            {
                entities = Lv.SelectedItems.Cast<Entity>().ToList();
            }
            else if (e.Data is Entity entity)
            {
                entities = new List<Entity> { entity };
            }
            else if (Lv.SelectedItem is Entity en)
            {
                entities = new List<Entity> { en };
            }

            if (entities != null && entities.Count > 0)
            {
                if (!await Kit.Confirm("确认要删除选择的数据吗？"))
                {
                    Kit.Msg("已取消删除！");
                    return;
                }

                if (await entities.Delete())
                {
                    await Refresh();
                }
            }
        }

        /// <summary>
        /// 切换选择行事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lv.SelectionMode != SelectionMode.Multiple
                && Msg != null)
            {
                var r = Lv.SelectedRow;
                if (r != null)
                {
                    Msg(new LvMsgArgs
                    {
                        ID = r.ID,
                        Data = r,
                        ParentID = _parentID,
                        Event = LvEventType.SelectionChanged,
                        Action = FormAction.None,
                    });
                }
                else
                {
                    // 清空Form数据
                    Msg(new LvMsgArgs
                    {
                        ParentID = _parentID,
                        Event = LvEventType.SelectionChanged,
                        Action = FormAction.Clear,
                    });
                }
            }
        }

        /// <summary>
        /// 双击行事件处理
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnItemDbClick(object e)
        {
            if (Lv.SelectionMode != SelectionMode.Multiple
                && Msg != null)
            {
                var r = Lv.SelectedRow;
                Msg(new LvMsgArgs
                {
                    ID = r.ID,
                    Data = r,
                    ParentID = _parentID,
                    Event = LvEventType.DbClick,
                });
            }
        }

        /// <summary>
        /// 单击行事件处理
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnItemClick(ItemClickArgs e)
        {
            if (Lv.SelectionMode != SelectionMode.Multiple)
            {
                Navi?.Invoke();
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 添加默认菜单项：增加、删除，若提供Menu则添加默认项，否则创建新菜单
        /// </summary>
        /// <param name="p_menu">若提供Menu则添加默认项，否则创建新菜单</param>
        /// <param name="add">是否添加"增加"菜单项</param>
        /// <param name="del">是否添加"删除"菜单项</param>
        /// <returns></returns>
        protected Menu CreateMenu(Menu p_menu = null, bool add = true, bool del = true)
        {
            Menu menu = p_menu ?? new Menu();
            if (add)
                menu.Items.Add(Mi.增加(call: OnAdd));
            if (del)
                menu.Items.Add(Mi.删除(click: OnDel));
            return menu;
        }

        /// <summary>
        /// 添加Lv默认上下文菜单项：编辑、删除，若提供Menu则添加默认项，否则创建新菜单
        /// </summary>
        /// <param name="p_menu">若提供Menu则添加默认项，否则创建新菜单</param>
        /// <param name="edit">是否添加"编辑"菜单项</param>
        /// <param name="del">是否添加"删除"菜单项</param>
        /// <returns></returns>
        protected Menu CreateContextMenu(Menu p_menu = null, bool edit = true, bool del = true)
        {
            Menu menu = p_menu ?? new Menu();
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
        protected void OnMsg(LvMsgArgs p_args)
        {
            Msg?.Invoke(p_args);
        }
        #endregion
    }
}
