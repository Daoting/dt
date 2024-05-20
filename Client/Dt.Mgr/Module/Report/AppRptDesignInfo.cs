#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Mgr.Rbac;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Module
{
    public class AppRptDesignInfo : RptDesignInfo
    {
        RptX _rpt;

        public AppRptDesignInfo(RptX p_rpt)
        {
            _rpt = p_rpt;
            Name = p_rpt.Name;
            ShowSave = true;
        }

        public override Task<string> ReadTemplate()
        {
            return At.GetScalar<string>($"select define from cm_rpt where id={_rpt.ID}");
        }

        public override async Task<bool> SaveTemplate(string p_xml)
        {
            if (!_rpt.Contains("define"))
                _rpt.Add<string>("define");
            _rpt["define"] = p_xml;

            if (_rpt.IsAdded)
            {
                _rpt["ctime"] = _rpt["mtime"] = Kit.Now;
            }
            else
            {
                _rpt["mtime"] = Kit.Now;
            }

            return await _rpt.Save();
        }
    }
}