#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.Foundation;
#endregion

namespace Dt.Base.MenuView
{
    public partial class SubMenuDlg : Dlg
    {
        Mi _mi;

        public SubMenuDlg(Mi p_mi)
        {
            InitializeComponent();
            PlacementTarget = p_mi;
            ClipElement = p_mi;
            _mi = p_mi;
            _mi.Items.ItemsChanged += OnItemsChanged;
        }

        public void ShowDlg()
        {
            if (_panel.Children.Count == 0)
            {
                foreach (var mi in _mi.Items)
                {
                    _panel.Children.Add(mi);
                }
            }

            if (_mi.Owner.IsContextMenu)
            {
                if (Kit.IsPhoneUI)
                {
                    PhonePlacement = DlgPlacement.FromBottom;
                }
                else
                {
                    // 默认放右侧，不能完全显示放左侧
                    if (_mi.GetBounds().Right + MinWidth > SysVisual.ViewWidth)
                        WinPlacement = DlgPlacement.TargetOuterLeftTop;
                    else
                        WinPlacement = DlgPlacement.TargetTopRight;

                    // 打开多级后点击空白时一起关闭
                    SysVisual.BlankPressed = OnBlankPressed;
                }
            }
            else
            {
                if (Kit.IsPhoneUI)
                {
                    PhonePlacement = (_mi.ParentMi == null) ? DlgPlacement.TargetBottomLeft : DlgPlacement.FromBottom;
                }
                else
                {
                    // 一级放左下方，多级默认放右侧，不能完全显示放左侧
                    if (_mi.ParentMi == null && !_mi.Owner.IsContextMenu)
                        WinPlacement = DlgPlacement.TargetBottomLeft;
                    else if (_mi.GetBounds().Right + MinWidth > SysVisual.ViewWidth)
                        WinPlacement = DlgPlacement.TargetOuterLeftTop;
                    else
                        WinPlacement = DlgPlacement.TargetTopRight;

                    // 打开多级后点击空白时一起关闭
                    SysVisual.BlankPressed = OnBlankPressed;
                }
            }

            Closed -= OnClosed;
            if (_mi.ParentMi == null || Kit.IsPhoneUI)
                Closed += OnClosed;
            Show();
        }

        void OnClosed(object sender, bool e)
        {
            _mi.IsSelected = false;
            if (!_mi.Owner.IsContextMenu)
                _mi.Owner.Close();
        }

        void OnItemsChanged(object sender, ItemListChangedArgs e)
        {
            _panel.Children.Clear();
        }

        /// <summary>
        /// 在空白处点击(所有对话框外部)
        /// </summary>
        void OnBlankPressed()
        {
            _mi.Owner?.Close();
            SysVisual.BlankPressed = null;
        }

        /// <summary>
        /// 点击对话框外部时
        /// </summary>
        /// <param name="p_point">外部点击位置</param>
        protected override void OnOuterPressed(Point p_point)
        {
            if (Kit.IsPhoneUI)
                base.OnOuterPressed(p_point);

            // WinUI模式不自动关闭
        }
    }
}
