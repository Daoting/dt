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

namespace Dt.App.Workflow
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
            _info.RunCmd(ToggleAccept);
        }

        async Task ToggleAccept()
        {
            if (_info.WorkItem.IsAccept)
            {
                _info.WorkItem.IsAccept = false;
                _info.WorkItem.AcceptTime = null;
                if (await AtCm.Save(_info.WorkItem, false))
                    Kit.Msg("已取消签收！");
            }
            else
            {
                _info.WorkItem.IsAccept = true;
                _info.WorkItem.AcceptTime = Kit.Now;
                if (await AtCm.Save(_info.WorkItem, false))
                    Kit.Msg("已签收！");
            }
        }
    }
}
