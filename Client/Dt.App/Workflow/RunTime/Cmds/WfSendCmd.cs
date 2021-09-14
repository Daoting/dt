#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.App.Workflow;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Workflow
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
            _info.RunCmd(Send);
        }

        async Task Send()
        {
            // 先保存
            if (!await _info.CmdSave.Save())
                return;

            // 判断当前活动是否结束（需要多人同时完成该活动的情况）
            if (!await _info.AtvInst.IsFinished())
            {
                // 活动未结束（不是最后一人），只结束当前工作项
                await SaveWorkItem(false);
                return;
            }

            // 获得所有后续活动，包括同步
            var nextAtvs = await AtCm.Query<WfdAtvObj>("流程-后续活动", new { atvid = _info.AtvDef.ID });
            if (nextAtvs.Count == 0)
            {
                // 无后续活动，结束当前工作项和活动
                await SaveWorkItem(true);
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
                        var recv = await LoadRecvs(atv);
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
                        if (await IsActive(atv))
                        {
                            // 同步活动只支持一个后续活动！
                            var syncNext = await AtCm.First<WfdAtvObj>("流程-后续活动", new { atvid = atv.ID });
                            if (syncNext != null)
                            {
                                recv = await LoadRecvs(syncNext);
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
                        break;
                }
            }

            // 当后续迁移活动为独占式选择且后续活动多于一个时手动选择
            if (!manualSend
                && _info.AtvDef.TransKind == WfdAtvTransKind.独占式选择
                && nextRecvs.AtvCount > 1)
            {
                manualSend = true;
            }
            _info.NextRecvs = nextRecvs;

            // 触发外部自定义执行者范围事件
            await _info.OnSending();

            // 外部可以修改接收者列表
            if (_info.NextRecvs != null && _info.NextRecvs.AtvCount > 0)
            {
                if (manualSend)
                {
                    // 手动选择后发送
                    if (await new WfSendDlg().Show(_info))
                        DoSend(true);
                }
                else
                {
                    DoSend(false);
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
        async void DoSend(bool p_manualSend)
        {
            #region 后续活动
            // 生成后续活动的活动实例、工作项、迁移实例，一个或多个
            var tblAtvs = Table<WfiAtvObj>.Create();
            var tblItems = Table<WfiItemObj>.Create();
            var tblTrs = Table<WfiTrsObj>.Create();
            DateTime time = Kit.Now;

            if (_info.NextRecvs.FinishedAtv != null
                && (!p_manualSend || _info.NextRecvs.FinishedAtv.IsSelected))
            {
                // 完成
                _info.PrcInst.Status = WfiAtvStatus.结束;
                _info.PrcInst.Mtime = time;
            }
            else
            {
                // 普通活动
                foreach (AtvRecv ar in _info.NextRecvs.Atvs)
                {
                    // 手动无选择时
                    if (p_manualSend
                        && (ar.SelectedRecvs == null || ar.SelectedRecvs.Count == 0))
                        continue;

                    var atvInst = new WfiAtvObj(
                        ID: await AtCm.NewID(),
                        PrciID: _info.PrcInst.ID,
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
                            var wi = await WfiItemObj.Create(atvInst.ID, time, ar.IsRole, recvID, ar.Note, false);
                            tblItems.Add(wi);
                        }
                    }
                    else
                    {
                        // 自动发送，按角色
                        atvInst.InstCount = ar.Recvs.Count;
                        foreach (var row in ar.Recvs)
                        {
                            var wi = await WfiItemObj.Create(atvInst.ID, time, ar.IsRole, row.ID, ar.Note, false);
                            tblItems.Add(wi);
                        }
                    }

                    // 增加迁移实例
                    var trs = await _info.CreateAtvTrs(ar.Def.ID, atvInst.ID, time, false);
                    tblTrs.Add(trs);
                }

                // 同步活动
                var syncAtv = _info.NextRecvs.SyncAtv;
                if (syncAtv != null
                    && (!p_manualSend || (syncAtv.SelectedRecvs != null && syncAtv.SelectedRecvs.Count > 0)))
                {
                    // 同步实例
                    var syncInst = new WfiAtvObj(
                        ID: await AtCm.NewID(),
                        PrciID: _info.PrcInst.ID,
                        AtvdID: syncAtv.SyncDef.ID,
                        Status: WfiAtvStatus.同步,
                        InstCount: 1,
                        Ctime: time,
                        Mtime: time);
                    tblAtvs.Add(syncInst);

                    // 同步工作项
                    WfiItemObj item = new WfiItemObj(
                        ID: await AtCm.NewID(),
                        AtviID: syncInst.ID,
                        AssignKind: WfiItemAssignKind.普通指派,
                        Status: WfiItemStatus.同步,
                        IsAccept: false,
                        UserID: Kit.UserID,
                        Sender: Kit.UserName,
                        Stime: time,
                        Ctime: time,
                        Mtime: time,
                        Dispidx: await AtCm.NewSeq("sq_wfi_item"));
                    tblItems.Add(item);

                    // 同步迁移实例
                    Dict dt = new Dict();
                    dt["prcid"] = _info.PrcInst.PrcdID;
                    dt["SrcAtvID"] = _info.AtvInst.AtvdID;
                    dt["TgtAtvID"] = syncAtv.SyncDef.ID;
                    dt["IsRollback"] = false;
                    long trsdid = await AtCm.GetScalar<long>("流程-迁移模板ID", dt);
                    
                    var trs = new WfiTrsObj(
                        ID: await AtCm.NewID(),
                        TrsdID: trsdid,
                        SrcAtviID: _info.AtvInst.ID,
                        TgtAtviID: syncInst.ID,
                        IsRollback: false,
                        Ctime: time);
                    tblTrs.Add(trs);

                    // 同步活动的后续活动实例
                    var nextInst = new WfiAtvObj(
                        ID: await AtCm.NewID(),
                        PrciID: _info.PrcInst.ID,
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
                            var wi = await WfiItemObj.Create(nextInst.ID, time, syncAtv.IsRole, recvID, "", false);
                            tblItems.Add(wi);
                        }
                    }
                    else
                    {
                        // 自动发送，按角色
                        nextInst.InstCount = syncAtv.Recvs.Count;
                        foreach (var row in syncAtv.Recvs)
                        {
                            var wi = await WfiItemObj.Create(nextInst.ID, time, syncAtv.IsRole, row.ID, "", false);
                            tblItems.Add(wi);
                        }
                    }

                    // 增加迁移实例
                    dt = new Dict();
                    dt["prcid"] = _info.PrcInst.PrcdID;
                    dt["SrcAtvID"] = syncAtv.SyncDef.ID;
                    dt["TgtAtvID"] = syncAtv.Def.ID;
                    dt["IsRollback"] = false;
                    trsdid = await AtCm.GetScalar<long>("流程-迁移模板ID", dt);

                    trs = new WfiTrsObj(
                        ID: await AtCm.NewID(),
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
            if (tblAtvs.Count == 0 && _info.PrcInst.Status != WfiAtvStatus.结束)
            {
                Kit.Msg("所有后续活动均无接收者，发送失败！");
                return;
            }
            #endregion

            #region 整理待保存数据
            List<object> data = new List<object>();
            if (_info.PrcInst.IsChanged)
                data.Add(_info.PrcInst);

            _info.AtvInst.Finished();
            data.Add(_info.AtvInst);

            _info.WorkItem.Finished();
            data.Add(_info.WorkItem);

            if (tblAtvs.Count > 0)
            {
                data.Add(tblAtvs);
                data.Add(tblItems);
                data.Add(tblTrs);
            }
            #endregion

            if (await AtCm.BatchSave(data, false))
            {
                Kit.Msg("发送成功！");
                _info.CloseWin();
                // 推送客户端提醒

            }
            else
            {
                // 避免保存失败后，再次点击发送时和保存表单一起被保存，造成状态错误！
                _info.PrcInst.RejectChanges();
                _info.AtvInst.RejectChanges();
                _info.WorkItem.RejectChanges();
                Kit.Warn("发送失败！");
            }
        }

        /// <summary>
        /// 保存当前工作项置完成状态
        /// </summary>
        /// <param name="p_isFinished"></param>
        /// <returns></returns>
        async Task SaveWorkItem(bool p_isFinished)
        {
            if (p_isFinished)
                _info.AtvInst.Finished();
            _info.WorkItem.Finished();

            List<object> ls = new List<object>();
            if (_info.AtvInst.IsChanged)
                ls.Add(_info.AtvInst);
            ls.Add(_info.WorkItem);

            if (await AtCm.BatchSave(ls, false))
            {
                Kit.Msg(p_isFinished ? "任务结束" : "当前工作项完成");
                _info.CloseWin();
            }
            else
            {
                Kit.Warn("工作项保存失败");
            }
        }

        /// <summary>
        /// 加载活动的接收者
        /// </summary>
        /// <param name="p_atv"></param>
        /// <returns></returns>
        async Task<RecvDef> LoadRecvs(WfdAtvObj p_atv)
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
                    recv.Recvs = await AtCm.Query("流程-活动的所有授权角色", new Dict { { "atvid", p_atv.ID } });
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
                    var limitUsers = await GetLimitUsers(atvdid, p_atv.ExecLimit);
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
        /// <returns></returns>
        async Task<bool> IsActive(WfdAtvObj p_atvSync)
        {
            Dict dt = new Dict();
            dt["prciid"] = _info.PrcInst.ID;
            dt["atvdid"] = p_atvSync.ID;
            int cnt = await AtCm.GetScalar<int>("流程-同步活动实例数", dt);

            // 已产生同步实例
            if (cnt > 0)
                return false;

            // 获得同步前所有活动
            dt = new Dict();
            dt["TgtAtvID"] = p_atvSync.ID;
            var trss = await AtCm.Query<WfdTrsObj>("流程-活动前的迁移", dt);

            // 聚合方式
            // 全部
            if (p_atvSync.JoinKind == 0)
                return await GetAllFinish(trss);

            // 任一
            if (p_atvSync.JoinKind == WfdAtvJoinKind.任一任务)
                return true;

            // 即时
            return await GetExistFinish(trss);
        }

        /// <summary>
        /// 获得同步前的活动是否已经都完成
        /// </summary>
        /// <param name="p_trss"></param>
        /// <returns></returns>
        async Task<bool> GetAllFinish(Table<WfdTrsObj> p_trss)
        {
            bool finish = true;
            foreach (var trs in p_trss)
            {
                if (trs.SrcAtvID == _info.AtvDef.ID)
                    continue;

                var dt = new Dict();
                dt["atvdid"] = trs.SrcAtvID;
                dt["prciid"] = _info.PrcInst.ID;
                int cnt = await AtCm.GetScalar<int>("流程-活动结束的实例数", dt);
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
        /// <returns></returns>
        async Task<bool> GetExistFinish(Table<WfdTrsObj> p_trss)
        {
            bool finish = true;
            foreach (var trs in p_trss)
            {
                if (trs.SrcAtvID == _info.AtvDef.ID)
                    continue;

                var dt = new Dict();
                dt["atvdid"] = trs.SrcAtvID;
                dt["prciid"] = _info.PrcInst.ID;
                var tbl = await AtCm.Query("流程-活动实例的状态", dt);
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
        async Task<Table> GetAtvUsers(long p_atvid)
        {
            Dict dt = new Dict { { "atvid", p_atvid } };
            if (await AtCm.GetScalar<int>("流程-是否活动授权任何人", dt) == 0)
                return await AtCm.Query("流程-活动的所有执行者", dt);
            return await AtCm.Query("流程-所有未过期用户");
        }

        Task<Table> GetLimitUsers(long p_atvdid, WfdAtvExecLimit p_execLimit)
        {
            string key;
            switch (p_execLimit)
            {
                case WfdAtvExecLimit.前一活动的执行者:
                    // 前一活动执行者
                    key = "流程-前一活动执行者";
                    break;
                case WfdAtvExecLimit.前一活动的同部门执行者:
                    // 前一活动的同部门执行者
                    key = "流程-前一活动的同部门执行者";
                    break;
                case WfdAtvExecLimit.已完成活动的执行者:
                    // 已完成活动执行者
                    key = "流程-已完成活动执行者";
                    break;
                default:
                    // 已完成活动同部门执行者
                    key = "流程-已完成活动同部门执行者";
                    break;
            }
            return AtCm.Query(key, new { prciId = _info.PrcInst.ID, atvdid = p_atvdid });
        }

        class RecvDef
        {
            public bool IsRole { get; set; }

            public Table Recvs { get; set; }

            public bool IsManualSend { get; set; }
        }
    }
}
