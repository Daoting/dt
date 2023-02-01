#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-25 创建
**************************************************************************/
#endregion

#region 命名空间
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 发送命令
    /// </summary>
    public class WfSendCmd : BaseCommand
    {
        WfFormInfo _info;

        public WfSendCmd(WfFormInfo p_info)
        {
            _info = p_info;
            AllowExecute = true;
        }

        protected override void DoExecute(object p_parameter)
        {
            _info.RunCmd(WfiDs.Me.Send);
        }
    }
}
