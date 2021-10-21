#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 查看日志
    /// </summary>
    public sealed partial class WfLogDlg : Dlg
    {
        long _prciID;
        TextBlock _tb;

        public WfLogDlg()
        {
            InitializeComponent();
        }

        public async void Show(long p_prciID, long p_prcID)
        {
            _prciID = p_prciID;

            // 查询流程模板id
            if (p_prcID <= 0)
                p_prcID = await AtCm.GetScalar<long>("流程-实例id获取模板id", new { id = p_prciID });

            var def = await WfFormInfo.GetPrcDef(p_prcID);
            if (string.IsNullOrEmpty(def.Diagram))
            {
                Kit.Warn("流程图模板内容为空！");
                return;
            }

            _sketch.ReadXml(def.Diagram);
            var atvs = await AtCm.Query("流程-活动实例状态", new { prciid = p_prciID });
            if (atvs.Count > 0)
            {
                foreach (var node in _sketch.Container.Children.OfType<SNode>())
                {
                    var dr = (from c in atvs
                              where c.Long("atvdid") == node.ID
                              select c).LastOrDefault();
                    if (dr != null)
                    {
                        switch (dr.Str("status"))
                        {
                            //活动
                            case "0":
                                node.BorderBrush = Res.亮红;
                                break;
                            //结束
                            case "1":
                                node.BorderBrush = Res.GreenBrush;
                                break;
                        }
                        node.Click += OnNodeClick;
                    }
                }
            }

            if (!Kit.IsPhoneUI)
            {
                Height = 700;
                Width = 500;
            }
            Show();
        }

        async void OnNodeClick(object sender, EventArgs e)
        {
            SNode node = (SNode)sender;
            string msg = await GetWfLog(_prciID, (sender as SNode).ID);
            if (!string.IsNullOrEmpty(msg))
                Kit.Msg(string.Format("{0}\r\n{1}", node.Title, msg));
        }

        async void OnLog(object sender, Mi e)
        {
            if (e.ID == "详细")
            {
                e.ID = "流程图";
                if (_tb == null)
                {
                    string msg = await GetWfLog(_prciID);
                    _tb = new TextBlock { Text = msg, Margin = new Thickness(10) };
                }
                Content = _tb;
            }
            else
            {
                e.ID = "详细";
                Content = _sketch;
            }
        }

        /// <summary>
        /// 获取指定流程的日志
        /// </summary>
        /// <param name="p_prciID">流程ID</param>
        /// <param name="p_atvdID">活动ID（查询当前活动日志），0时为整个流程</param>
        /// <returns></returns>
        async Task<string> GetWfLog(long p_prciID, long p_atvdID = 0)
        {
            var log = await AtCm.Query("流程-生成日志列表", new { prciid = p_prciID, atvdid = p_atvdID });
            if (log.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (var dr in log)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }

                var status = (WfiItemStatus)dr.Int("itemstatus");
                if (status == WfiItemStatus.同步)
                {
                    // 同步
                    sb.AppendFormat("💠 完成同步（{0}）", dr.Date("ctime").ToString("MM-dd HH:mm"));
                    continue;
                }

                sb.AppendFormat("🚩 {0}（{1}）", dr.Str("atvdname"), dr.Str("username"));
                // 指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派
                var akind = (WfiItemAssignKind)dr.Int("AssignKind");
                switch (akind)
                {
                    case WfiItemAssignKind.起始指派:
                        sb.AppendFormat("\n启动（{0}） 📩", dr.Date("ctime").ToString("MM-dd HH:mm"));
                        break;
                    case WfiItemAssignKind.回退:
                        sb.AppendFormat("\n已被【{0}】退回（{1}）", dr.Str("sender"), dr.Date("ctime").ToString("MM-dd HH:mm"));
                        break;
                    case WfiItemAssignKind.追回:
                        sb.AppendFormat("\n已追回（{0}）", dr.Date("ctime").ToString("MM-dd HH:mm"));
                        break;
                }

                // 工作项状态 0活动 1结束 2终止 3同步
                switch (status)
                {
                    case WfiItemStatus.活动:
                        if (akind == WfiItemAssignKind.起始指派)
                        {
                            // 起始指派
                            sb.AppendFormat("\n正在进行{0} 🕑", dr.Str("atvdname"));
                        }
                        else
                        {
                            if (!dr.Bool("IsAccept"))
                                sb.AppendFormat("\n正在进行{0}，尚未签收 🕒", dr.Str("atvdname"));
                            else
                                sb.AppendFormat("\n正在进行{0}，已签收（{1}） 🕔", dr.Str("atvdname"), dr.Date("AcceptTime").ToString("MM-dd HH:mm"));
                        }
                        break;
                    case WfiItemStatus.结束:
                        sb.AppendFormat("\n完成（{0}） ✔", dr.Date("mtime").ToString("MM-dd HH:mm"));
                        sb.Append(await BuildNext(dr));
                        break;
                    case WfiItemStatus.终止:
                        sb.AppendFormat("\n已终止（{0}） ⛔", dr.Date("mtime").ToString("MM-dd HH:mm"));
                        break;
                }

                string note = dr.Str("note");
                if (!string.IsNullOrEmpty(note))
                    sb.AppendFormat("\n备注：【{0}】📣 ", note);
            }

            if (p_atvdID == 0)
            {
                // 流程实例状态 0活动 1结束 2终止
                Row prci = log[0];
                var prciStatus = (WfiPrcStatus)prci.Int("prcistatus");
                if (prciStatus == WfiPrcStatus.结束)
                    sb.AppendFormat("\n\n♻ 流程已结束（{0}）", prci.Date("prcitime").ToString("MM-dd HH:mm"));
                else if (prciStatus == WfiPrcStatus.终止)
                    sb.AppendFormat("\n\n⛔ 流程已终止（{0}）", prci.Date("prcitime").ToString("MM-dd HH:mm"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 构造当前数据的下一步骤
        /// </summary>
        /// <param name="p_row"></param>
        /// <returns></returns>
        async Task<string> BuildNext(Row p_row)
        {
            // 指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派
            var tbl = await AtCm.Query("流程-日志目标项", new { prciid = p_row.Long("prciid"), atviid = p_row.Long("atviid") });
            string nexttext = "";
            foreach (var row in tbl)
            {
                nexttext += $"\n提交给{row.Str("accpname")}，进行{row.Str("atvdname")} 📨";
            }
            return nexttext;
        }

    }
}
