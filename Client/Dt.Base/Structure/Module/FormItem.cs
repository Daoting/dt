#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media.Animation;
using System.ComponentModel;
using System.Reflection;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 子列表及子表单
    /// </summary>
    public partial class Fi : DependencyObject
    {
        #region 静态内容
        public static readonly DependencyProperty LvProperty = DependencyProperty.Register(
            "Lv",
            typeof(Lv),
            typeof(Fi),
            new PropertyMetadata(null));

        public static readonly DependencyProperty FvProperty = DependencyProperty.Register(
            "Fv",
            typeof(Fv),
            typeof(Fi),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(Fi),
            new PropertyMetadata(false, OnIsReadOnlyChanged));

        public static readonly DependencyProperty ConfirmDeleteProperty = DependencyProperty.Register(
            "ConfirmDelete",
            typeof(bool),
            typeof(Fi),
            new PropertyMetadata(false));
        
        static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Fi)d).OnIsReadOnlyChanged();
        }
        #endregion

        #region 成员变量
        const double _lvMinHeight = 120;
        readonly CBar _bar = new CBar { MinHeight = Res.RowOuterHeight };
        ContentPresenter _presenter;
        Form _form;
        #endregion
        
        /// <summary>
        /// 获取设置分隔行标题
        /// </summary>
        public string Title
        {
            get { return _bar.Title; }
            set { _bar.Title = value; }
        }

        /// <summary>
        /// 分割栏
        /// </summary>
        public CBar Bar => _bar;

        /// <summary>
        /// 子列表
        /// </summary>
        public Lv Lv
        {
            get { return (Lv)GetValue(LvProperty); }
            set { SetValue(LvProperty, value); }
        }

        /// <summary>
        /// 子表单
        /// </summary>
        public Fv Fv
        {
            get { return (Fv)GetValue(FvProperty); }
            set { SetValue(FvProperty, value); }
        }

        /// <summary>
        /// 删除前是否需要确认
        /// </summary>
        public bool ConfirmDelete
        {
            get { return (bool)GetValue(ConfirmDeleteProperty); }
            set { SetValue(ConfirmDeleteProperty, value); }
        }

        /// <summary>
        /// 获取设置表单是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Lv工具栏菜单
        /// </summary>
        public Menu LvMenu { get; private set; }

        /// <summary>
        /// Lv上下文菜单
        /// </summary>
        public Menu LvContextMenu { get; private set; }

        /// <summary>
        /// Fv工具栏菜单
        /// </summary>
        public Menu FvMenu { get; private set; }

        /// <summary>
        /// 切换到表单模式
        /// </summary>
        /// <param name="p_row"></param>
        public void ShowFv(Row p_row)
        {
            Fv fv = Fv;
            if (fv == null)
                return;

            if (FvMenu == null)
                CreateFvMenu();

            if (IsReadOnly)
            {
                FvMenu[0].Visibility = Visibility.Collapsed;
                FvMenu[1].Visibility = Visibility.Collapsed;
                FvMenu[2].Visibility = Visibility.Collapsed;
                fv.IsReadOnly = true;
            }
            else
            {
                FvMenu[0].Visibility = Visibility.Visible;
                FvMenu[1].Visibility = Visibility.Visible;
                FvMenu[2].Visibility = Visibility.Visible;
                if (fv.IsReadOnly)
                    fv.IsReadOnly = false;
            }
            
            if (Bar.Content != FvMenu)
            {
                // 被CBar修改
                FvMenu.Margin = new Thickness();
                Bar.Content = FvMenu;

                if (!Kit.IsPhoneUI && double.IsInfinity(Fv.MaxHeight))
                {
                    // 按对话框宽度水平优先摆放
                    int colCount = Math.Max(1, (int)Math.Floor(_form.Width / FormPanel.CellMinWidth));
                    Fv.ApplyTemplate();
                    double height = Fv.GetTotalHeight(colCount);
                    Fv.MaxHeight = Math.Ceiling(height / 0.8);
                    Fv.MaxWidth = _form.Width;
                }

                _presenter.Content = Fv;
                // 确保校验时能定位错误位置
                _presenter.UpdateLayout();
            }
            Fv.Data = p_row;

            int no = Lv.Table.IndexOf(p_row);
            FvMenu[3].IsEnabled = no > 0;
            FvMenu[4].IsEnabled = no < Lv.Table.Count - 1;
        }

        /// <summary>
        /// 切换到列表模式
        /// </summary>
        public void ShowLv()
        {
            _presenter.Content = Lv;
            if (IsReadOnly)
            {
                Bar.Content = null;
                Lv.SetMenu(null);
                Lv.ClearValue(Ex.MenuProperty);
            }
            else if (Bar.Content != LvMenu)
            {
                // 被CBar修改
                LvMenu.Margin = new Thickness();
                Bar.Content = LvMenu;
                Lv.SetMenu(LvContextMenu);
            }
        }

        internal Grid Init(Form p_form)
        {
            _form = p_form;
            
            // UI框架
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                },
            };
            grid.Children.Add(Bar);

            _presenter = new ContentPresenter();
