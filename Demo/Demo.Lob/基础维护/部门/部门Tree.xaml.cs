#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 部门X;
    
    public partial class 部门Tree : Tree
    {
        public 部门Tree()
        {
            InitializeComponent();
            Menu = CreateMenu();
            _tv.FilterCfg = new FilterCfg
            {
                FilterCols = "名称",
                EnablePinYin = true,
                Placeholder = "文字或拼音简码",
                IsRealtime = true,
            };
        }
        
        public void SelectByID(long p_id)
        {
            _tv.SelectByID(p_id);
        }
        
        protected override void OnFirstLoaded()
        {
            _tv.FixedRoot = new A(ID: 0, 名称: "所有部门");
            _ = Refresh();
        }

        protected override async Task OnQuery()
        {
            _tv.Data = await A.Query("where 1=1 order by 编码");
        }
    }
}