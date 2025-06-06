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
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using System.ComponentModel;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 表单对话框
    /// </summary>
    [ContentProperty(Name = nameof(MainFv))]
    public abstract partial class Form : Dlg
    {
        #region 静态成员
        public readonly static DependencyProperty MainFvProperty = DependencyProperty.Register(
            "MainFv",
            typeof(Fv),
            typeof(Form),
            new PropertyMetadata(null, OnMainFvChanged));

        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(
            "IsDirty",
            typeof(bool),
            typeof(Form),
            new PropertyMetadata(false, OnIsDirtyChanged));

        public static readonly DependencyProperty CheckChangesProperty = DependencyProperty.Register(
            "CheckChanges",
            typeof(bool),
            typeof(Form),
            new PropertyMetadata(true));

        public static readonly DependencyProperty BeforeAddProperty = DependencyProperty.Register(
            "BeforeAdd",
            typeof(BeforeAddOption),
            typeof(Form),
            new PropertyMetadata(BeforeAddOption.AutoSave));

        static void OnMainFvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fm = (Form)d;
            if (e.OldValue is Fv oldFv)
                oldFv.DataChanged -= fm.OnMainFvDataChanged;
            if (e.NewValue is Fv newFv)
                newFv.DataChanged += fm.OnMainFvDataChanged;
        }

        static void OnIsDirtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Form)d).OnDirty();
        }
        #endregion

        #region 变量
        Grid _root;
        #endregion

        #region 属性
        /// <summary>
        /// 主Fv
        /// </summary>
        public Fv MainFv
        {
            get { return (Fv)GetValue(MainFvProperty); }
            set { SetValue(MainFvProperty, value); }
        }

        /// <summary>
        /// 子列表集合
        /// </summary>
        public List<Fi> Items { get; } = new List<Fi>();

        /// <summary>
        /// 获取表单数据是否已修改
        /// </summary>
        public bool IsDirty
        {
            get { return (bool)GetValue(IsDirtyProperty); }
            set { SetValue(IsDirtyProperty, value); }
        }

        /// <summary>
        /// 切换数据源或关闭前是否检查、提示旧数据已修改
        /// </summary>
        public bool CheckChanges
        {
            get { return (bool)GetValue(CheckChangesProperty); }
            set { SetValue(CheckChangesProperty, value); }
        }

        /// <summary>
        /// 增加前选项：自动保存已修改的数据、提示、不检查
        /// </summary>
        public BeforeAddOption BeforeAdd
        {
            get { return (BeforeAddOption)GetValue(BeforeAddProperty); }
            set { SetValue(BeforeAddProperty, value); }
        }
        #endregion

        #region OnApplyTemplate
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (MainFv == null)
                return;
            
            // 非父子表单
            if (Items.Count == 0)
            {
                MinHeight = 300;
                Content = MainFv;
                return;
            }

            if (!Kit.IsPhoneUI)
            {
                // 两列摆放占可视高度的50%以内时按两列；超出高度时按三列、四列
                MainFv.ApplyTemplate();
                double maxHeight = Math.Floor(Kit.ViewHeight / 2);
                double height = MainFv.GetTotalHeight(2);
                if (height > maxHeight)
                {
                    height = MainFv.GetTotalHeight(3);
                    if (height > maxHeight)
                        height = MainFv.GetTotalHeight(4);
                }
                MainFv.MaxHeight = Math.Ceiling(height / 0.8);
                MainFv.MaxWidth = Kit.ViewWidth;

                Height = Kit.ViewHeight - 100;
                MinWidth = Math.Min(Kit.ViewWidth, FormPanel.CellMaxWidth * 2);
            }

            _root = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                },
            };
            _root.Children.Add(MainFv);
            
            if (Items.Count == 1)
            {
                var child = Items[0].Init(this);
                Grid.SetRow(child, 1);
                _root.Children.Add(child);
            }
            else
            {
                TabControl tabs = new TabControl();
                foreach (var item in Items)
                {
                    var child = item.Init(this);
                    tabs.Items.Add(new TabItem { Title = item.Title, Content = child });
                }
                Grid.SetRow(tabs, 1);
                _root.Children.Add(tabs);
            }
            
            Content = _root;
        }
        #endregion

        #region IsDirty
        Row _rowLast = null;
        
        void OnMainFvDataChanged(object obj)
        {
            if (_rowLast != null)
            {
                _rowLast.PropertyChanged -= OnRowChanged;
                if (_rowLast is Entity entity)
                    entity.Saved -= OnSavedData;
            }
            
            _rowLast = obj as Row;
            if (_rowLast != null)
            {
                _rowLast.PropertyChanged += OnRowChanged;
                if (_rowLast is Entity entity)
                    entity.Saved += OnSavedData;
            }
            UpdateDirtyState();
            OnFvDataChanged();
        }

        void OnRowChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChanged")
                UpdateDirtyState();
        }

        void OnSavedData(Entity p_entity)
        {
            UpdateRelatedInternal(p_entity.ID, p_entity, UpdateRelatedEvent.Saved);
            UpdateListInternal(p_entity, UpdateListEvent.Saved);
            OnFvDataChanged();
        }

        /// <summary>
        /// 触发数据源修改状态变化事件
        /// </summary>
        void OnDirty()
        {
            Dirty?.Invoke(IsDirty);
        }

        internal void UpdateDirtyState()
        {
            bool changed = _rowLast != null ? _rowLast.IsChanged : false;
            if (Items.Count > 0 && !changed)
            {
                foreach (var fi in Items)
                {
                    if (fi.Lv.Data is Table tbl && tbl.IsDirty)
                    {
                        changed = true;
                        break;
                    }
                }
            }
            IsDirty = changed;
        }
        
        protected List<Table> GetDirtyItemsData()
        {
            var ls = new List<Table>();
            if (Items.Count == 0)
                return ls;

            foreach (var fi in Items)
            {
                if (fi.Lv.Data is Table tbl
                    && tbl.IsDirty
                    && tbl.GetType().IsGenericType)
                {
                    ls.Add(tbl);
                }
            }
            return ls;
        }

        /// <summary>
        /// 所有数据是否为实体类型
        /// </summary>
        /// <returns></returns>
        protected bool IsEntityData()
        {
            if (MainFv.Data is not Entity)
                return false;
            
            if (Items.Count > 0)
            {
                foreach (var fi in Items)
                {
                    if (fi.Lv.Data == null
                        || (fi.Lv.Data is Table tbl && tbl.GetType().IsGenericType))
                        continue;
                    
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
