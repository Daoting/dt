#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
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
        internal void OnCellClick(object p_cell)
        {
            if (IsDesignMode)
            {
                if (_rcDesign == null)
                {
                    _rcDesign = new Dlg
                    {
                        IsPinned = true,
                        HideTitleBar = true,
                        BorderBrush = Res.RedBrush,
                        BorderThickness = new Thickness(2),
                        Background = null,
                        WinPlacement = DlgPlacement.TargetOverlap,
                    };
                }
                
                var tgt = p_cell as FrameworkElement;
                if (_rcDesign.PlacementTarget != tgt)
                {
                    _rcDesign.Close();
                    _rcDesign.PlacementTarget = tgt;
                    _rcDesign.Show();
                }
            }
            CellClick?.Invoke(p_cell);
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