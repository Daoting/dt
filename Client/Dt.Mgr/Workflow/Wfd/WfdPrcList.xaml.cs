#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-24 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Workflow
{
    using A = WfdPrcX;

    public partial class WfdPrcList : List
    {
        public WfdPrcList()
        {
            InitializeComponent();
            Menu = CreateMenu(del: false);
            Menu.Add("流程实例", Icons.信件, OnWfInst);
            var menu = CreateContextMenu();
            menu.Add("流程图", Icons.双绞线, OnDesign);
            menu.Add("流程实例", Icons.信件, OnWfInst);
            _lv.SetMenu(menu);
        }

        protected override async Task OnQuery()
        {
            if (_clause == null)
            {
                _lv.Data = await A.Query("where 1=1 order by Dispidx");
            }
            else
            {
                var par = await _clause.Build<A>();
                _lv.Data = await A.Query(par.Sql + " order by Dispidx", par.Params);
            }
        }
        
        void OnWfInst(Mi mi)
        {
            var win = Kit.OpenWin(typeof(WfiPrcWin), "流程实例", Icons.信件) as WfiPrcWin;
            if (mi.Data is A x)
                win.Query(x);
        }
        
        void OnDesign(Mi mi)
        {
            var x = mi.Data as A;
            Kit.OpenWin(typeof(WorkflowDesign), x.Name, Icons.双绞线, x.ID);
        }
    }
}