#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Dt.Base;
using Dt.Core;
using Dt.Core.Rpc;
using Uno;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Xamarin.Essentials;
#endregion

namespace Dt.Sample
{
    public sealed partial class TestDemo1 : PageWin
    {
        public TestDemo1()
        {
            InitializeComponent();

            Path path = new Path();
            var rc = new PathGeometry();
            path.Data = rc;
            rc.Figures = new PathFigureCollection();
            path.Fill = AtRes.RedBrush;
            Canvas.SetLeft(path, 30);
            Canvas.SetTop(path, 250);
            _cv.Children.Add(path);
        }
    }
}