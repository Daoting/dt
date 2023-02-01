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
    /// 签收命令
    /// </summary>
    public class WfAcceptCmd : BaseCommand
    {
        WfFormInfo _info;

        public WfAcceptCmd(WfFormInfo p_info)
        {
            _info = p_info;
            AllowExecute = true;
        }

        protected override void DoExecute(object p_parameter)
        {
            _info.RunCmd(WfiDs.Me.ToggleAccept);
        }
    }
}
