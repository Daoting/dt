#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public partial class RptTemplateEditor : Win
    {
        public RptTemplateEditor()
        {
            InitializeComponent();
            _btns.Init(2, "报表模板编辑列表");
        }

    }
}