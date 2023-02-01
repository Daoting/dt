#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 删除流程实例命令
    /// </summary>
    public class WfDeleteCmd : BaseCommand
    {
        WfFormInfo _info;

        public WfDeleteCmd(WfFormInfo p_info)
        {
            _info = p_info;
            AllowExecute = true;
        }

        protected override void DoExecute(object p_parameter)
        {
            _info.RunCmd(WfiDs.Me.Delete);
        }
    }
}
