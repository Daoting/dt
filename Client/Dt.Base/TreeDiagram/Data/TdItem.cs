#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Shapes;
using Path = Microsoft.UI.Xaml.Shapes.Path;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 树节点视图
    /// </summary>
    public partial class TdItem : ViewItem
    {
        #region 静态内容
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool?),
            typeof(TdItem),
            new PropertyMetadata(false, OnIsSelectedChanged));

        static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TdItem)d).OnPropertyChanged("IsSelected");
        }
        #endregion

        #region 成员变量
        TreeDiagram _owner;
        #endregion

        #region 构造方法
        public TdItem(TreeDiagram p_tv, object p_data, TdItem p_parent)
            : base(p_data)
        {
            _owner = p_tv;
            Parent = p_parent;
            Depth = (p_parent == null) ? 0 : p_parent.Depth + 1;
            Children = new List<TdItem>();
        }
        #endregion

        /// <summary>
        /// 获取子级节点集合
        /// </summary>
        public List<TdItem> Children { get; }

        /// <summary>
        /// 获取当前节点的父节点
        /// </summary>
        public TdItem Parent { get; }

        /// <summary>
        /// 获取当前节点距离根节点的深度
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// 获取当前节点是否为选择状态
        /// </summary>
        public bool? IsSelected
        {
            get { return (bool?)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// 宿主
        /// </summary>
        internal override IViewItemHost Host => _owner;

        /// <summary>
        /// 节点元素
        /// </summary>
        internal UIElement UI { get; set; }

        /// <summary>
        /// 节点下面的竖线
        /// </summary>
        internal Line TopVerLine { get; set; }

        /// <summary>
        /// 节点上面的箭头
        /// </summary>
        internal Path TopArrow { get; set; }

        /// <summary>
        /// 节点下面的竖线
        /// </summary>
        internal Line BottomVerLine { get; set; }

        /// <summary>
        /// 节点下面的水平连线
        /// </summary>
        internal Line HorLinkLine { get; set; }

        /// <summary>
        /// 节点和所有子孙节点占用的最大宽度
        /// </summary>
        internal double TotalWidth { get; set; }

        /// <summary>
        /// 节点左侧所有同层节点的宽度
        /// </summary>
        internal double LeftWidth { get; set; }

        internal void ClearUI()
        {
            UI = null;
            TopVerLine = null;
            BottomVerLine = null;
            HorLinkLine = null;
            TopArrow = null;
        }

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
            _owner.OnItemDoubleClick(Data);
        }
    }
}