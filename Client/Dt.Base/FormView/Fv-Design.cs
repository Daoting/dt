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
using Microsoft.UI.Xaml.Markup;
using System.Text;
using System.Xml;
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

        #region Xaml
        /// <summary>
        /// 导出Fv的xaml
        /// </summary>
        /// <returns></returns>
        public string ExportXaml()
        {
            var sb = new StringBuilder();
            using (XmlWriter xw = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true }))
            {
                // 可能为QueryFv
                xw.WriteStartElement("a", GetType().Name, "using:Dt.Base");
                xw.WriteAttributeString("xmlns", "x", null, "http://schemas.microsoft.com/winfx/2006/xaml");
                
                // Kit.LoadXaml已自动添加
                //xw.WriteStartElement("a", GetType().Name, "using:Dt.Base");
                //xw.WriteAttributeString("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                //xw.WriteAttributeString("xmlns", "x", null, "http://schemas.microsoft.com/winfx/2006/xaml");

                foreach (var obj in Items)
                {
                    if (obj is FvCell cell)
                        cell.ExportXaml(xw);
                    else if (obj is CBar bar)
                        bar.ExportXaml(xw);
                }

                xw.WriteEndElement();
                xw.Flush();
            }

            // 去掉冗余的命名空间，Kit.LoadXaml已自动添加
            string str = sb.ToString();
            int idx = str.IndexOf(' ');
            int idx2 = str.IndexOf('>');
            if (str[idx2 - 1] == '/')
                idx2--;
            return str.Remove(idx, idx2 - idx);
        }
        #endregion

        /// <summary>
        /// 设计模式时删除当前选择的的单元格
        /// </summary>
        public void DelDesignCell()
        {
            if (_rcDesign != null && _rcDesign.IsOpened)
            {
                Items.Remove(_rcDesign.PlacementTarget);
                _rcDesign.PlacementTarget = null;
                _rcDesign.Close();
            }
        }

        /// <summary>
        /// 设计模式时清除当前选择的的单元格
        /// </summary>
        public void ClearDesignCell()
        {
            if (_rcDesign != null && _rcDesign.IsOpened)
            {
                _rcDesign.PlacementTarget = null;
                _rcDesign.Close();
            }
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
                        TopMost = true,
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
            // _panel.Children和Items个数不相同！末尾多边框
            int tgt = Items.Count - 1;
            for (int i = 0; i < Items.Count; i++)
            {
                var item = _panel.Children[i] as FrameworkElement;
                if (item.ContainPoint(e))
                {
                    tgt = i;
                    break;
                }
            }

            int src = _panel.Children.IndexOf(_rcDesign.PlacementTarget);
            if (src == tgt)
                return;

            Items.ItemsChanged -= OnItemsChanged;

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