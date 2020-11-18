#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 发起任务
    /// </summary>
    public sealed partial class StartWorkflow
    {
        public StartWorkflow()
        {
            InitializeComponent();
        }

        public async void Open()
        {
            _lv.Data = await AtCm.Query<WfdPrc>("流程-所有流程模板");

            if (!AtSys.IsPhoneUI)
            {
                MaxHeight = 400;
                MaxWidth = 300;
            }
            Show();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Close();
            AtKit.RunAsync(() =>
            {
                //WfFormInfo info = new WfFormInfo();
                //DataRow row = _st.SelectedItem;
                //info.PrcDef = AtWf.GetPrcDef(row.Str("id"));
                //AtWf.ShowForm(info);
            });
        }
    }
}
