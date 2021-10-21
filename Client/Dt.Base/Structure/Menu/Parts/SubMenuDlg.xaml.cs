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

            Background = Res.浅灰1;
            MinWidth = 160;
            HideTitleBar = true;
            Resizeable = false;
            // 不向下层对话框传递Press事件
            AllowRelayPress = false;

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
                    if (_mi.GetBounds().Right + MinWidth > Kit.ViewWidth)
                        WinPlacement = DlgPlacement.TargetOuterLeftTop;
                    else
                        WinPlacement = DlgPlacement.TargetTopRight;
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
                    else if (_mi.GetBounds().Right + MinWidth > Kit.ViewWidth)
                        WinPlacement = DlgPlacement.TargetOuterLeftTop;
                    else
                        WinPlacement = DlgPlacement.TargetTopRight;
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
        /// 点击对话框外部时
        /// </summary>
        /// <param name="p_point">外部点击位置</param>
        protected override void OnOuterPressed(Point p_point)
        {
            if (Kit.IsPhoneUI)
            {
                base.OnOuterPressed(p_point);
            }
            else
            {
                // 在空白处点击关闭所有菜单项对话框
                _mi.Owner?.Close();
            }
        }
    }
}
