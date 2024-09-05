#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-01 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Mgr.Workflow;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Mgr.Rbac;
using System.Reflection;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 流程表单基类
    /// </summary>
    public abstract partial class WfForm : Form
    {
        public Task Query(WfFormInfo p_info)
        {
            if (p_info == null)
                return Task.CompletedTask;

            return Query(p_info, new LvMsgArgs
            {
                ID = p_info.PrcInst.IsAdded ? -1 : p_info.PrcInst.ID,
                Action = FormAction.Open,
            });
        }

        public override async Task Query(LvMsgArgs p_args)
        {
            if (_args != p_args)
            {
                // id相同，使用上次_info，内部不会再次调用OnRefresh
                await Query(_args?.ID == p_args?.ID ? _info : null, p_args);
            }
        }

        Task Query(WfFormInfo p_info, LvMsgArgs p_args)
        {
            _info = p_info;
            _args = p_args;

            if (_args != null)
            {
                if (_args.Action == FormAction.Open)
                {
                    Show();
                }
                else if (_args.Action == FormAction.Close)
                {
                    Close();
                    return Task.CompletedTask;
                }
            }
            return Refresh();
        }
        
        protected override async Task OnLoading()
        {
            if (_args == null)
                return;

            if (_info == null)
            {
                _info = new WfFormInfo();
                string prcName = null;
                if (_args.ID <= 0)
                {
                    var attr = GetType().GetCustomAttribute<WfFormAttribute>(false);
                    if (attr == null)
                        Throw.Msg($"未指定流程表单类型，请在流程表单类型 [{GetType().Name}] 上添加 [WfForm(\"流程名\")] 标签！");
                    prcName = attr.Alias;
                }
                await _info.Init(-1, _args.ID, prcName);
            }
            _info.Form = this;
        }

    }
}
