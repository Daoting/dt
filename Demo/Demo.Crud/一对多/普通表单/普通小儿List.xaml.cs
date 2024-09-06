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
    using A = 小儿X;
    
    public partial class 普通小儿List : List
    {
        public 普通小儿List()
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
                _lv.Data = await A.Query($"where group_id={_parentID}");
            }
            else
            {
                _lv.Data = null;
            }
            Menu["增加"].IsEnabled = _parentID > 0;
        }
    }
}