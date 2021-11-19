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
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    public class AppRptDesignInfo : RptDesignInfo
    {
        RptObj _rpt;

        public AppRptDesignInfo(RptObj p_rpt)
        {
            _rpt = p_rpt;
            Name = p_rpt.Name;
        }

        public override Task<string> ReadTemplate()
        {
            return AtCm.GetScalar<string>("报表-模板", new { id = _rpt.ID });
        }

        public override async void SaveTemplate(string p_xml)
        {
            if (!_rpt.Contains("define"))
                _rpt.AddCell<string>("define");
            _rpt["define"] = p_xml;

            if (_rpt.IsAdded)
            {
                _rpt["ctime"] = _rpt["mtime"] = Kit.Now;
            }
            else
            {
                _rpt["mtime"] = Kit.Now;
            }

            if (await AtCm.Save(_rpt))
                AtCm.PromptForUpdateModel();
        }
    }
}