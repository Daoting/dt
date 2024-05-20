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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    [RptScript]
    public class DataRptScript : RptScript
    {
        public override Task<Table> GetData(string p_name)
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptHome).Assembly.GetManifestResourceStream($"Dt.UIDemo.Files.Embed.数据源.{p_name}.json"))
                {
                    return Table.Create(stream);
                }
            });
        }
    }
    
    [RptScript]
    public class MyRptScript : DataRptScript
    {
        static Dict _curParams;
        
        public override void InitParams(Dict p_dict)
        {
            if (_curParams == null)
            {
                p_dict["parentid"] = "";
                p_dict["parentname"] = "根菜单";
            }
            else
            {
                p_dict["parentid"] = _curParams["parentid"];
                p_dict["parentname"] = _curParams["parentname"];
            }
        }
        
        public override Task<Table> GetData(string p_name)
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptHome).Assembly.GetManifestResourceStream($"Dt.UIDemo.Files.Embed.数据源.{p_name}.json"))
                {
                    var tbl = Table.Create(stream);
                    var tgt = Table.Clone(tbl);
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

        public override Task RenderCell(Cells.Data.Cell p_cell, RptCellArgs p_args)
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
            return Task.CompletedTask;
        }

        public override void InitMenu(Menu p_menu)
        {
            Mi mi = new Mi { ID = "后退", Icon = Icons.向左 };
            mi.Click += OnBack;
            p_menu.Items.Insert(0, mi);
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
                    OnBack(null);
                }
            }
            else if (p_args.Col == 1)
            {
                if (row.Bool("isgroup"))
                {
                    var info = new RptInfo
                    {
                        Uri = "embedded://Dt.UIDemo/Dt.UIDemo.Files.Embed.模板.交互脚本.rpt",
                    };
                    _curParams = new Dict { { "parentid", row.Str("id") }, { "parentname", row.Str("name") } };
                    
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

        void OnBack(Mi e)
        {
            var ls = View.Tag as Stack<RptInfo>;
            if (ls != null && ls.Count > 0)
            {
                var info = ls.Pop();
                _curParams = info.Params;
                View.LoadReport(info);
            }
        }

        void OnDetail(Mi e)
        {
            Dlg dlg = new Dlg();
            dlg.Content = new TextBlock { Margin = new Thickness(20), Text = $"id：{View.Info.Params["parentid"]}\r\nname：{View.Info.Params["parentname"]}" };
            dlg.Show();
        }
    }
}