﻿#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 流程功能代理类
    /// </summary>
    public static class AtWf
    {
        /// <summary>
        /// 打开流程表单窗口（创建、编辑或浏览表单）
        /// </summary>
        /// <param name="p_info">流程表单描述信息</param>
        public static async void ShowForm(WfFormInfo p_info)
        {
            Throw.IfNull(p_info, "流程表单描述信息不可为空！");

            if (!AtSys.IsPhoneUI)
            {
                // 因p_info.Init耗时，先激活已打开的窗口，AtApp.OpenWin中也有判断
                foreach (var win in Desktop.Inst.Items)
                {
                    if (win.Params != null && win.Params.Equals(p_info))
                    {
                        Desktop.Inst.ActiveWin(win);
                        return;
                    }
                }
            }

            await p_info.Init();
            p_info.Form = (IWfForm)AtApp.OpenWin(p_info.FormType, null, Icons.None, p_info);
        }

        #region Wf
        /// <summary>
        /// 获取活动可迁移的后续活动的接收者列表
        /// </summary>
        /// <param name="p_atvID">当前活动模板标识</param>
        /// <param name="p_prciID">流程实例标识</param>
        /// <returns></returns>
        public static Task<Dict> GetNextRecvs(long p_atvID, long p_prciID)
        {
            return new UnaryRpc(
                "cm",
                "Wf.GetNextRecvs",
                p_atvID,
                p_prciID
            ).Call<Dict>();
        }
        #endregion
    }
}
