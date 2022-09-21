#region 文件描述
/******************************************************************************
* 创建: daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
#endregion

namespace Dt.Base.ListView
{
    public partial class PullToRefreshDlg : Dlg
    {
        const double _maxY = 120;

        public PullToRefreshDlg(Lv p_owner)
        {
            InitializeComponent();

            HideTitleBar = true;
            Resizeable = false;
            IsPinned = true;
            PhonePlacement = DlgPlacement.TargetOverlap;
            PlacementTarget = p_owner;
            Background = Res.TransparentBrush;
            ShowVeil = false;
        }

        public void ShowInteracting(double p_y)
        {
            double y = p_y - _bd.Height > _maxY ? _maxY : p_y - _bd.Height;
            _bd.Margin = new Thickness(0, y, 0, 0);
            if (y < 0)
                _bd.Clip = new RectangleGeometry { Rect = new Rect(0, -y, _bd.Width, _bd.Height) };
            else
                _bd.Clip = null;

            RotateTransform tran = new RotateTransform();
            tran.Angle = (y / _maxY) * 360;
            tran.CenterY = 10;
            tran.CenterX = 10;
            _tb.RenderTransform = tran;
            if (y >= 40)
                _tb.Foreground = Res.BlackBrush;

            Show();
        }

        public void ShowRefreshing()
        {
            _bdChild.Child = new ProgressRing { Background = Res.WhiteBrush, IsActive = true, Width = 20, Height = 20, };
            _bd.Margin = new Thickness(0, 40, 0, 0);
            Show();
        }
    }
}