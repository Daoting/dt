#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.App.Workflow;
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
        public static async void OpenFormWin(WfFormInfo p_info)
        {
            Throw.IfNull(p_info, "流程表单描述信息不可为空！");

            if (!Kit.IsPhoneUI)
            {
                // 因p_info.Init耗时，先激活已打开的窗口，Kit.OpenWin中也有判断
                foreach (var win in Desktop.Inst.Items)
                {
                    if (win.GetType() == typeof(WfFormWin)
                        && win.Params != null
                        && win.Params.Equals(p_info))
                    {
                        Desktop.Inst.ActiveWin(win);
                        return;
                    }
                }
            }

            await p_info.Init();
            p_info.FormWin = (WfFormWin)Kit.OpenWin(typeof(WfFormWin), p_info.PrcInst.Name, Icons.None, p_info);
        }

        /// <summary>
        /// 创建流程表单窗口
        /// </summary>
        /// <param name="p_info">流程表单描述信息</param>
        /// <returns></returns>
        public static async Task<WfFormWin> CreateFormWin(WfFormInfo p_info)
        {
            await p_info.Init();
            var win = new WfFormWin(p_info);
            p_info.FormWin = win;
            return win;
        }

        /// <summary>
        /// 查看日志(流程图)
        /// </summary>
        /// <param name="p_prcID">流程实例id</param>
        /// <param name="p_prciID">流程模板id，-1时内部查询</param>
        public static void ShowLog(long p_prciID, long p_prcID = -1)
        {
            new WfLogDlg().Show(p_prciID, p_prcID);
        }

        /// <summary>
        /// 为Lv增加默认上下文菜单
        /// </summary>
        /// <param name="p_lv"></param>
        public static void AddMenu(Lv p_lv)
        {
            Throw.IfNull(p_lv);
            p_lv.ItemDoubleClick += (s, e) => OpenFormWin(new WfFormInfo(((Row)e).Long("id")));

            Menu menu = new Menu();
            var mi = new Mi { ID = "查看表单", Icon = Icons.全选 };
            mi.Click += (s, e) => OpenFormWin(new WfFormInfo(e.Row.Long("id")));
            menu.Items.Add(mi);

            mi = new Mi { ID = "日志", Icon = Icons.审核 };
            mi.Click += (s, e) => ShowLog(e.Row.Long("id"));
            menu.Items.Add(mi);
            Ex.SetMenu(p_lv, menu);
        }

    }
}
