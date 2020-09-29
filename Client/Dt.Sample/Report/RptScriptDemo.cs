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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public class DataRptScript : RptScript
    {
        public override Task<Table> GetData(string p_name)
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptDesignDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.数据源.{p_name}.json"))
                {
                    return Table.Create(stream);
                }
            });
        }
    }

    public class MyRptScript : DataRptScript
    {
        public override Task<Table> GetData(string p_name)
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptDesignDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.数据源.{p_name}.json"))
                {
                    var tbl = Table.Create(stream);
                    var tgt = Table.Create(tbl);
                    var ls = from row in tbl
                             where row.Str("parentid") == View.Info.Params.Str("parentid")
                             select row;
                    foreach (var row in ls)
                    {
                        tgt.Add(row);
                    }
                    return tgt;
                }
            });
        }

        public override void InitMenu(Menu p_menu)
        {
            p_menu.Items.Add(new Mi { ID = "显示网格", IsCheckable = true, Cmd = View.CmdGridLine });
            p_menu.Items.Add(new Mi { ID = "显示列头", IsCheckable = true, Cmd = View.CmdColHeader });
            p_menu.Items.Add(new Mi { ID = "显示行头", IsCheckable = true, Cmd = View.CmdRowHeader });
        }

        public override void OnCellClick(string p_id, IRptCell p_text)
        {
            
        }
    }
}