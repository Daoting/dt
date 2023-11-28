#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Rbac;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 流程实例
    /// </summary>
    public partial class WfiDs : DomainSvc<WfiDs>
    {
        #region 保存表单
        public static Task<bool> SaveForm(WfFormInfo p_info)
        {
            return SaveFormInternal(p_info, true);
        }

        static async Task<bool> SaveFormInternal(WfFormInfo p_info, bool p_isNotify)
        {
            var w = _da.NewWriter();

            // 先添加待保存的表单数据
            if (!await p_info.Form.OnSave(w))
                return false;

            // 标题
            string name = p_info.Form.GetPrcName();
            if (name != p_info.PrcInst.Name)
            {
                p_info.PrcInst.Name = name;
                p_info.FormWin.Title = name;
            }

            if (p_info.PrcInst.IsAdded)
            {
                DateTime time = Kit.Now;
                p_info.PrcInst.Ctime = time;
                p_info.PrcInst.Mtime = time;
                p_info.PrcInst.Dispidx = await WfiPrcX.NewSeq("Dispidx");

                p_info.AtvInst.Ctime = time;
                p_info.AtvInst.Mtime = time;

                p_info.WorkItem.AcceptTime = time;
                p_info.WorkItem.Dispidx = await WfiItemX.NewSeq("Dispidx");
                p_info.WorkItem.Ctime = time;
                p_info.WorkItem.Mtime = time;
                p_info.WorkItem.Stime = time;
            }

            await w.Save(p_info.PrcInst);
            await w.Save(p_info.AtvInst);
            await w.Save(p_info.WorkItem);

            return await w.Commit(p_isNotify);
        }
        #endregion

        #region 发送
        /// <summary>
        /// 发送成功事件
        /// </summary>
        public static event Action<WfFormInfo> Sended;

        public static async Task Send(WfFormInfo p_info)
        {
            // 先按照非发送状态保存表单数据，如自填写、新建等数据
            // 此保存过程和发送无关，相当于点击保存按钮，与发送不在一个事务！
            if (!await SaveFormInternal(p_info, false))
            {
                Kit.Warn("表单保存失败！");
                return;
            }

            // 确保发送时外部自动填写表单数据或特殊校验，如发送人、发送时间、当前状态等
            var w = _da.NewWriter();
            if (!await p_info.Form.OnSend(w))
                return;

            // 判断当前活动是否结束（需要多人同时完成该活动的情况）
            if (!await p_info.AtvInst.IsFinished())
            {
                // 活动未结束（不是最后一人），只结束当前工作项
                await SaveWorkItem(false, p_info, w);
                return;
            }

            // 获得所有后续活动，包括同步
            var nextAtvs = await WfdAtvX.GetNextAtv(p_info.AtvDef.ID);
            if (nextAtvs.Count == 0)
            {
                // 无后续活动，结束当前工作项和活动
                await SaveWorkItem(true, p_info, w);
                return;
            }

            bool manualSend = false;
            var nextRecvs = new AtvRecvs();
            foreach (var atv in nextAtvs)
            {
                switch (atv.Type)
                {
                    case WfdAtvType.Normal:
                        // 普通活动
                        var recv = await LoadRecvs(atv, p_info);
                        if (recv.Recvs != null && recv.Recvs.Count > 0)
                        {
                            AtvRecv ar = new AtvRecv { Def = atv, IsRole = recv.IsRole, Recvs = recv.Recvs };
                            nextRecvs.Atvs.Add(ar);

                            if (recv.IsManualSend)
                                manualSend = true;
                        }
                        break;

                    case WfdAtvType.Sync:
                        // 同步活动 且 可激活后续活动时
                        if (await IsActive(atv, p_info))
                        {
                            // 同步活动只支持一个后续活动！
                            var syncNext = await WfdAtvX.GetFirstNextAtv(atv.ID);
                            if (syncNext != null)
                            {
                                recv = await LoadRecvs(syncNext, p_info);
                                if (recv.Recvs != null && recv.Recvs.Count > 0)
                                {
                                    nextRecvs.SyncAtv = new AtvSyncRecv
                                    {
                                        SyncDef = atv,
                                        Def = syncNext,
                                        IsRole = recv.IsRole,
                                        Recvs = recv.Recvs
                                    };

                                    if (recv.IsManualSend)
                                        manualSend = true;
                                }
                            }
                        }
                        break;

                    case WfdAtvType.Finish:
                        // 结束活动
                        nextRecvs.FinishedAtv = new AtvFinishedRecv { Def = atv };
                        if (!await Kit.Confirm($"当前 [{p_info.State}] 通过后，任务将完成结束！\r\n确认继续执行吗？"))
                            return;
                        break;
                }
            }

            // 当后续迁移活动为独占式选择且后续活动多于一个时手动选择
            if (!manualSend
                && p_info.AtvDef.TransKind == WfdAtvTransKind.独占式选择
                && nextRecvs.AtvCount > 1)
            {
                manualSend = true;
            }
            p_info.NextRecvs = nextRecvs;

            // 触发外部自定义执行者范围事件
            await p_info.OnSending();

            // 外部可以修改接收者列表
            if (p_info.NextRecvs != null && p_info.NextRecvs.AtvCount > 0)
            {
                if (manualSend)
                {
                    // 手动选择后发送
                    var dlg = new WfSendDlg();
                    if (await dlg.Show(p_info))
                        DoSend(true, p_info, dlg.Note == "" ? null : dlg.Note, w);
                }
                else
                {
                    DoSend(false, p_info, null, w);
                }
            }
            else
            {
                Kit.Warn("无后续活动接收者，请检查流程授权是否合理！");
            }
        }

        /// <summary>
        /// 执行发送
        /// </summary>
        /// <param name="p_manualSend">是否手动选择接收者</param>
        /// <param name="p_info"></param>
        /// <param name="p_note">留言</param>
        /// <param name="w"></param>
        static async void DoSend(bool p_manualSend, WfFormInfo p_info, string p_note, IEntityWriter w)
        {
            #region 后续活动
            // 生成后续活动的活动实例、工作项、迁移实例，一个或多个
            var tblAtvs = await Table<WfiAtvX>.Create();
            var tblItems = await Table<WfiItemX>.Create();
            var tblTrs = await Table<WfiTrsX>.Create();
            DateTime time = Kit.Now;

            if (p_info.NextRecvs.FinishedAtv != null
                && (!p_manualSend || p_info.NextRecvs.FinishedAtv.IsSelected))
            {
                // 完成
                p_info.PrcInst.Status = WfiPrcStatus.结束;
                p_info.PrcInst.Mtime = time;
            }
            else
            {
                // 普通活动
                foreach (AtvRecv ar in p_info.NextRecvs.Atvs)
                {
                    // 手动无选择时
                    if (p_manualSend
                        && (ar.SelectedRecvs == null || ar.SelectedRecvs.Count == 0))
                        continue;

                    var atvInst = await WfiAtvX.New(
                        PrciID: p_info.PrcInst.ID,
                        AtvdID: ar.Def.ID,
                        Status: WfiAtvStatus.活动,
                        Ctime: time,
                        Mtime: time);
                    tblAtvs.Add(atvInst);

                    if (p_manualSend)
                    {
                        // 手动发送，已选择项可能为用户或角色
                        atvInst.InstCount = ar.SelectedRecvs.Count;
                        foreach (var recvID in ar.SelectedRecvs)
                        {
                            long? userID = ar.IsRole ? null : recvID;
                            long? roleID = ar.IsRole ? recvID : null;
                            var wi = await WfiItemX.New(
                                AtviID: atvInst.ID,
                                Stime: time,
                                Ctime: time,
                                Mtime: time,
                                AssignKind: WfiItemAssignKind.普通指派,
                                SenderID: Kit.UserID,
                                Sender: Kit.UserName,
                                Status: WfiItemStatus.活动,
                                RoleID: roleID,
                                UserID: userID,
                                Note: p_note);
                            tblItems.Add(wi);
                        }
                    }
                    else
                    {
                        // 自动发送，按角色
                        atvInst.InstCount = ar.Recvs.Count;
                        foreach (var row in ar.Recvs)
                        {
                            long? userID = ar.IsRole ? null : row.ID;
                            long? roleID = ar.IsRole ? row.ID : null;
                            var wi = await WfiItemX.New(
                                AtviID: atvInst.ID,
                                Stime: time,
                                Ctime: time,
                                Mtime: time,
                                AssignKind: WfiItemAssignKind.普通指派,
                                SenderID: Kit.UserID,
                                Sender: Kit.UserName,
                                Status: WfiItemStatus.活动,
                                RoleID: roleID,
                                UserID: userID,
                                Note: p_note);
                            tblItems.Add(wi);
                        }
                    }

                    // 增加迁移实例
                    var trs = await p_info.CreateAtvTrs(ar.Def.ID, atvInst.ID, time, false);
                    tblTrs.Add(trs);
                }

                // 同步活动
                var syncAtv = p_info.NextRecvs.SyncAtv;
                if (syncAtv != null
                    && (!p_manualSend || (syncAtv.SelectedRecvs != null && syncAtv.SelectedRecvs.Count > 0)))
                {
                    // 同步实例
                    var syncInst = await WfiAtvX.New(
                        PrciID: p_info.PrcInst.ID,
                        AtvdID: syncAtv.SyncDef.ID,
                        Status: WfiAtvStatus.同步,
                        InstCount: 1,
                        Ctime: time,
                        Mtime: time);
                    tblAtvs.Add(syncInst);

                    // 同步工作项
                    WfiItemX item = await WfiItemX.New(
                        AtviID: syncInst.ID,
                        AssignKind: WfiItemAssignKind.普通指派,
                        Status: WfiItemStatus.同步,
                        IsAccept: false,
                        UserID: Kit.UserID,
                        SenderID: Kit.UserID,
                        Sender: Kit.UserName,
                        Stime: time,
                        Ctime: time,
                        Mtime: time);
                    tblItems.Add(item);

                    // 同步迁移实例
                    long trsdid = await WfdDs.GetWfdTrsID(p_info.PrcInst.PrcdID, p_info.AtvInst.AtvdID, syncAtv.SyncDef.ID, false);

                    var trs = await WfiTrsX.New(
                        TrsdID: trsdid,
                        SrcAtviID: p_info.AtvInst.ID,
                        TgtAtviID: syncInst.ID,
                        IsRollback: false,
                        Ctime: time);
                    tblTrs.Add(trs);

                    // 同步活动的后续活动实例
                    var nextInst = await WfiAtvX.New(
                        PrciID: p_info.PrcInst.ID,
                        AtvdID: syncAtv.Def.ID,
                        Status: WfiAtvStatus.活动,
                        Ctime: time,
                        Mtime: time);
                    tblAtvs.Add(nextInst);

                    if (p_manualSend)
                    {
                        // 手动发送，已选择项可能为用户或角色
                        nextInst.InstCount = syncAtv.SelectedRecvs.Count;
                        foreach (var recvID in syncAtv.SelectedRecvs)
                        {
                            long? userID = syncAtv.IsRole ? null : recvID;
                            long? roleID = syncAtv.IsRole ? recvID : null;
                            var wi = await WfiItemX.New(
                                AtviID: nextInst.ID,
                                Stime: time,
                                Ctime: time,
                                Mtime: time,
                                AssignKind: WfiItemAssignKind.普通指派,
                                SenderID: Kit.UserID,
                                Sender: Kit.UserName,
                                Status: WfiItemStatus.活动,
                                RoleID: roleID,
                                UserID: userID,
                                Note: p_note);
                            tblItems.Add(wi);
                        }
                    }
                    else
                    {
                        // 自动发送，按角色
                        nextInst.InstCount = syncAtv.Recvs.Count;
                        foreach (var row in syncAtv.Recvs)
                        {
                            long? userID = syncAtv.IsRole ? null : row.ID;
                            long? roleID = syncAtv.IsRole ? row.ID : null;
                            var wi = await WfiItemX.New(
                                AtviID: nextInst.ID,
                                Stime: time,
                                Ctime: time,
                                Mtime: time,
                                AssignKind: WfiItemAssignKind.普通指派,
                                SenderID: Kit.UserID,
                                Sender: Kit.UserName,
                                Status: WfiItemStatus.活动,
                                RoleID: roleID,
                                UserID: userID,
                                Note: p_note);
                            tblItems.Add(wi);
                        }
                    }

                    // 增加迁移实例
                    trsdid = await WfdDs.GetWfdTrsID(p_info.PrcInst.PrcdID, syncAtv.SyncDef.ID, syncAtv.Def.ID, false);

                    trs = await WfiTrsX.New(
                        TrsdID: trsdid,
                        SrcAtviID: syncInst.ID,
                        TgtAtviID: nextInst.ID,
                        IsRollback: false,
                        Ctime: time);
                    tblTrs.Add(trs);
                }
            }

            // 发送是否有效
            // 1. 只有'完成'时有效
            // 2. 至少含有一个活动实例时有效
            if (tblAtvs.Count == 0 && p_info.PrcInst.Status != WfiPrcStatus.结束)
            {
                Kit.Msg("所有后续活动均无接收者，发送失败！");
                return;
            }
            #endregion

            #region 整理待保存数据
            if (p_info.PrcInst.IsChanged)
                await w.Save(p_info.PrcInst);

            p_info.AtvInst.Finished();
            await w.Save(p_info.AtvInst);

            p_info.WorkItem.Finished();
            await w.Save(p_info.WorkItem);

            if (tblAtvs.Count > 0)
            {
                await w.Save(tblAtvs);
                await w.Save(tblItems);
                await w.Save(tblTrs);
            }
            #endregion

            if (await w.Commit(false))
            {
                Kit.Msg("发送成功！");
                p_info.CloseWin();

                // 触发事件
                Sended?.Invoke(p_info);

                // 推送客户端提醒
                PushNotify(tblItems);
            }
            else
            {
                // 避免保存失败后，再次点击发送时和保存表单一起被保存，造成状态错误！
                p_info.PrcInst.RejectChanges();
                p_info.AtvInst.RejectChanges();
                p_info.WorkItem.RejectChanges();
                Kit.Warn("发送失败！");
            }
        }

        /// <summary>
        /// 保存当前工作项置完成状态
        /// </summary>
        /// <param name="p_isFinished"></param>
        /// <param name="p_info"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        static async Task SaveWorkItem(bool p_isFinished, WfFormInfo p_info, IEntityWriter w)
        {
            if (p_isFinished)
                p_info.AtvInst.Finished();
            p_info.WorkItem.Finished();

            if (p_info.AtvInst.IsChanged)
                await w.Save(p_info.AtvInst);
            await w.Save(p_info.WorkItem);

            if (await w.Commit(false))
            {
                Kit.Msg(p_isFinished ? "任务结束" : "当前工作项完成");
                p_info.CloseWin();
            }
            else
            {
                Kit.Warn("工作项保存失败");
            }
        }

        /// <summary>
        /// 推送接收者客户端提醒
        /// </summary>
        /// <param name="p_items"></param>
        static async void PushNotify(Table<WfiItemX> p_items)
        {
            HashSet<long> users = new HashSet<long>();
            List<long> roles = new List<long>();
            foreach (var item in p_items)
            {
                if (item.UserID != null)
                    users.Add(item.UserID.Value);
                else if (item.RoleID != null)
                    roles.Add(item.RoleID.Value);
            }

            if (roles.Count > 0)
            {
                var ls = await RbacDs.GetUsersOfRoles(roles);
                if (ls.Count > 0)
                {
                    users.UnionWith(ls);
                }
            }

            if (users.Count > 0)
            {
                var mi = new MsgInfo
                {
                    MethodName = "SysPushApi.WfNotify",
                    Title = "您有新的待办任务",
                };
                await AtMsg.BatchSendCmd(users.ToList(), mi);
            }
        }

        /// <summary>
        /// 加载活动的接收者
        /// </summary>
        /// <param name="p_atv"></param>
        /// <param name="p_info"></param>
        /// <returns></returns>
        static async Task<RecvDef> LoadRecvs(WfdAtvX p_atv, WfFormInfo p_info)
        {
            RecvDef recv = new RecvDef();
            if (p_atv.ExecLimit == WfdAtvExecLimit.无限制)
            {
                // 无限制
                if (p_atv.ExecScope == WfdAtvExecScope.一组用户 || p_atv.ExecScope == WfdAtvExecScope.单个用户)
                {
                    // 一组用户或单个用户，所有授权用户为被选项
                    recv.IsManualSend = true;
                    recv.IsRole = false;
                    recv.Recvs = await GetAtvUsers(p_atv.ID);
                }
                else
                {
                    // 所有用户或任一用户，按角色发
                    recv.IsRole = true;
                    recv.Recvs = await RoleX.Query($"where exists (select distinct (role_id) from cm_wfd_atv_role ar where r.id=ar.role_id and atv_id={p_atv.ID})");
                }
            }
            else
            {
                // 有限制，按过滤后的用户发送
                recv.IsRole = false;
                var users = await GetAtvUsers(p_atv.ID);
                if (users.Count > 0)
                {
                    long atvdid = (p_atv.ExecLimit == WfdAtvExecLimit.已完成活动的执行者 || p_atv.ExecLimit == WfdAtvExecLimit.已完成活动的同部门执行者) ? p_atv.ExecAtvID.Value : p_atv.ID;
                    var limitUsers = await GetLimitUsers(atvdid, p_atv.ExecLimit, p_info);
                    if (limitUsers.Count > 0)
                    {
                        // 取已授权用户和符合限制用户的交集
                        Table tblJoin = new Table
                        {
                            {"id", typeof(long) },
                            {"name" }
                        };
                        foreach (var l in limitUsers)
                        {
                            foreach (var r in users)
                            {
                                if (r.Long("id") == l.Long(0))
                                {
                                    tblJoin.AddRow(new { id = l.Long(0), name = r["name"] });
                                    break;
                                }
                            }
                        }
                        recv.Recvs = tblJoin;
                    }
                }

                // 除‘所有用户’外其余手动发送
                if (p_atv.ExecScope != WfdAtvExecScope.所有用户)
                    recv.IsManualSend = true;
            }
            return recv;
        }

        /// <summary>
        /// 是否激活后续活动
        /// </summary>
        /// <param name="p_atvSync">同步活动</param>
        /// <param name="p_info"></param>
        /// <returns></returns>
        static async Task<bool> IsActive(WfdAtvX p_atvSync, WfFormInfo p_info)
        {
            int cnt = await WfiAtvX.GetCount($"where prci_id={p_info.PrcInst.ID} and atvd_id={p_atvSync.ID}");

            // 已产生同步实例
            if (cnt > 0)
                return false;

            // 获得同步前所有活动
            var trss = await WfdTrsX.Query($"where tgt_atv_id={p_atvSync.ID}");

            // 聚合方式
            // 全部
            if (p_atvSync.JoinKind == 0)
                return await GetAllFinish(trss, p_info);

            // 任一
            if (p_atvSync.JoinKind == WfdAtvJoinKind.任一任务)
                return true;

            // 即时
            return await GetExistFinish(trss, p_info);
        }

        /// <summary>
        /// 获得同步前的活动是否已经都完成
        /// </summary>
        /// <param name="p_trss"></param>
        /// <param name="p_info"></param>
        /// <returns></returns>
        static async Task<bool> GetAllFinish(Table<WfdTrsX> p_trss, WfFormInfo p_info)
        {
            bool finish = true;
            foreach (var trs in p_trss)
            {
                if (trs.SrcAtvID == p_info.AtvDef.ID)
                    continue;

                int cnt = await WfiAtvX.GetCount($"where atvd_id={trs.SrcAtvID} and prci_id={p_info.PrcInst.ID} and status=1");
                if (cnt == 0)
                {
                    finish = false;
                    break;
                }
            }
            return finish;
        }

        /// <summary>
        /// 同步前已存在的实例是否都完成
        /// </summary>
        /// <param name="p_trss"></param>
        /// <param name="p_info"></param>
        /// <returns></returns>
        static async Task<bool> GetExistFinish(Table<WfdTrsX> p_trss, WfFormInfo p_info)
        {
            bool finish = true;
            foreach (var trs in p_trss)
            {
                if (trs.SrcAtvID == p_info.AtvDef.ID)
                    continue;

                var tbl = await _da.Query($"select status from cm_wfi_atv where atvd_id={trs.SrcAtvID} and prci_id={p_info.PrcInst.ID}");
                if (tbl.Count > 0 && tbl[0].Int("status") != 1)
                {
                    finish = false;
                    break;
                }
            }
            return finish;
        }

        /// <summary>
        /// 获取活动的所有可执行用户
        /// </summary>
        /// <param name="p_atvid"></param>
        /// <returns></returns>
        static async Task<Table> GetAtvUsers(long p_atvid)
        {
            // 是否活动授权任何人
            if (await WfdAtvRoleX.GetCount($"where role_id=1 and atv_id={p_atvid}") == 0)
                return await _da.Query($"select id,coalesce(name, acc, phone) as name from cm_user u where exists (select distinct (user_id) from cm_user_role ur where exists (select role_id from cm_wfd_atv_role ar where ur.role_id=ar.role_id and atv_id={p_atvid}) and u.id=ur.user_id) order by name");
            return await _da.Query("select id, coalesce(name, acc, phone) as name from cm_user where expired='0'");
        }

        static Task<Table> GetLimitUsers(long p_atvdid, WfdAtvExecLimit p_execLimit, WfFormInfo p_info)
        {
            string sql;
            switch (p_execLimit)
            {
                case WfdAtvExecLimit.前一活动的执行者:
                    // 前一活动执行者
                    sql = Sql前一活动的执行者;
                    break;
                case WfdAtvExecLimit.前一活动的同部门执行者:
                    // 前一活动的同部门执行者
                    sql = Sql前一活动的同部门执行者;
                    break;
                case WfdAtvExecLimit.已完成活动的执行者:
                    // 已完成活动执行者
                    sql = Sql已完成活动的执行者;
                    break;
                default:
                    // 已完成活动同部门执行者
                    sql = Sql已完成活动同部门执行者;
                    break;
            }
            Throw.IfEmpty(sql, "暂时未实现");
            return _da.Query(string.Format(sql, p_info.PrcInst.ID, p_atvdid));
        }

        class RecvDef
        {
            public bool IsRole { get; set; }

            public Table Recvs { get; set; }

            public bool IsManualSend { get; set; }
        }
        #endregion

        #region 回退
        public static async Task Rollback(WfFormInfo p_info)
        {
            // 活动执行者多于一人时，不允许进行回退
            if (p_info.AtvInst.InstCount > 1)
            {
                Kit.Msg("该活动执行者多于一人，不允许回退！");
                return;
            }

            // 获得前一活动实例
            var pre = await p_info.AtvInst.GetRollbackAtv();
            if (pre == null)
            {
                Kit.Msg("该活动不允许回退！");
                return;
            }

            var res = await Confirm();
            if (!res.Item1)
                return;

            DateTime time = Kit.Now;
            var newAtvInst = await WfiAtvX.New(
                PrciID: p_info.PrcInst.ID,
                AtvdID: pre.AtvdID,
                Status: WfiAtvStatus.活动,
                InstCount: 1,
                Ctime: time,
                Mtime: time);

            // 创建迁移实例
            var newTrs = await p_info.CreateAtvTrs(pre.AtvdID, newAtvInst.ID, time, true);

            // 当前活动完成状态
            p_info.AtvInst.Finished();

            // 当前工作项置成完成状态
            p_info.WorkItem.Finished();

            long userId = await GetSenderID(p_info);
            if (userId == 0)
            {
                Kit.Msg("未找到要回退的目标用户！");
                p_info.CloseWin();
            }

            var newItem = await WfiItemX.New(
                AtviID: newAtvInst.ID,
                Stime: time,
                Ctime: time,
                Mtime: time,
                AssignKind: WfiItemAssignKind.回退,
                Status: WfiItemStatus.活动,
                SenderID: Kit.UserID,
                Sender: Kit.UserName,
                UserID: userId,
                Note: res.Item2);

            var w = _da.NewWriter();
            if (p_info.AtvInst.IsChanged)
                await w.Save(p_info.AtvInst);
            await w.Save(p_info.WorkItem);
            await w.Save(newAtvInst);
            await w.Save(newItem);
            await w.Save(newTrs);

            if (await w.Commit(false))
            {
                Kit.Msg("回退成功！");
                p_info.CloseWin();
            }
            else
            {
                Kit.Msg("回退失败！");
            }
        }

        static async Task<long> GetSenderID(WfFormInfo p_info)
        {
            long id = 0;
            if (p_info.WorkItem.AssignKind == WfiItemAssignKind.回退)
            {
                long atvid = await _da.GetScalar<long>($"select id from cm_wfi_atv where prci_id={p_info.AtvInst.PrciID} and atvd_id={p_info.AtvInst.AtvdID} and status=1 order by mtime desc");
                if (atvid != 0)
                {
                    id = await _da.GetScalar<long>($"select sender_id from cm_wfi_item where atvi_id={atvid} order by mtime desc");
                }
            }
            else if (p_info.WorkItem.SenderID.HasValue)
            {
                id = p_info.WorkItem.SenderID.Value;
            }
            return id;
        }

        static async Task<(bool, string)> Confirm()
        {
            var dlg = new Dlg { Title = "回退确认", IsPinned = true };
            if (!Kit.IsPhoneUI)
            {
                dlg.WinPlacement = DlgPlacement.CenterScreen;
                dlg.Width = 500;
                dlg.ShowVeil = true;
            }
            Grid grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            var sp = new StackPanel();
            sp.Children.Add(new TextBlock { Text = "确认要回退吗？", TextWrapping = TextWrapping.Wrap });
            var tb = new TextBox { PlaceholderText = "留言", AcceptsReturn = true, Height = 120, Margin = new Thickness(0, 20, 0, 0) };
            sp.Children.Add(tb);
            grid.Children.Add(sp);

            StackPanel spBtn = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 20, 0, 0), HorizontalAlignment = HorizontalAlignment.Right };
            var btn = new Button { Content = "确认", Margin = new Thickness(0, 0, 20, 0) };
            btn.Click += (s, e) => dlg.Close(true);
            spBtn.Children.Add(btn);
            btn = new Button { Content = "取消" };
            btn.Click += (s, e) => dlg.Close(false);
            spBtn.Children.Add(btn);
            Grid.SetRow(spBtn, 1);
            grid.Children.Add(spBtn);
            dlg.Content = grid;
            var res = await dlg.ShowAsync();
            var txt = tb.Text.Trim().Trim('\r');
            return (res, txt == "" ? null : txt);
        }
        #endregion

        #region 追回
        public static async Task<bool> Retrieve(Row row)
        {
            var status = (WfiPrcStatus)row.Int("status");
            if (status != WfiPrcStatus.活动)
            {
                Kit.Warn($"该任务已{status}，无法追回");
                return false;
            }

            if (row.Int("reCount") > 0)
            {
                Kit.Warn("含回退，无法追回");
                return false;
            }

            var tbl = await WfiItemX.GetNextItems(row.Long("atvd_id"), row.Long("prci_id"));
            if (tbl.Count == 0)
            {
                Kit.Warn("无后续活动，无法追回");
                return false;
            }

            HashSet<long> ls = new HashSet<long>();
            foreach (var r in tbl)
            {
                var itemState = r.Status;
                if (itemState == WfiItemStatus.同步)
                {
                    Kit.Warn("后续活动包含同步，无法追回");
                    return false;
                }

                if (itemState != WfiItemStatus.活动
                    || r.IsAccept)
                {
                    Kit.Warn("已签收无法追回！");
                    return false;
                }
                ls.Add(r.Long("atvi_id"));
            }

            // 更新当前实例状态为活动
            DateTime time = Kit.Now;
            WfiAtvX curAtvi = await WfiAtvX.GetByID(row.Long("atvi_id"));
            curAtvi.Status = WfiAtvStatus.活动;
            curAtvi.InstCount += 1;
            curAtvi.Mtime = time;

            // 根据当前工作项创建新工作项并更改指派方式
            var curItem = await WfiItemX.GetByID(row.Long("item_id"));
            var newItem = await WfiItemX.New(
                AtviID: curItem.AtviID,
                Status: WfiItemStatus.活动,
                AssignKind: WfiItemAssignKind.追回,
                SenderID: curItem.SenderID,
                Sender: curItem.Sender,
                Stime: curItem.Stime,
                IsAccept: false,
                RoleID: curItem.RoleID,
                UserID: curItem.UserID,
                Note: curItem.Note,
                Ctime: time,
                Mtime: time);

            // 删除已发送的后续活动实例，关联删除工作项及迁移实例
            Table<WfiAtvX> nextAtvs = new Table<WfiAtvX>();
            nextAtvs.RecordDeleted();
            foreach (var id in ls)
            {
                nextAtvs.DeletedRows.Add(new WfiAtvX(id));
            }

            // 一个事务批量保存
            var w = _da.NewWriter();
            await w.Save(nextAtvs);
            await w.Save(curAtvi);
            await w.Save(newItem);
            return await w.Commit(false);
        }
        #endregion

        #region 签收
        public static async Task ToggleAccept(WfFormInfo p_info)
        {
            if (p_info.WorkItem.IsAccept)
            {
                p_info.WorkItem.IsAccept = false;
                p_info.WorkItem.AcceptTime = null;
                if (await p_info.WorkItem.Save(false))
                    Kit.Msg("已取消签收！");
            }
            else
            {
                p_info.WorkItem.IsAccept = true;
                p_info.WorkItem.AcceptTime = Kit.Now;
                if (await p_info.WorkItem.Save(false))
                    Kit.Msg("已签收！");
            }
        }
        #endregion

        #region 删除
        public static async Task Delete(WfFormInfo p_info)
        {
            if (p_info.IsReadOnly)
            {
                Kit.Warn("禁止删除表单！");
                return;
            }

            // 管理时始终可删除
            if (!p_info.IsReadOnly
                && (p_info.AtvDef == null || (!p_info.AtvDef.CanDelete && p_info.AtvDef.Type != WfdAtvType.Start)))
            {
                Kit.Warn("您无权删除当前表单！请回退或发送到其他用户进行删除。");
                return;
            }

            if (!await Kit.Confirm("确认要删除当前表单吗？删除后表单将不可恢复！"))
                return;

            var w = _da.NewWriter();
            if (await p_info.Form.OnDelete(w))
            {
                if (!p_info.PrcInst.IsAdded)
                {
                    await w.Delete(p_info.PrcInst);
                    if (!await w.Commit(false))
                        Kit.Warn("表单删除失败！");
                }
                p_info.CloseWin();
            }
        }
        #endregion

        #region 新任务推送
        /// <summary>
        /// 新任务事件
        /// </summary>
        public static event Action NewTask;

        public static void ReceiveNewTask()
        {
            NewTask?.Invoke();

            // 避免过多
            if (Kit.NotifyList.Count > 5)
                return;

            var notify = new NotifyInfo();
            notify.Delay = 0;
            notify.Link = "查看";
            notify.LinkCallback = (e) =>
            {
                Dlg dlg = new Dlg { IsPinned = true };
                if (!Kit.IsPhoneUI)
                {
                    dlg.Width = 700;
                    dlg.Height = 600;
                }
                dlg.LoadTab(new CurrentTasks());
                dlg.Show();

                // 关闭所有待办提醒的提示
                var list = new List<NotifyInfo>();
                foreach (var ni in Kit.NotifyList)
                {
                    if (ni.Tag is string msg && msg == "待办提醒")
                        list.Add(ni);
                }
                if (list.Count > 0)
                    list.ForEach((ni) => Kit.CloseNotify(ni));
            };
            notify.Message = "您有新的待办任务";
            notify.Tag = "待办提醒";
            Kit.Notify(notify);
        }
        #endregion
    }
}
