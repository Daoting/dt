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
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Sample
{
    public sealed partial class TestDemo1 : Win
    {
        public TestDemo1()
        {
            InitializeComponent();

            //Excel excel = new Excel();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _pnl.InvalidateMeasure();
        }
    }

    public sealed partial class My1 : StackPanel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var my = (My2)Children[1];
            for (int i = 0; i < 5; i++)
            {
                var tb = new Button { Content = i.ToString() };
                my.Children.Add(tb);
                tb.Measure(new Size(40, 100));
            }
            
            return base.MeasureOverride(availableSize);
        }
    }

    public sealed partial class My2 : StackPanel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            Log.Debug("my2");
            foreach (var elem in Children)
            {
                ((Button)elem).Measure(new Size(40, 100));
            }
            return new Size(40, Children.Count * 50);
        }
    }
}