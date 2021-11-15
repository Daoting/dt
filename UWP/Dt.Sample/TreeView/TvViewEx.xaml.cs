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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class TvViewEx : Win
    {
        public TvViewEx()
        {
            InitializeComponent();
            _tv.CellEx = typeof(TvViewEx1);
            _tv.Data = TvData.GetTbl();
        }
    }

    public class TvViewEx1
    {
        public static TextBlock Icon(TvItem p_item)
        {
            return new TextBlock
            {
                Style = Res.LvTextBlock,
                FontFamily = Res.IconFont,
                TextAlignment = TextAlignment.Center,
                Text = (p_item.Children.Count > 0) ? "\uE067" : "\uE002",
            };
        }

        public static TextBlock RowColor(TvItem p_item)
        {
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                Text = $" ({p_item.Row.Str("code")})",
            };
            string code = p_item.Row.Str("code");
            if (code.Length < 4)
                p_item.Foreground = Res.RedBrush;
            else if (code.Length > 4)
                p_item.Foreground = Res.GreenBrush;

            if (p_item.Children.Count > 4)
                p_item.Background = Res.浅黄;
            return tb;
        }
    }
}