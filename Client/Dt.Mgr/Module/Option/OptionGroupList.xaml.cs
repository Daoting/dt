#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    public partial class OptionGroupList : List
    {
        public OptionGroupList()
        {
            InitializeComponent();
            Menu = CreateMenu();
            _lv.SetMenu(CreateContextMenu());
        }
        
        protected override async Task OnQuery()
        {
            if (_clause == null)
            {
                _lv.Data = await OptionGroupX.Query(null);
            }
            else
            {
                var par = await _clause.Build<OptionGroupX>();
                _lv.Data = await OptionGroupX.Query(par.Sql, par.Params);
            }
        }
    }
}