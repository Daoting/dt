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

namespace Dt.App.Workflow
{
    [View("流程设计")]
    public partial class WorkflowMgr : Win
    {
        public WorkflowMgr()
        {
            InitializeComponent();
            LoadAll();
        }

        async void LoadAll()
        {
            _lv.Data = await AtCm.Query<WfdPrc>("流程-所有流程模板");
        }

        async void OnSearch(object sender, string e)
        {
            if (e == "#全部")
            {
                LoadAll();
            }
            else if (e == "#最近修改")
            {
                _lv.Data = await AtCm.Query<WfdPrc>("流程-最近修改");
            }
            else if (!string.IsNullOrEmpty(e))
            {
                _lv.Data = await AtCm.Query<WfdPrc>("流程-模糊查询", new { input = $"%{e}%" });
            }
            SelectTab("列表");
        }

        void OnAdd(object sender, Mi e)
        {
            EditTemplate("新流程", -1);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            var prc = e.Data.To<WfdPrc>();
            EditTemplate(prc.Name, prc.ID);
        }

        void OnEditTemplateContext(object sender, Mi e)
        {
            var prc = e.Data.To<WfdPrc>();
            EditTemplate(prc.Name, prc.ID);
        }

        void EditTemplate(string p_title, long p_id)
        {
            AtApp.OpenWin(typeof(WorkflowDesign), p_title, Icons.双绞线, p_id);
        }

        async void OnDelContext(object sender, Mi e)
        {
            var p_prc = e.Data.To<WfdPrc>();
            if (!await AtKit.Confirm($"确认要删除流程模板[{p_prc.Name}]吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (await AtCm.Delete(p_prc))
            {
                LoadAll();
            }
        }

        void OnRefreshModel(object sender, Mi e)
        {
            ModelKit.UpdateModel();
        }
    }
}