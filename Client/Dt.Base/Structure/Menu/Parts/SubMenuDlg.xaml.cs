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

            if (AtSys.IsPhoneUI)
            {
                PhonePlacement = (_mi.ParentMi == null) ? DlgPlacement.TargetBottomLeft : DlgPlacement.FromBottom;
            }
            else
            {
                // 一级放左下方，多级默认放左侧，不能完全显示放右侧
                if (_mi.ParentMi == null && !_mi.Owner.IsContextMenu)
                    WinPlacement = DlgPlacement.TargetBottomLeft;
                else if (_mi.GetBounds(null).Right + MinWidth > SysVisual.ViewWidth)
                    WinPlacement = DlgPlacement.TargetOuterLeftTop;
                else
                    WinPlacement = DlgPlacement.TargetTopRight;
            }

            Closed -= OnClosed;
            if (_mi.ParentMi == null || AtSys.IsPhoneUI)
                Closed += OnClosed;
            Show();
        }

        void OnClosed(object sender, EventArgs e)
        {
            _mi.IsSelected = false;
            if (!_mi.Owner.IsContextMenu)
                _mi.Owner.Close();
        }

        void OnItemsChanged(ItemList<Mi> sender, ItemListChangedArgs e)
        {
            _panel.Children.Clear();
        }
    }
}
