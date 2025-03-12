#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 设计模式
    /// </summary>
    public partial class Fv
    {
        #region 静态内容
        public readonly static DependencyProperty IsDesignModeProperty = DependencyProperty.Register(
            "IsDesignMode",
            typeof(bool),
            typeof(Fv),
            new PropertyMetadata(false, OnIsDesignModeChanged));

        static void OnIsDesignModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fv fv = (Fv)d;
            if ((bool)e.NewValue)
            {
                fv.Unloaded += fv.OnFvUnloaded;
            }
            else
            {
                fv.Unloaded -= fv.OnFvUnloaded;
                if (fv._rcDesign != null)
                {
                    fv._rcDesign.Close();
                    fv._rcDesign = null;
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取设置是否为设计模式，默认false，设计模式时点击格显示选中状态、可拖拽格调序
        /// </summary>
        public bool IsDesignMode
        {
            get { return (bool)GetValue(IsDesignModeProperty); }
            set { SetValue(IsDesignModeProperty, value); }
        }

        /// <summary>
        /// 触发内部单元格点击事件
        /// </summary>
        /// <param name="p_cell"></param>
        /// <param name="e"></param>
        internal void OnCellClick(object p_cell, PointerRoutedEventArgs e)
        {
            CellClick?.Invoke(p_cell);
            
            if (IsDesignMode)
            {
                if (_rcDesign == null)
                {
                    _rcDesign = new Dlg
                    {
                        IsPinned = true,
                        HideTitleBar = true,
                        BorderBrush = Res.亮红,
                        BorderThickness = new Thickness(2),
                        Background = null,
                        WinPlacement = DlgPlacement.TargetOverlap,
                    };
                }

                var tgt = p_cell as FrameworkElement;
                if (_rcDesign.PlacementTarget != tgt || !_rcDesign.IsOpened)
                {
                    _rcDesign.Close();
                    _rcDesign.PlacementTarget = tgt;
                    _rcDesign.Show();
                }

                e.StartDrag(OnStopDrag, OnDragging);
            }
        }

        void OnStopDrag(Point e)
        {
            if (!this.ContainPoint(e) || _rcDesign.ContainPoint(e))
                return;

            // uno中的 VisualTreeHelper.FindElementsInHostCoordinates 无值
            int tgt = _panel.Children.Count - 1;
            for (int i = 0; i < _panel.Children.Count; i++)
            {
                var item = _panel.Children[i] as FrameworkElement;
                if (item.ContainPoint(e))
                {
                    tgt = i;
                    break;
                }
            }

            Items.ItemsChanged -= OnItemsChanged;

            var src = _panel.Children.IndexOf(_rcDesign.PlacementTarget);

            // 往前移动插入到目标前面，往后移动插入到目标后面
            _panel.Children.RemoveAt(src);
            if (tgt >= _panel.Children.Count)
                _panel.Children.Add(_rcDesign.PlacementTarget);
            else
                _panel.Children.Insert(tgt, _rcDesign.PlacementTarget);

            var srcCell = Items[src];
            Items.RemoveAt(src);
            if (tgt >= Items.Count)
                Items.Add(srcCell);
            else
                Items.Insert(tgt, srcCell);
            
            Items.ItemsChanged += OnItemsChanged;
            _rcDesign.Close();
        }
        
        void OnDragging(Dlg dlg, Point e)
        {
            if (!this.ContainPoint(e) || _rcDesign.ContainPoint(e))
            {
                dlg.Foreground = Res.深灰2;
            }
            else
            {
                dlg.Foreground = Res.亮红;
            }
        }
        
        void OnFvUnloaded(object sender, RoutedEventArgs e)
        {
            if (_rcDesign != null)
            {
                _rcDesign.Close();
                _rcDesign = null;
            }
        }
        
        Dlg _rcDesign;
    }
}