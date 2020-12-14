#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 回退命令
    /// </summary>
    public class WfRollbackCmd : BaseCommand
    {
        WfFormInfo _info;

        public WfRollbackCmd(WfFormInfo p_info)
        {
            _info = p_info;
            AllowExecute = true;
        }

        protected override void DoExecute(object p_parameter)
        {
            _info.RunCmd(Rollback);
        }

        async Task Rollback()
        {
            // 获得前一活动实例
            var pre = await _info.AtvInst.GetRollbackAtv();
            if (pre == null)
            {
                AtKit.Msg("该活动不允许进行回退！");
                return;
            }

            // 活动执行者多于一人时，不允许进行回退
            if (_info.AtvInst.InstCount > 1)
            {
                AtKit.Msg("该活动执行者多于一人，不允许进行回退！");
                return;
            }

            DateTime time = AtSys.Now;
            var newAtvInst = new WfiAtv(
                ID: await AtCm.NewID(),
                PrciID: _info.PrcInst.ID,
                AtvdID: pre.AtvdID,
                Status: WfiAtvStatus.活动,
                InstCount: 1,
                Ctime: time,
                Mtime: time);

            // 创建迁移实例
            var newTrs = await _info.CreateAtvTrs(pre.AtvdID, newAtvInst.ID, time, true);

            // 全部完成时，将当前活动实例状态置成完成
            await _info.AtvInst.UpdateFinished();

            // 当前活动项置成完成状态
            _info.WorkItem.Finished();

            Dict dict = new Dict();
            dict["name"] = await GetSender();
            long userId = await AtCm.GetScalar<long>("流程-获取用户ID", dict);
            var newItem = await WfiItem.Create(newAtvInst.ID, time, false, userId, null, true);

            List<object> ls = new List<object>();
            if (_info.AtvInst.IsChanged)
                ls.Add(_info.AtvInst);
            ls.Add(_info.WorkItem);
            ls.Add(newAtvInst);
            ls.Add(newItem);
            ls.Add(newTrs);

            if (await AtCm.BatchSave(ls, false))
            {
                AtKit.Msg("回退成功！");
                _info.CloseWin();
            }
            else
            {
                AtKit.Msg("回退失败！");
            }
        }

        async Task<string> GetSender()
        {
            string sender = _info.WorkItem.Sender;
            if (_info.WorkItem.AssignKind == WfiItemAssignKind.回退)
            {
                Dict dt = new Dict();
                dt["prciid"] = _info.AtvInst.PrciID;
                dt["atvdid"] = _info.AtvInst.AtvdID;
                long id = await AtCm.GetScalar<long>("流程-最后已完成活动ID", dt);
                if (id != 0)
                {
                    dt = new Dict();
                    dt["atviid"] = id;
                    sender = await AtCm.GetScalar<string>("流程-活动发送者", dt);
                }
            }
            return sender;
        }
    }
}
