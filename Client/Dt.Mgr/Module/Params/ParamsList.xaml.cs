#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Module
{
    public partial class ParamsList : List
    {
        public ParamsList()
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
                _lv.Data = await ParamsX.Query(null);
            }
            else
            {
                var par = await _clause.Build<ParamsX>();
                _lv.Data = await ParamsX.Query(par.Sql, par.Params);
            }
        }
    }
}