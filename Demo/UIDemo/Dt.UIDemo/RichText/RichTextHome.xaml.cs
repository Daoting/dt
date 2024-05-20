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
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public sealed partial class RichTextHome : Win
    {
        public RichTextHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("Html", typeof(HtmlBoxDemo), Icons.Html) { Desc = "Html格式富文本的编辑及浏览" },
                new Nav("Markdown", typeof(MarkdownBoxDemo), Icons.Ppt) { Desc = "Markdown格式富文本的编辑及浏览" },
                new Nav("富文本格", typeof(RichTextCellDemo), Icons.Word) { Desc = "CHtml CMarkdown格" },

            };
        }
    }
}
