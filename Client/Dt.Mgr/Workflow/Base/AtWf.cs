#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Mgr.Workflow;
using Dt.Base;
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 流程功能代理类
    /// </summary>
    public static class AtWf
    {
        /// <summary>
        /// 打开流程表单窗口（创建、编辑或浏览表单），参数优先级：
        /// 1. itemID > 0 时，其余两项无效，以当前工作项为标准
        /// 2. prciID > 0 时，以该流程实例的最后工作项为标准
        /// 3. 提供流程名称时，创建新工作项、流程实例、起始活动实例
        /// </summary>
        /// <param name="p_itemID">工作项标识</param>
        /// <param name="p_prciID">流程实例标识</param>
        /// <param name="p_prcName">流程名称</param>
        public static async void OpenFormWin(long p_itemID = -1, long p_prciID = -1, string p_prcName = null)
        {
            WfFormInfo info = new WfFormInfo();
            await info.Init(p_itemID, p_prciID, p_prcName);

            // 需要在窗口内部调用 info.SetForm() ！
            if (info.FormType.IsSubclassOf(typeof(Win)))
            {
                info.FormWin = (Win)Kit.OpenWin(info.FormType, info.PrcInst.Name, Icons.None, info);
            }
            else
            {
                info.FormWin = (WfFormWin)Kit.OpenWin(typeof(WfFormWin), info.PrcInst.Name, Icons.None, info);
            }
        }

        /// <summary>
        /// 查看日志(流程图)
        /// </summary>
        /// <param name="p_prcID">流程实例id</param>
        /// <param name="p_prciID">流程模板id，-1时内部查询</param>
        public static Dlg ShowLog(long p_prciID, long p_prcID = -1)
        {
            var log = new WfLog();
            log.Show(p_prciID, p_prcID);

            Dlg dlg = new Dlg { IsPinned = true };
            if (!Kit.IsPhoneUI)
            {
                dlg.Height = 700;
                dlg.Width = 500;
            }
            dlg.LoadTab(log);
            dlg.Show();
            return dlg;
        }

        /// <summary>
        /// 为Lv增加默认上下文菜单
        /// </summary>
        /// <param name="p_lv"></param>
        public static void AddMenu(Lv p_lv)
        {
            //Throw.IfNull(p_lv);
            //p_lv.ItemDoubleClick += (e) => OpenFormWin(new WfFormInfo(((Row)e).Long("id")));

            //Menu menu = new Menu();
            //var mi = new Mi { ID = "查看表单", Icon = Icons.全选 };
            //mi.Click += (s, e) => OpenFormWin(new WfFormInfo(e.Row.Long("id")));
            //menu.Items.Add(mi);

            //mi = new Mi { ID = "日志", Icon = Icons.审核 };
            //mi.Click += (s, e) => ShowLog(e.Row.Long("id"));
            //menu.Items.Add(mi);
            //Ex.SetMenu(p_lv, menu);
        }

    }
}
