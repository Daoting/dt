#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class LvRowView : Win
    {
        public LvRowView()
        {
            InitializeComponent();

            _lv.View = new MyRowView();
            _lv.Data = SampleData.CreatePersonsTbl(50);
        }
    }

    public class MyRowView : IRowView
    {
        public UIElement Create(LvItem p_item)
        {
            return new TextBlock
            {
                Text = p_item.Row.Str("xm"),
                Margin = new Thickness(10),
                Foreground = (p_item.Row.Str("xb") == "男") ? Res.BlackBrush : Res.RedBrush,
            };
        }
    }
}