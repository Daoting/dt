#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Demo.UI
{
    public partial class RichTextCellDemo : Win
    {
        public RichTextCellDemo()
        {
            InitializeComponent();
            _fv.Changed += (e) => CellDemoKit.OnChanged(_fv, e);
            _fv.Data = new Row
            {
                { "html", _initHtml },
                { "md",  _initText },
            };
        }

        const string _initHtml = @"<p><span style=""font-size: 24px;"">初始内容</span></p><ol><li><span style=""font-size: 14px;""><strong>粗体</strong></span></li><li><s>删除线</s></li></ol>";

        const string _initText = @"事实上, 编写 Web 内容很麻烦. [WYSIWYG]^(所见即所得) 编辑器帮助减轻了这一任务. 但通常会导致代码太糟, 或更糟糕的是, 网页也会很丑.

没有通常伴随的所有复杂和丑陋的问题, **Markdown** 是一种更好的生成 **HTML** 内容的方式.

一些主要好处是:

1. Markdown 简单易学, 几乎没有多余的字符, 因此编写内容也更快.
2. 用 Markdown 书写时出错的机会更少.
3. 可以产生有效的 XHTML 输出.
4. 将内容和视觉显示保持分开, 这样就不会打乱网站的外观.
5. 可以在你喜欢的任何文本编辑器或 Markdown 应用程序中编写内容.
6. Markdown 使用起来很有趣!

John Gruber, Markdown 的作者如是说:

> Markdown 格式的首要设计目标是更具可读性.
> 最初的想法是 Markdown 格式的文档应当以纯文本形式发布,
> 而不会看起来像被标签或格式说明所标记.
> 虽然 Markdown 的语法受到几种现有的文本到 HTML 转换工具的影响,
> 但 Markdown 语法的最大灵感来源是纯文本电子邮件的格式.";
    }
}