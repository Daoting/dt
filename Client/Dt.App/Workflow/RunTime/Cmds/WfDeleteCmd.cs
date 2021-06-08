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
            _info.RunCmd(Delete);
        }

        async Task Delete()
        {
            if (_info.Usage == WfFormUsage.Read)
            {
                Kit.Warn("禁止删除表单！");
                return;
            }

            // 管理时始终可删除
            if (_info.Usage == WfFormUsage.Edit
                && (_info.AtvDef == null || (!_info.AtvDef.CanDelete && _info.AtvDef.Type != WfdAtvType.Start)))
            {
                Kit.Warn("您无权删除当前表单！请回退或发送到其他用户进行删除。");
                return;
            }

            if (!await Kit.Confirm("确认要删除当前表单吗？删除后表单将不可恢复！"))
                return;

            if (await _info.FormWin.Delete())
            {
                if (!_info.PrcInst.IsAdded)
                {
                    if (!await AtCm.Delete(_info.PrcInst, false))
                        Kit.Warn("表单已删除，未找到待删除的流程实例！");
                }
                _info.CloseWin();
            }
        }
    }
}
