#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 保存命令
    /// </summary>
    public class WfSaveCmd : BaseCommand
    {
        WfFormInfo _info;

        public WfSaveCmd(WfFormInfo p_info)
        {
            _info = p_info;
        }

        protected override void DoExecute(object p_parameter)
        {
            _info.RunCmd(Save);
        }

        public async Task<bool> Save()
        {
            // 先保存表单数据
            if (!await _info.FormWin.Save())
                return false;

            // 标题
            string name = _info.FormWin.GetPrcName();
            if (name != _info.PrcInst.Name)
            {
                _info.PrcInst.Name = name;
                _info.FormWin.Title = name;
            }

            bool suc = true;
            if (_info.PrcInst.IsAdded)
            {
                DateTime time = Kit.Now;
                _info.PrcInst.Ctime = time;
                _info.PrcInst.Mtime = time;
                _info.PrcInst.Dispidx = await AtCm.NewSeq("sq_wfi_prc");

                _info.AtvInst.Ctime = time;
                _info.AtvInst.Mtime = time;

                _info.WorkItem.AcceptTime = time;
                _info.WorkItem.Dispidx = await AtCm.NewSeq("sq_wfi_item");
                _info.WorkItem.Ctime = time;
                _info.WorkItem.Mtime = time;
                _info.WorkItem.Stime = time;
            }

            List<object> ls = new List<object>();
            if (_info.PrcInst.IsAdded || _info.PrcInst.IsChanged)
                ls.Add(_info.PrcInst);
            if (_info.AtvInst.IsAdded || _info.AtvInst.IsChanged)
                ls.Add(_info.AtvInst);
            if (_info.WorkItem.IsAdded || _info.WorkItem.IsChanged)
                ls.Add(_info.WorkItem);

            if (ls.Count > 0)
                suc = await AtCm.BatchSave(ls, false);
            return suc;
        }
    }
}
