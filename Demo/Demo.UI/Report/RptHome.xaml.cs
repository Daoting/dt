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

namespace Demo.UI
{
    public sealed partial class RptHome : Win
    {
        public RptHome()
        {
            InitializeComponent();
            _nav.Data = Dir;
        }

        public static Nl<Nav> Dir { get; } = new Nl<Nav>
        {
            new Nav("报表浏览器", typeof(RptTabDemo), Icons.折线图) { Desc = "支持Excel Pdf两种格式的报表浏览、导出、打印" },
            new Nav("使用RptTab报表预览", typeof(RptPreview), Icons.靠近) { Desc = "所有不同报表项的绘制、导出、打印" },
            new Nav("在新窗口中报表预览", typeof(RptPreviewInWin), Icons.打开新窗口) { Desc = "在新窗口中进行所有不同报表项的绘制、导出、打印" },
            new Nav("报表模板编辑", typeof(RptTemplateEditor), Icons.Excel) { Desc = "报表模板编辑器的使用" },
            new Nav("报表参数", typeof(RptNotice), Icons.城市) { Desc = "报表查询参数、查询面板、查询方式" },
            new Nav("报表数据源", typeof(RptNotice), Icons.数据库) { Desc = "报表数据源定义、预览" },
        };
    }
}
