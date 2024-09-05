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
    public partial class OptionGroupOptionList : List
    {
        public OptionGroupOptionList()
        {
            InitializeComponent();
            Menu = CreateMenu();
            _lv.AddMultiSelMenu(Menu);
            _lv.SetMenu(CreateContextMenu());
        }
        
        protected override async Task OnQuery()
        {
            if (_parentID > 0)
            {
                _lv.Data = await OptionX.Query($"SELECT a.*,b.Name as GroupName FROM cm_option a, cm_option_group b where a.group_id=b.ID and a.group_id={_parentID} order by Dispidx");
            }
            else
            {
                _lv.Data = null;
            }
            Menu["增加"].IsEnabled = _parentID > 0;
        }
    }
}