#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Crud
{
    using A = 大儿X;
    
    public partial class 父表大儿List : List
    {
        public 父表大儿List()
        {
            InitializeComponent();
        }
        
        protected override async Task OnQuery()
        {
            if (_parentID > 0)
            {
                _lv.Data = await A.Query($"where parent_id={_parentID}");
            }
            else
            {
                _lv.Data = null;
            }
        }
    }
}