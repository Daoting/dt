#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cm.Workflow;
using Dt.Core;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static Dt.Core.MySqlAccess;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 流程Api (Workflow)
    /// </summary>
    [Api]
    public class Wf : BaseApi
    {

        /// <summary>
        /// 获取活动可迁移的后续活动的接收者列表
        /// </summary>
        /// <param name="p_atvID">当前活动模板标识</param>
        /// <param name="p_prciID">流程实例标识</param>
        /// <returns></returns>
        public async Task<Dict> GetNextRecvs(long p_atvID, long p_prciID)
        {
            // 获得所有后续活动，包括同步
            var nextAtvs = await _dp.Query<WfdAtv>("流程-后续活动", new { atvid = p_atvID });
            if (nextAtvs == null || nextAtvs.Count == 0)
                return null;

            List<WfdAtv> atvs = new List<WfdAtv>();
            // 合并同步活动的后续活动，同步的后续活动不可回退！
            foreach (var atv in nextAtvs)
            {
                // 同步且可激活后续活动时合并
                if (atv.Type == WfdAtvType.Sync && await IsActive(atv, p_prciID, p_atvID))
                {
                    var tblNext = await _dp.Query<WfdAtv>("流程-后续活动", new { atvid = atv.ID });
                    if (tblNext.Count > 0)
                    {
                        atvs.AddRange(tblNext);
                    }
                }
            }
            if (atvs.Count > 0)
                atvs.ForEach(a => nextAtvs.Add(a));

            // 按照活动获取接收者
            // 活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动
            // 执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户
            // 执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者
            Dict result = new Dict();
            Dict allAtv = new Dict();
            result["atv"] = allAtv;
            bool manualSend = false;
            int cnt = 0;

            foreach (var atv in nextAtvs)
            {
                string id = atv.ID.ToString();
                var tp = atv.Type;

                // 2:同步活动 3:结束活动
                if (tp == WfdAtvType.Sync || tp == WfdAtvType.Finish)
                {
                    allAtv[id] = null;
                    continue;
                }

                Dict info = new Dict();
                allAtv[id] = info;
                Dict dtSql = new Dict { { "atvid", id } };

                if (atv.ExecLimit != 0)
                {
                    // 有限制，按过滤后的用户发送
                    info["isrole"] = false;
                    var users = await GetAtvUsers(dtSql);
                    if (users.Count == 0)
                    {
                        // 无授权用户
                        info["data"] = null;
                    }
                    else
                    {
                        long atvdid = (atv.ExecLimit == WfdAtvExecLimit.已完成活动的执行者 || atv.ExecLimit == WfdAtvExecLimit.已完成活动的同部门执行者) ? atv.ExecAtvID.Value : atv.ID;
                        var datas = await GetLimitUsers(p_prciID, atvdid, atv.ExecLimit);
                        if (datas == null || datas.Count == 0)
                        {
                            // 没有符合限制的用户
                            info["data"] = null;
                        }
                        else
                        {
                            // 取已授权用户和符合限制用户的交集
                            Table tblJoin = new Table
                            {
                                {"id", typeof(long) },
                                {"name" }
                            };
                            foreach (var uid in datas)
                            {
                                foreach (var r in users)
                                {
                                    if (r.Long("id") == uid)
                                    {
                                        tblJoin.AddRow(new { id = uid, name = r["name"] });
                                        break;
                                    }
                                }
                            }
                            info["data"] = tblJoin;
                        }
                    }
                    // 除‘所有用户’外其余手动发送
                    if (atv.ExecScope != WfdAtvExecScope.所有用户)
                        manualSend = true;
                }
                else
                {
                    // 无限制
                    if (atv.ExecScope == WfdAtvExecScope.一组用户 || atv.ExecScope == WfdAtvExecScope.单个用户)
                    {
                        // 一组用户或单个用户，所有授权用户为被选项
                        manualSend = true;
                        info["isrole"] = false;
                        info["data"] = await GetAtvUsers(dtSql);
                    }
                    else
                    {
                        // 所有用户或任一用户，按角色发
                        info["isrole"] = true;
                        info["data"] = await _dp.Query("流程-活动的所有授权角色", dtSql);
                    }
                }
                cnt++;
            }

            // 当后续迁移活动为独占式选择且后续活动多于一个时手动选择
            if (!manualSend
                && cnt > 1
                && await _dp.GetScalar<byte>("流程-活动的后续迁移方式", new { id = p_atvID }) == 2)
            {
                manualSend = true;
            }
            result["manualsend"] = manualSend;
            return result;
        }

        /// <summary>
        /// 是否激活后续活动
        /// </summary>
        /// <param name="p_atvSync">同步活动</param>
        /// <param name="p_prciID">流程实例标识</param>
        /// <param name="p_atvID">活动模板标识</param>
        /// <returns></returns>
        async Task<bool> IsActive(WfdAtv p_atvSync, long p_prciID, long p_atvID)
        {
            Dict dt = new Dict();
            dt["prciid"] = p_prciID;
            dt["atvdid"] = p_atvSync.ID;
            int cnt = await _dp.GetScalar<int>("流程-同步活动实例数", dt);

            // 已产生同步实例
            if (cnt > 0)
                return false;

            // 获得同步前所有活动
            dt = new Dict();
            dt["TgtAtvID"] = p_atvSync.ID;
            var trss = await _dp.Query<WfdTrs>("流程-活动前的迁移", dt);

            // 聚合方式
            // 全部
            if (p_atvSync.JoinKind == 0)
                return await GetAllFinish(trss, p_atvID, p_prciID);

            // 任一
            if (p_atvSync.JoinKind == WfdAtvJoinKind.任一任务)
                return true;

            // 即时
            return await GetExistFinish(trss, p_atvID, p_prciID);
        }

        /// <summary>
        /// 获得同步前的活动是否已经都完成
        /// </summary>
        /// <param name="p_trss"></param>
        /// <param name="p_atvID"></param>
        /// <param name="p_prciID"></param>
        /// <returns></returns>
        async Task<bool> GetAllFinish(Table<WfdTrs> p_trss, long p_atvID, long p_prciID)
        {
            bool finish = true;
            foreach (var trs in p_trss)
            {
                if (trs.SrcAtvID == p_atvID)
                    continue;

                var dt = new Dict();
                dt["atvdid"] = trs.SrcAtvID;
                dt["prciid"] = p_prciID;
                int cnt = await _dp.GetScalar<int>("流程-活动结束的实例数", dt);
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
        /// <param name="p_atvID"></param>
        /// <param name="p_prciID"></param>
        /// <returns></returns>
        async Task<bool> GetExistFinish(Table<WfdTrs> p_trss, long p_atvID, long p_prciID)
        {
            bool finish = true;
            foreach (var trs in p_trss)
            {
                if (trs.SrcAtvID == p_atvID)
                    continue;

                var dt = new Dict();
                dt["atvdid"] = trs.SrcAtvID;
                dt["prciid"] = p_prciID;
                var tbl = await _dp.Query("流程-活动实例的状态", dt);
                if (tbl != null && tbl.Count > 0 && tbl[0].Int("status") != 1)
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
        /// <param name="p_dt"></param>
        /// <returns></returns>
        async Task<Table> GetAtvUsers(Dict p_dt)
        {
            if (await _dp.GetScalar<int>("流程-是否活动授权任何人", p_dt) == 0)
                return await _dp.Query("流程-活动的所有执行者", p_dt);
            return await _dp.Query("select id, name from cm_user where expired = 0");
        }

        async Task<List<long>> GetLimitUsers(long p_prciID, long p_atvdid, WfdAtvExecLimit p_execLimit)
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
            return (await _dp.EachItem<long>(key, new { prciId = p_prciID, atvdid = p_atvdid })).ToList();
        }

    }
}
