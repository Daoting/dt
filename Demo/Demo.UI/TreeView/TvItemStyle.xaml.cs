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
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public partial class TvItemStyle : Win
    {
        public TvItemStyle()
        {
            InitializeComponent();
            _tv.Data = TvData.GetTbl();

            _tv.ItemStyle = (e) =>
            {
                string code = e.Row.Str("code");
                if (code.Length < 4)
                    e.Foreground = Res.RedBrush;
                else if (code.Length > 4)
                    e.Foreground = Res.GreenBrush;

                if (e.Children.Count > 4)
                    e.Background = Res.浅黄;
            };
        }
    }

    [LvCall]
    public class TvItemStyleUI
    {
        public static void 图标(Env e)
        {
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                FontFamily = Res.IconFont,
                TextAlignment = TextAlignment.Center,
            };
            e.UI = tb;
            e.Set += c =>
            {
                var tbl = (ITreeData)c.Row.Table;
                var child = tbl.GetTreeItemChildren(c.Row);
                if (child != null && child.Any())
                {
                    tb.Text = "\uE067";
                }
                else
                {
                    tb.Text = "\uE002";
                }
            };
        }
    }
}