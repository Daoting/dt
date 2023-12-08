#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
#endregion

namespace Dt.Mgr.Workflow
{
    [View(LobViews.流程设计)]
    public partial class WfTemplateMgr : Win
    {
        public WfTemplateMgr()
        {
            InitializeComponent();
            LoadAll();
        }

        async void LoadAll()
        {
            _lv.Data = await WfdPrcX.Query("where 1=1 order by Dispidx");
        }

        async void OnSearch(string e)
        {
            if (e == "#全部")
            {
                LoadAll();
            }
            else if (!string.IsNullOrEmpty(e))
            {
                _lv.Data = await WfdPrcX.Query($"where name like '%{e}%'");
            }
            NaviTo("列表");
        }

        void OnAdd(Mi e)
        {
            EditTemplate("新流程", -1);
        }

        void OnItemClick(ItemClickArgs e)
        {
            var prc = e.Data.To<WfdPrcX>();
            EditTemplate(prc.Name, prc.ID);
        }

        void OnEditTemplateContext(Mi e)
        {
            var prc = e.Data.To<WfdPrcX>();
            EditTemplate(prc.Name, prc.ID);
        }

        void EditTemplate(string p_title, long p_id)
        {
            Kit.OpenWin(typeof(WorkflowDesign), p_title, Icons.修改, p_id);
        }

        async void OnDelContext(Mi e)
        {
            var p_prc = e.Data.To<WfdPrcX>();
            if (!await Kit.Confirm($"确认要删除流程模板[{p_prc.Name}]吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (await p_prc.Delete())
            {
                LoadAll();
            }
        }

        void OnToSearch(Mi e)
        {
            NaviTo("搜索");
        }

        void OnWfInst(Mi e)
        {
            Kit.OpenWin(typeof(WfInstMgr), "流程实例", Icons.信件);
        }
    }
}