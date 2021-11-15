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

namespace Dt.Sample
{
    public class MyRptDesignInfo : RptDesignInfo
    {
        public override Task<string> ReadTemplate()
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.模板.{Name}.xml"))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            });
        }

        public override void SaveTemplate(string p_xml)
        {
            DataPackage data = new DataPackage();
            data.SetText(p_xml);
            Clipboard.SetContent(data);
            Kit.Msg("已保存到剪切板！");
        }
    }

    public class MyRptInfo : RptInfo
    {
        public override Task<string> ReadTemplate()
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.模板.{Name}.xml"))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            });
        }
    }

    public class DataRptScript : RptScript
    {
        public override Task<Table> GetData(string p_name)
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.数据源.{p_name}.json"))
                {
                    return Table.Create(stream);
                }
            });
        }
    }

    public class RptSearchFormScript : DataRptScript
    {
        public override IRptSearchForm GetSearchForm(RptInfo p_info)
        {
            return new CustomSearchForm(p_info);
        }
    }

    public class MyRptScript : DataRptScript
    {
        public override Task<Table> GetData(string p_name)
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.数据源.{p_name}.json"))
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

        public override void RenderCell(Cells.Data.Cell p_cell, RptCellArgs p_args)
        {
            if (p_args.Col == 1)
            {
                string name = View.Info.Params.Str("parentname");
                if (name == "根菜单")
                {
                    p_cell.Value = name;
                    p_cell.Foreground = Res.BlackBrush;
                }
                else
                {
                    p_cell.Value = "<- " + name;
                }
            }
            else if (p_args.Col == 2)
            {
                if (p_args.Data.Bool("isgroup"))
                {
                    p_cell.Value = "V";
                    p_cell.Foreground = Res.GreenBrush;
                }
                else
                {
                    p_cell.Value = "X";
                    p_cell.Foreground = Res.RedBrush;
                }
            }
        }

        public override void InitMenu(Menu p_menu)
        {
            Mi mi = new Mi { ID = "后退", Icon = Icons.向左 };
            mi.Click += OnBack;
            p_menu.Items.Insert(0, mi);
            p_menu.Items.Add(new Mi { ID = "显示网格", IsCheckable = true, Cmd = View.CmdGridLine });
        }

        public override void OpenContextMenu(Menu p_contextMenu)
        {
            Mi back = p_contextMenu["后退"];
            if (back == null)
            {
                back = new Mi { ID = "后退", Icon = Icons.向左 };
                back.Click += OnBack;
                p_contextMenu.Items.Insert(0, back);
            }
            var ls = View.Tag as Stack<RptInfo>;
            back.Visibility = (ls != null && ls.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            if (p_contextMenu["菜单详情"] == null)
            {
                Mi mi = new Mi { ID = "菜单详情", Icon = Icons.文件 };
                mi.Click += OnDetail;
                p_contextMenu.Items.Add(mi);
            }
        }

        public override void OnCellClick(RptCellArgs p_args)
        {
            var row = p_args.Data;
            if (p_args.Row == 0 && p_args.Col == 1)
            {
                if (p_args.Text != "根菜单")
                {
                    OnBack(null, null);
                }
            }
            else if (p_args.Col == 1)
            {
                if (row.Bool("isgroup"))
                {
                    var info = new MyRptInfo { Name = "脚本", Params = new Dict { { "parentid", row.Str("id") }, { "parentname", row.Str("name") } } };
                    var ls = View.Tag as Stack<RptInfo>;
                    if (ls == null)
                    {
                        ls = new Stack<RptInfo>();
                        View.Tag = ls;
                    }
                    ls.Push(View.Info);
                    View.LoadReport(info);
                }
                else
                {
                    Dlg dlg = new Dlg();
                    var pnl = new StackPanel
                    {
                        Children =
                    {
                        new TextBlock { Text = "id：" + row.Str("id")},
                        new TextBlock { Text = "parentid：" + row.Str("parentid")},
                        new TextBlock { Text = "name：" + row.Str("name")},
                        new TextBlock { Text = "isgroup：" + row.Str("isgroup")},
                    },
                        Margin = new Thickness(20),
                    };
                    dlg.Content = pnl;
                    dlg.Show();
                }
            }
            else if (p_args.Col == 2)
            {
                Kit.Msg(row.Bool("isgroup") ? "分组菜单" : "实体菜单");
            }
        }

        void OnBack(object sender, Mi e)
        {
            var ls = View.Tag as Stack<RptInfo>;
            if (ls != null && ls.Count > 0)
                View.LoadReport(ls.Pop());
        }

        void OnDetail(object sender, Mi e)
        {
            Dlg dlg = new Dlg();
            dlg.Content = new TextBlock { Margin = new Thickness(20), Text = $"id：{View.Info.Params["parentid"]}\r\nname：{View.Info.Params["parentname"]}" };
            dlg.Show();
        }
    }
}