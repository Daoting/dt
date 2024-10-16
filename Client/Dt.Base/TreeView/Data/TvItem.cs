#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Reflection;
using Dt.Base.ListView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 树节点视图
    /// </summary>
    public partial class TvItem : ViewItem
    {
        #region 静态内容
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded",
            typeof(bool),
            typeof(TvItem),
            new PropertyMetadata(false, OnIsExpandedChanged));

        public static readonly DependencyProperty HasLoadedChildrenProperty = DependencyProperty.Register(
            "HasLoadedChildren",
            typeof(bool),
            typeof(TvItem),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool?),
            typeof(TvItem),
            new PropertyMetadata(false, OnIsSelectedChanged));

        static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TvItem)d).OnIsExpandedChanged();
        }

        static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TvItem)d).OnPropertyChanged("IsSelected");
        }

        /// <summary>
        /// 卸载标志对象
        /// </summary>
        internal static TvItem UnloadFlagItem = new TvItem(null, Dot.UnloadFlag, null);
        #endregion

        #region 成员变量
        Tv _owner;
        TvItemExpandedState _expandedState;
        Button _btnExpanded;
        bool _isExpandedAll;
        #endregion

        #region 构造方法
        public TvItem(Tv p_tv, object p_data, TvItem p_parent)
            : base(p_data)
        {
            _owner = p_tv;
            Parent = p_parent;
            Depth = (p_parent == null) ? 0 : p_parent.Depth + 1;
            Children = new List<TvItem>();
        }
        #endregion

        /// <summary>
        /// 获取子级节点集合
        /// </summary>
        public List<TvItem> Children { get; }

        /// <summary>
        /// 获取当前节点的父节点
        /// </summary>
        public TvItem Parent { get; private set; }

        /// <summary>
        /// 获取当前节点距离根节点的深度
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// 获取设置当前节点是否已展开
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// 获取设置是否已加载子节点，动态加载节点时有效
        /// </summary>
        public bool HasLoadedChildren
        {
            get { return (bool)GetValue(HasLoadedChildrenProperty); }
            set { SetValue(HasLoadedChildrenProperty, value); }
        }

        /// <summary>
        /// 获取当前节点是否为选择状态
        /// </summary>
        public bool? IsSelected
        {
            get { return (bool?)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// 获取展开/折叠按钮
        /// </summary>
        public object ExpandedUI
        {
            get
            {
                // 不可缓存重用！
                _btnExpanded = null;
                if (_expandedState == TvItemExpandedState.Expanded || _expandedState == TvItemExpandedState.NotExpanded)
                {
                    _btnExpanded = new Button { Style = Res.字符按钮, Margin = new Thickness(0, 0, 0, 1) };
                    _btnExpanded.Click += OnExpandBtnClick;
                    _btnExpanded.DoubleTapped += OnExpandBtnDoubleTapped;
                    _btnExpanded.Content = (_expandedState == TvItemExpandedState.Expanded) ? "\uE013" : "\uE011";
                    return _btnExpanded;
                }

                if (_expandedState == TvItemExpandedState.Loading)
                {
                    return new ProgressRing
                    {
                        IsActive = true,
                        Width = 20,
                        Height = 20,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// 展开从根节点到当前节点的所有节点
        /// </summary>
        public void ExpandAll()
        {
            TvItem parent = Parent;
            while (parent != null)
            {
                parent.IsExpanded = true;
                parent = parent.Parent;
            }
            if (Children.Count > 0 && !IsExpanded)
                IsExpanded = true;
        }

        /// <summary>
        /// 节点的展开状态
        /// </summary>
        internal TvItemExpandedState ExpandedState
        {
            get { return _expandedState; }
            set
            {
                if (_expandedState != value)
                {
                    _expandedState = value;
                    if (_btnExpanded != null
                        && (_expandedState == TvItemExpandedState.Expanded || _expandedState == TvItemExpandedState.NotExpanded))
                    {
                        // 解决点击展开/折叠按钮时切换按钮闪烁的问题
                        _btnExpanded.Content = (_expandedState == TvItemExpandedState.Expanded) ? "\uE013" : "\uE011";
                    }
                    else
                    {
                        // 通知外部绑定刷新
                        OnPropertyChanged("ExpandedUI");
                    }
                }
            }
        }

        /// <summary>
        /// 宿主
        /// </summary>
        internal override IViewItemHost Host => _owner;

        /// <summary>
        /// 单击行
        /// </summary>
        internal void OnClick()
        {
            if (_owner.SelectionMode == SelectionMode.Multiple)
            {
                // 多选时切换选择状态，联动
                _owner.ToggleSelectedCascade(this);
                _owner.OnItemClick(Data, null);
            }
            else
            {
                // 单选
                if (IsSelected.HasValue && IsSelected.Value)
                {
                    _owner.OnItemClick(Data, Data);
                }
                else
                {
                    object old = _owner.SelectedItem;
                    _owner.OnToggleSelected(this);
                    _owner.OnItemClick(Data, old);
                }
            }
        }

        internal void OnDoubleTapped()
        {
            if (_btnExpanded != null)
                IsExpanded = !IsExpanded;
            _owner.OnItemDoubleClick(Data);
        }

        internal void SetExpandState(bool p_isExpanded)
        {
            if (p_isExpanded == IsExpanded)
                return;

            _isExpandedAll = true;
            IsExpanded = p_isExpanded;
            _isExpandedAll = false;
        }

        /// <summary>
        /// 切换 IsExpanded
        /// </summary>
        async void OnIsExpandedChanged()
        {
            // 展开/折叠所有节点
            if (_isExpandedAll)
            {
                if (IsExpanded)
                    ExpandedState = (Children.Count > 0) ? TvItemExpandedState.Expanded : TvItemExpandedState.Hide;
                else
                    ExpandedState = (Children.Count > 0) ? TvItemExpandedState.NotExpanded : TvItemExpandedState.Hide;
                return;
            }

            if (IsExpanded)
            {
                // 展开
                if (_owner.IsDynamicLoading && !HasLoadedChildren)
                {
                    // 动态加载子节点
                    ExpandedState = TvItemExpandedState.Loading;
                    await _owner.OnLoadingChild(this);
                }
                ExpandedState = (Children.Count > 0) ? TvItemExpandedState.Expanded : TvItemExpandedState.Hide;
            }
            else
            {
                // 折叠
                ExpandedState = (Children.Count > 0) ? TvItemExpandedState.NotExpanded : TvItemExpandedState.Hide;
            }
            _owner.RootItems.Invalidate();
        }

        internal void Unload()
        {
            ((ILvCleaner)this).Unload();
            if (Children.Count > 0)
                Children.Clear();
            if (_btnExpanded != null)
            {
                _btnExpanded.Click -= OnExpandBtnClick;
                _btnExpanded.DoubleTapped -= OnExpandBtnDoubleTapped;
            }
            Parent = null;
        }

        void OnExpandBtnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void OnExpandBtnClick(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }
    }

    /// <summary>
    /// 节点展开状态
    /// </summary>
    internal enum TvItemExpandedState
    {
        Hide,

        NotExpanded,

        Loading,

        Expanded,
    }
}