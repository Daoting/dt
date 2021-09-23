#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.MenuView
{
    /// <summary>
    /// 菜单项布局面板
    /// </summary>
    public partial class MenuPanel : Panel
    {
        #region 成员变量
        const double _miMoreWidth = 50;
        static Rect _rcEmpty = new Rect();
        Menu _owner;
        Mi _miMore;
        Size _availableSize;
        bool _updateItems;
        #endregion

        /*********************************************************************************************************/
        // MeasureOverride中尽可能不增删Children元素，uno中每增删一个元素会重复一次MeasureOverride，严重时死循环！！！
        // UWP和uno的调用顺序不同！
        // UWP：MeasureOverride > _owner.SizeChanged > SizeChanged > Loaded
        // uno：Loaded > MeasureOverride > SizeChanged > _owner.SizeChanged
        /*********************************************************************************************************/

        internal void SetOwner(Menu p_owner)
        {
            _owner = p_owner;
            if (_owner.IsContextMenu)
            {
                LoadContextItems();
                _owner.Items.ItemsChanged += OnContextItemsChanged;
            }
            else
            {
                _updateItems = true;
                _owner.Items.ItemsChanged += (s, e) => LoadItems();
            }
        }

        /// <summary>
        /// 更新布局
        /// </summary>
        internal void UpdateArrange()
        {
            if (!_owner.IsContextMenu)
                LoadItems();
        }

        #region 重写方法
        protected override Size MeasureOverride(Size availableSize)
        {
            double maxWidth = double.IsInfinity(availableSize.Width) ? Kit.ViewWidth : availableSize.Width;
            double maxHeight = double.IsInfinity(availableSize.Height) ? Kit.ViewHeight : availableSize.Height;
            bool widthChanged = _availableSize.Width != maxWidth;
            _availableSize = new Size(maxWidth, maxHeight);

            // 在宽度变化或外部刷新布局时重新测量，把不可见的菜单项调整到more下！
            if (!_owner.IsContextMenu
                && (widthChanged || _updateItems))
            {
                return MeasureAndResetItems();
            }

            if (Children.Count == 0)
                return new Size();
            return _owner.IsContextMenu ? MeasureContextMenu() : MeasureMenu();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            // 水平/垂直布局
            if (_owner.IsContextMenu)
                ArrangeContextMenu(finalSize);
            else
                ArrangeMenu(finalSize);
            return finalSize;
        }
        #endregion

        #region Menu
        Size MeasureMenu()
        {
            double width = 0;
            double height = 0;
            foreach (Mi mi in Children.OfType<Mi>())
            {
                mi.Measure(_availableSize);
                if (mi.Visibility == Visibility.Visible)
                {
                    width += mi.DesiredSize.Width;
                    if (mi.DesiredSize.Height > height)
                        height = mi.DesiredSize.Height;
                }
            }
            return new Size(width, height);
        }

        void ArrangeMenu(Size p_finalSize)
        {
            double left = 0;
            foreach (Mi mi in Children.OfType<Mi>())
            {
                if (mi.Visibility == Visibility.Visible)
                {
                    mi.Arrange(new Rect(left, 0, mi.DesiredSize.Width, p_finalSize.Height));
                    left += mi.DesiredSize.Width;
                }
                else
                {
                    mi.Arrange(_rcEmpty);
                }
            }
        }
        #endregion

        #region ContextMenu
        void LoadContextItems()
        {
            foreach (var mi in _owner.Items)
            {
                mi.UpdateOwner(_owner, null);
                Children.Add(mi);
            }
        }

        void OnContextItemsChanged(object sender, ItemListChangedArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemInserted)
            {
                Mi mi = _owner.Items[e.Index];
                mi.UpdateOwner(_owner, null);
                Children.Insert(e.Index, mi);
            }
            else if (e.CollectionChange == CollectionChange.ItemRemoved)
            {
                Children.RemoveAt(e.Index);
            }
            else if (e.CollectionChange == CollectionChange.Reset)
            {
                Children.Clear();
                LoadContextItems();
            }
        }

        Size MeasureContextMenu()
        {
            double width = 0;
            double height = 0;
            foreach (var mi in _owner.Items)
            {
                mi.Measure(_availableSize);
                if (mi.Visibility == Visibility.Visible)
                {
                    height += mi.DesiredSize.Height;
                    if (mi.DesiredSize.Width > width)
                        width = mi.DesiredSize.Width;
                }
            }
            return new Size(width, height);
        }

        void ArrangeContextMenu(Size p_finalSize)
        {
            double top = 0;
            foreach (var mi in _owner.Items)
            {
                if (mi.Visibility == Visibility.Visible)
                {
                    mi.Arrange(new Rect(0, top, p_finalSize.Width, mi.DesiredSize.Height));
                    top += mi.DesiredSize.Height;
                }
                else
                {
                    mi.Arrange(_rcEmpty);
                }
            }
        }
        #endregion

        #region 内部方法
        void LoadItems()
        {
            _updateItems = true;
            InvalidateMeasure();
        }

        Size MeasureAndResetItems()
        {
            _updateItems = false;
            Children.Clear();
            if (_miMore != null)
                _miMore.Items.Clear();
            if (_owner.Items.Count == 0 || _availableSize.Width == 0)
                return new Size();

            int index = _owner.Items.Count;
            double width = 0;
            double height = 0;
            double overWidth = _availableSize.Width - _miMoreWidth;

            // 摆满可见区域
            for (int i = 0; i < _owner.Items.Count; i++)
            {
                Mi mi = _owner.Items[i];
                mi.UpdateOwner(_owner, null);
                Children.Add(mi);
                mi.Measure(_availableSize);

                if (mi.Visibility == Visibility.Collapsed)
                    continue;

                width += mi.DesiredSize.Width;
                if (width > _availableSize.Width
                    || (width > overWidth && ExistVisibleMi(i + 1)))
                {
                    // 超过宽度，将后面的项调整到二级
                    // 不超宽度，但超过MoreButton位置，还有项时从此处切分
                    index = i;
                    width -= mi.DesiredSize.Width;
                    Children.RemoveAt(Children.Count - 1);
                    break;
                }

                if (mi.DesiredSize.Height > height)
                    height = mi.DesiredSize.Height;
            }

            // 未放下的添加到more下
            if (index != _owner.Items.Count)
            {
                ResetMoreItems(index);
                _miMore.Measure(_availableSize);
                width += _miMoreWidth;
            }
            return new Size(width, height);
        }

        /// <summary>
        /// 后续菜单是否存在可见项
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        bool ExistVisibleMi(int p_index)
        {
            for (int i = p_index; i < _owner.Items.Count; i++)
            {
                if (_owner.Items[i].Visibility == Visibility.Visible)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 未放下的添加到more下
        /// </summary>
        /// <param name="p_index"></param>
        void ResetMoreItems(int p_index)
        {
            if (_miMore == null)
            {
                _miMore = new Mi
                {
                    Owner = _owner,
                    Icon = Icons.等等,
                    ShowInPhone = VisibleInPhone.Icon,
                };
                ToolTipService.SetToolTip(_miMore, "查看更多");
            }

            for (int i = p_index; i < _owner.Items.Count; i++)
            {
                Mi mi = _owner.Items[i];
                mi.UpdateOwner(_owner, _miMore);
                _miMore.Items.Add(mi);
            }
            Children.Add(_miMore);
        }
        #endregion
    }
}
