#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.App.Workflow;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 查看日志(流程图)命令
    /// </summary>
    public class WfLogCmd : BaseCommand
    {
        WfFormInfo _info;

        public WfLogCmd(WfFormInfo p_info)
        {
            _info = p_info;
            AllowExecute = true;
        }

        protected override void DoExecute(object p_parameter)
        {
            _info.RunCmd(OpenLog);
        }

        Task OpenLog()
        {
            new WfLogDlg().Show(_info.PrcDef.ID, _info.PrcInst.ID);
            return Task.CompletedTask;
        }
    }
}
