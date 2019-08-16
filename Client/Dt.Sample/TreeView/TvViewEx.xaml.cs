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
            _tv.ViewEx = typeof(TvViewEx1);
            _tv.Data = TvData.GetTbl();
        }
    }

    public class TvViewEx1
    {
        public static TextBlock Icon(TvItem p_item)
        {
            return new TextBlock
            {
                Style = AtRes.LvTextBlock,
                FontFamily = AtRes.IconFont,
                TextAlignment = TextAlignment.Center,
                Text = (p_item.Children.Count > 0) ? "\uE016" : "\uE004",
            };
        }

        public static TextBlock RowColor(TvItem p_item)
        {
            var tb = new TextBlock
            {
                Style = AtRes.LvTextBlock,
                Text = $" ({p_item.Row.Str("code")})",
            };
            string code = p_item.Row.Str("code");
            if (code.Length < 4)
                p_item.Foreground = AtRes.RedBrush;
            else if (code.Length > 4)
                p_item.Foreground = AtRes.GreenBrush;

            if (p_item.Children.Count > 4)
                p_item.Background = AtRes.浅黄背景;
            return tb;
        }
    }
}