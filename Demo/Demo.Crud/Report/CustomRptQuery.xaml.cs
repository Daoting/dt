#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Crud
{
    [RptQuery]
    public partial class CustomRptQuery : RptQuery
    {
        public CustomRptQuery()
        {
            InitializeComponent();
        }

        public override QueryFv Fv => _fv;
    }
}