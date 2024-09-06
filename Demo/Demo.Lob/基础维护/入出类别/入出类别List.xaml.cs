#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 物资入出类别X;
    
    public partial class 入出类别List : List
    {
        public 入出类别List()
        {
            InitializeComponent();
            //Menu = CreateMenu(del:false);
            //_lv.SetMenu(CreateContextMenu());
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