#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021/8/2 9:56:23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;

#endregion

namespace Dt.Sample
{
    public partial class GuideDemo : Dlg
    {
        public GuideDemo()
        {
            InitializeComponent();

            HideTitleBar = true;
            WinPlacement = DlgPlacement.Maximized;

            for (int i = 0; i < _fv.Items.Count - 1; i++)
            {
                _sp.Children.Add(new Ellipse { Width = 12, Height = 12, Fill = Res.WhiteBrush, Margin = new Thickness(10) });
            }
            ((Ellipse)_sp.Children[0]).Fill = Res.RedBrush;
            _fv.SelectionChanged += OnSelectionChanged;
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < _sp.Children.Count; i++)
            {
                var item = (Ellipse)_sp.Children[i];
                if (i == _fv.SelectedIndex)
                    item.Fill = Res.RedBrush;
                else
                    item.Fill = Res.WhiteBrush;
            }

            if (_fv.SelectedIndex == _fv.Items.Count - 1)
            {
                Close();
            }
        }

        void OnTest(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}