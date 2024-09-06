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
    using A = 基础X;
    
    public partial class 树形List : List
    {
        public 树形List()
        {
            InitializeComponent();
            Menu = CreateMenu();
            _lv.AddMultiSelMenu(Menu);
            Ex.SetMenu(_lv, CreateContextMenu());
        }

        protected override async Task OnQuery()
        {
            if (_clause == null && _parentID <= 0)
            {
                _lv.Data = await A.Query(null);
            }
            else if (_clause != null)
            {
                var par = await _clause.Build<A>();
                _lv.Data = await A.Query(par.Sql, par.Params);
            }
            else
            {
                _lv.Data = await A.Query("where parent_id=" + _parentID);
            }
        }
    }
}