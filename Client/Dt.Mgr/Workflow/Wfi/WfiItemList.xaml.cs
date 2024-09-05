#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-24 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Workflow
{
    using A = WfiItemX;
    
    public partial class WfiItemList : List
    {
        public WfiItemList()
        {
            InitializeComponent();
        }
        
        protected override async Task OnQuery()
        {
            if (_parentID > 0)
            {
                _lv.Data = await A.Query($"where atvi_id={_parentID}");
            }
            else
            {
                _lv.Data = null;
            }
        }
    }
}