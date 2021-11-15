#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class IconDemo : Win
    {
        public IconDemo()
        {
            InitializeComponent();
            _lv.Data = IconItem.GetAllIcons();
            _lv.Filter = OnFilter;
        }

        void OnSearch(object sender, string e)
        {
            _lv.Refresh();
        }

        bool OnFilter(object p_obj)
        {
            return ((IconItem)p_obj).IsMatched(_sb.Text);
        }

        void OnCopyIcons(object sender, Mi e)
        {
            ToClipboard($"Icons.{((IconItem)e.Data).Name}");
        }

        void OnCopyName(object sender, Mi e)
        {
            ToClipboard(((IconItem)e.Data).Name);
        }

        void OnCopyButton(object sender, Mi e)
        {
            var icon = (IconItem)e.Data;
            ToClipboard($"<Button Content=\"&#x{icon.Hex};\" Style=\"{{StaticResource 字符按钮}}\" />");
        }

        void OnCopyBlock(object sender, Mi e)
        {
            var icon = (IconItem)e.Data;
            ToClipboard($"<TextBlock Text=\"&#x{icon.Hex};\" FontFamily=\"{{StaticResource IconFont}}\" />");
        }

        void ToClipboard(string p_content)
        {
            DataPackage data = new DataPackage();
            data.SetText(p_content);
            Clipboard.SetContent(data);
            Kit.Msg(string.Format("已复制到剪贴板：\r\n{0}", p_content));
        }

    }
}
