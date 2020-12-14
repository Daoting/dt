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

            await _info.AtvInst.UpdateFinished();
            _info.WorkItem.Finished();

            // 判断当前活动是否结束（需要多人同时完成该活动的情况）
            if (!_info.AtvInst.IsFinished)
            {
                // 活动未结束（不是最后一人），只结束当前工作项
                await SaveWorkItem();
                return;
            }

            // 获得所有后续活动，包括同步
            var nextAtvs = await AtCm.Query<WfdAtv>("流程-后续活动", new { atvid = _info.AtvDef.ID });
            if (nextAtvs.Count == 0)
            {
                // 无后续活动，结束当前工作项和活动
                await SaveWorkItem();
                return;
            }

            List<WfdAtv> atvs = new List<WfdAtv>();
            // 合并同步活动的后续活动，同步的后续活动不可回退！
            foreach (var atv in nextAtvs)
            {
                // 同步且可激活后续活动时合并
                if (atv.Type == WfdAtvType.Sync && await IsActive(atv))
                {
                    var tblNext = await AtCm.Query<WfdAtv>("流程-后续活动", new { atvid = atv.ID });
                    if (tblNext.Count > 0)
                    {
                        atvs.AddRange(tblNext);
                    }
                }
            }
            if (atvs.Count > 0)
                atvs.ForEach(a => nextAtvs.Add(a));

            bool manualSend = false;
            var nextRecvs = new AtvRecvs();
            foreach (var atv in nextAtvs)
            {
                // 同步活动 或 结束活动
                var tp = atv.Type;
                if (tp == WfdAtvType.Sync || tp == WfdAtvType.Finish)
                    continue;

                AtvRecv ar = new AtvRecv();
                ar.Def = atv;
                if (atv.ExecLimit == WfdAtvExecLimit.无限制)
                {
                    // 无限制
                    if (atv.ExecScope == WfdAtvExecScope.一组用户 || atv.ExecScope == WfdAtvExecScope.单个用户)
                    {
                        // 一组用户或单个用户，所有授权用户为被选项
                        manualSend = true;
                        ar.IsRole = false;
                        ar.Recvs = await GetAtvUsers(atv.ID);
                    }
                    else
                    {
                        // 所有用户或任一用户，按角色发
                        ar.IsRole = true;
                        ar.Recvs = await AtCm.Query("流程-活动的所有授权角色", new Dict { { "atvid", atv.ID } });
                    }
                }
                else
                {
                    // 有限制，按过滤后的用户发送
                    ar.IsRole = false;
                    var users = await GetAtvUsers(atv.ID);
                    if (users.Count > 0)
                    {
                        long atvdid = (atv.ExecLimit == WfdAtvExecLimit.已完成活动的执行者 || atv.ExecLimit == WfdAtvExecLimit.已完成活动的同部门执行者) ? atv.ExecAtvID.Value : atv.ID;
                        var limitUsers = await GetLimitUsers(atvdid, atv.ExecLimit);
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
                            ar.Recvs = tblJoin;
                        }
                    }

                    // 除‘所有用户’外其余手动发送
                    if (atv.ExecScope != WfdAtvExecScope.所有用户)
                        manualSend = true;
                }

                if (ar.Recvs != null && ar.Recvs.Count > 0)
                    nextRecvs.Add(ar);
            }

            // 当后续迁移活动为独占式选择且后续活动多于一个时手动选择
            if (!manualSend
                && nextRecvs.Count > 1
                && _info.AtvDef.TransKind == WfdAtvTransKind.独占式选择)
            {
                manualSend = true;
            }
            _info.NextRecvs = nextRecvs;

            // 触发外部自定义执行者范围事件
            await _info.OnSending();

            if (_info.NextRecvs != null && _info.NextRecvs.Count > 0)
            {
                // 手动选择后发送
                if (manualSend)
                    new WfSendDlg().Show(_info);
                else
                    DoSend(false);
            }
        }

        /// <summary>
        /// 执行发送
        /// </summary>
        internal async void DoSend(bool p_manualSend)
        {
            #region 后续活动
            // 生成后续活动的活动实例、工作项、迁移实例，一个或多个
            var tblAtvs = Table<WfiAtv>.Create();
            var tblItems = Table<WfiItem>.Create();
            var tblTrs = Table<WfiTrs>.Create();
            DateTime time = AtSys.Now;

            foreach (AtvRecv ar in _info.NextRecvs)
            {
                WfiAtv atvInst = null;
                var atvType = ar.Def.Type;
                if (atvType == WfdAtvType.Normal)
                {
                    // 活动实例
                    atvInst = new WfiAtv(
                        ID: await AtCm.NewID(),
                        PrciID: _info.PrcInst.ID,
                        AtvdID: ar.Def.ID,
                        Status: WfiAtvStatus.活动,
                        Ctime: time,
                        Mtime: time);

                    if (p_manualSend)
                    {
                        // 手动发送，已选择项可能为用户或角色
                        int cnt = 0;
                        if (ar.SelectedRecvs != null)
                        {
                            foreach (var recvID in ar.SelectedRecvs)
                            {
                                var wi = await WfiItem.Create(atvInst.ID, time, ar.IsRole, recvID, ar.Note, false);
                                tblItems.Add(wi);
                                cnt++;
                            }
                        }

                        // 无选择项时移除活动实例
                        if (cnt == 0)
                        {
                            atvInst = null;
                        }
                        else
                        {
                            atvInst.InstCount = cnt;
                            tblAtvs.Add(atvInst);
                        }
                    }
                    else
                    {
                        // 自动发送，按角色
                        atvInst.InstCount = ar.Recvs.Count;
                        tblAtvs.Add(atvInst);
                        foreach (var row in ar.Recvs)
                        {
                            var wi = await WfiItem.Create(atvInst.ID, time, ar.IsRole, row.ID, ar.Note, false);
                            tblItems.Add(wi);
                        }
                    }
                }
                else if (atvType == WfdAtvType.Sync)
                {
                    // 同步
                    // 活动实例
                    atvInst = new WfiAtv(
                        ID: await AtCm.NewID(),
                        PrciID: _info.PrcInst.ID,
                        AtvdID: ar.Def.ID,
                        Status: WfiAtvStatus.同步,
                        InstCount: 1,
                        Ctime: time,
                        Mtime: time);
                    tblAtvs.Add(atvInst);

                    // 工作项
                    WfiItem item = new WfiItem(
                        ID: await AtCm.NewID(),
                        AtviID: atvInst.ID,
                        AssignKind: WfiItemAssignKind.普通指派,
                        Status: WfiItemStatus.同步,
                        IsAccept: false,
                        UserID: AtUser.ID,
                        Sender: AtUser.Name,
                        Stime: time,
                        Ctime: time,
                        Mtime: time,
                        Dispidx: await AtCm.NewSeq("sq_wfi_item"));
                    tblItems.Add(item);
                }
                else if (atvType == WfdAtvType.Finish)
                {
                    // 完成
                    _info.PrcInst.Status = WfiAtvStatus.结束;
                    _info.PrcInst.Mtime = time;
                }

                // 增加迁移实例
                if (atvInst != null)
                {
                    var trs = await _info.CreateAtvTrs(ar.Def.ID, atvInst.ID, time, false);
                    tblTrs.Add(trs);
                }
            }

            // 发送是否有效
            // 1. 只有'完成'时有效
            // 2. 至少含有一个活动实例时有效
            if (tblAtvs.Count == 0 && _info.PrcInst.Status != WfiAtvStatus.结束)
            {
                AtKit.Msg("所有后续活动均无接收者，发送失败！");
                return;
            }
            #endregion

            #region 整理待保存数据
            List<object> data = new List<object>();
            if (_info.PrcInst.IsChanged)
                data.Add(_info.PrcInst);
            data.Add(_info.AtvInst);
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
                AtKit.Msg("发送成功！");
                _info.CloseWin();
                // 推送客户端提醒

            }
            else
            {
                AtKit.Warn("发送失败！");
            }
        }

        /// <summary>
        /// 保存当前工作项置完成状态
        /// </summary>
        /// <returns></returns>
        async Task SaveWorkItem()
        {
            List<object> ls = new List<object>();
            if (_info.AtvInst.IsChanged)
                ls.Add(_info.AtvInst);
            ls.Add(_info.WorkItem);

            if (await AtCm.BatchSave(ls, false))
            {
                AtKit.Msg(_info.AtvInst.IsFinished ? "任务结束" : "当前工作项完成");
                _info.CloseWin();
            }
            else
            {
                AtKit.Warn("工作项保存失败");
            }
        }

        /// <summary>
        /// 是否激活后续活动
        /// </summary>
        /// <param name="p_atvSync">同步活动</param>
        /// <returns></returns>
        async Task<bool> IsActive(WfdAtv p_atvSync)
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
            var trss = await AtCm.Query<WfdTrs>("流程-活动前的迁移", dt);

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
        async Task<bool> GetAllFinish(Table<WfdTrs> p_trss)
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
        async Task<bool> GetExistFinish(Table<WfdTrs> p_trss)
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
    }
}
