#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 物资目录X;
    
    public partial class 物资目录List : List
    {
        public 物资目录List()
        {
            InitializeComponent();
            Menu = CreateMenu(del:false);
            _lv.SetMenu(CreateContextMenu());
        }

        protected override async Task OnQuery()
        {
            if (_parentID > 0)
            {
                _lv.Data = await A.View1.Query($"where 分类id={_parentID}");
            }
            else if ( _clause != null)
            {
                var par = await _clause.Build<A>(false);
                _lv.Data = await A.View1.Query(par.Sql, par.Params);
            }
        }
    }
}