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
    using A = VirX<主表X, 扩展1X, 扩展2X>;
    
    public partial class 虚拟List : List
    {
        public 虚拟List()
        {
            InitializeComponent();
            Menu = CreateMenu();
            _lv.AddMultiSelMenu(Menu);
            _lv.SetMenu(CreateContextMenu());
        }

        protected override async Task OnQuery()
        {
            if (_clause == null)
            {
                _lv.Data = await A.Query(null);
            }
            else
            {
                var par = await _clause.Build<A>();
                _lv.Data = await A.Query(par.Sql, par.Params);
            }
        }
    }
}