#if WIN
            var trans = new TransitionCollection();
            trans.Add(new PaneThemeTransition { Edge = EdgeTransitionLocation.Top });
            _presenter.ContentTransitions = trans;
#endif
            Grid.SetRow(_presenter, 1);
            grid.Children.Add(_presenter);

            Lv lv = Lv;
            if (lv == null)
                Throw.Msg($"父子表单中未定义【{Title}】的Lv！");

            lv.ItemDoubleClick += OnItemDbClick;
            lv.DataChanged += OnLvDataChanged;
            if (lv.MinHeight < _lvMinHeight)
                lv.MinHeight = _lvMinHeight;

            CreateLvMenu();
            ShowLv();

            Fv fv = Fv;
            if (fv != null)
            {
                // 最下面Fv补充下划线
                fv.BorderThickness = new Thickness(0, 0, 0, 1);
                fv.BorderBrush = Res.浅灰2;
                fv.VerticalAlignment = VerticalAlignment.Top;
            }
            return grid;
        }

        internal void ClearData()
        {
            ShowLv();
            Lv.Data = null;
            Fv.Data = null;
        }

        void OnIsReadOnlyChanged()
        {
            // 未初始化
            if (_presenter == null)
                return;

            if (_presenter.Content == Lv)
            {
                ShowLv();
            }
            else if (_presenter.Content == Fv)
            {
                ShowFv(Fv.Row);
            }
        }
        
        async void OnAdd()
        {
            await _form.AddChild(Fv);
            var row = Fv.Row;
            if (row == null)
                Throw.Msg("OnAddChild创建子实体不可为空！");

            // 创建空数据源
            if (Lv.Data == null)
            {
                if (row is Entity entity)
                {
                    // 调用 Table<T>.Create(Entity) 创建空数据源
                    var tp = typeof(Table<>).MakeGenericType(entity.GetType());
                    var method = tp.GetMethod("Create", BindingFlags.Public | BindingFlags.Static, new Type[] { entity.GetType() });
                    Lv.Data = method.Invoke(null, new object[] { entity }) as Table;
                }
                else
                {
                    Lv.Data = Table.Clone(row);
                }
            }

            Lv.Table.Add(row);
            ShowFv(row);
        }

        void OnEdit(Mi e)
        {
            ShowFv(e.Row);
        }

        async void OnDel(Mi e)
        {
            List<Row> rows = null;
            if (Lv.SelectionMode == SelectionMode.Multiple)
            {
                rows = Lv.SelectedRows.ToList();
            }
            else if (e.Data is Row row)
            {
                rows = new List<Row> { row };
            }
            else if (Lv.SelectedRow != null)
            {
                rows = new List<Row> { Lv.SelectedRow };
            }

            if (rows != null && rows.Count > 0)
            {
                if (ConfirmDelete)
                {
                    if (!await Kit.Confirm("确认要删除选择的数据吗？"))
                    {
                        Kit.Msg("已取消删除！");
                        return;
                    }
                }
                Lv.Table.RemoveRange(rows);
            }
        }

        async void OnDelInFv()
        {
            if (Fv.Data == null || Lv.Table == null)
                return;

            int index = Lv.Table.IndexOf(Fv.Row);
            if (index < 0)
                return;

            if (ConfirmDelete)
            {
                if (!await Kit.Confirm("确认要删除选择的数据吗？"))
                {
                    Kit.Msg("已取消删除！");
                    return;
                }
            }

            Lv.Table.RemoveAt(index);
            if (index < Lv.Table.Count)
                ShowFv(Lv.Table[index]);
            else if (Lv.Table.Count > 0)
                ShowFv(Lv.Table[0]);
            else
                ShowLv();
        }

        void OnItemDbClick(object e)
        {
            if (Lv.SelectionMode != SelectionMode.Multiple
                && Fv != null)
            {
                ShowFv(Lv.SelectedRow);
            }
        }

        void OnNextRow()
        {
            int no = Lv.Table.IndexOf(Fv.Row);
            if (no > -1 && no < Lv.Table.Count - 1)
                ShowFv(Lv.Table[no + 1]);
        }

        void OnPreRow()
        {
            int no = Lv.Table.IndexOf(Fv.Row);
            if (no > 0)
                ShowFv(Lv.Table[no - 1]);
        }

        void CreateLvMenu()
        {
            // Lv工具栏菜单
            Menu menu = new Menu { HorizontalAlignment = HorizontalAlignment.Right };
            Mi mi = new Mi { Icon = Icons.加号, ShowInPhone = VisibleInPhone.Icon };
            mi.Call += OnAdd;
            menu.Items.Add(mi);

            mi = new Mi { Icon = Icons.删除, ShowInPhone = VisibleInPhone.Icon };
            mi.Click += OnDel;
            menu.Items.Add(mi);
            Lv.AddMultiSelMenu(menu);
            LvMenu = menu;

            // Lv上下文菜单
            menu = new Menu();
            mi = new Mi
            {
                ID = "编辑",
                Icon = Icons.修改,
            };
            mi.Click += OnEdit;
            menu.Items.Add(mi);

            mi = new Mi
            {
                ID = "删除",
                Icon = Icons.删除,
            };
            mi.Click += OnDel;
            menu.Items.Add(mi);
            LvContextMenu = menu;
        }

        void CreateFvMenu()
        {
            Menu menu = new Menu { HorizontalAlignment = HorizontalAlignment.Right };
            Mi mi = new Mi { Icon = Icons.加号, ShowInPhone = VisibleInPhone.Icon };
            mi.Call += OnAdd;
            menu.Items.Add(mi);

            mi = new Mi { Icon = Icons.删除, ShowInPhone = VisibleInPhone.Icon };
            mi.Call += OnDelInFv;
            menu.Items.Add(mi);

            mi = new Mi { Icon = Icons.撤消, ShowInPhone = VisibleInPhone.Icon };
            mi.Call += () => Fv.RejectChanges();
            mi.SetBinding(Mi.IsEnabledProperty, new Binding { Path = new PropertyPath("IsDirty"), Source = Fv });
            menu.Items.Add(mi);

            mi = new Mi { Icon = Icons.向前, ShowInPhone = VisibleInPhone.Icon };
            mi.Call += OnPreRow;
            menu.Items.Add(mi);

            mi = new Mi { Icon = Icons.向后, ShowInPhone = VisibleInPhone.Icon };
            mi.Call += OnNextRow;
            menu.Items.Add(mi);

            mi = new Mi { Icon = Icons.列表, ShowInPhone = VisibleInPhone.Icon };
            mi.Call += ShowLv;
            menu.Items.Add(mi);

            FvMenu = menu;
        }

        Table _lastTbl = null;
        void OnLvDataChanged(INotifyList obj)
        {
            if (_lastTbl != null)
            {
                _lastTbl.Dirty -= OnTblDirty;
                _lastTbl.UnlockCollection();
            }

            _lastTbl = obj as Table;
            if (_lastTbl != null && !IsReadOnly)
            {
                _lastTbl.LockCollection();
                _lastTbl.Dirty += OnTblDirty;
            }
            _form.UpdateDirtyState();
        }

        void OnTblDirty(bool obj)
        {
            _form.UpdateDirtyState();
        }
    }
}
