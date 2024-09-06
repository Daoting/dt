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
    using A = 物资分类X;
    
    public partial class 物资分类List : List
    {
        public 物资分类List()
        {
            InitializeComponent();
            Menu = CreateMenu();
            _lv.SetMenu(CreateContextMenu());
            _lv.FilterCfg = new FilterCfg
            {
                FilterCols = "名称",
                EnablePinYin = true,
                Placeholder = "文字或拼音简码",
                IsRealtime = true,
            };
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