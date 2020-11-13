#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Report;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Model
{
    public class AppRptDesignInfo : RptDesignInfo
    {
        Rpt _rpt;

        public AppRptDesignInfo(Rpt p_rpt)
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
                _rpt["ctime"] = _rpt["mtime"] = AtSys.Now;
            }
            else
            {
                _rpt["mtime"] = AtSys.Now;
            }

            await AtCm.Save(_rpt);
        }
    }
}