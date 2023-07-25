#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Mgr.Workflow
{
    public partial class CurrentTasks : Win
    {
        public CurrentTasks()
        {
            InitializeComponent();

            _lv.Loaded += (s, e) => Refresh();
        }

        async void Refresh()
        {
            _lv.Data = await WfdDs.GetMyTodoTasks();
        }

        void OnRefresh(object sender, Mi e)
        {
            Refresh();
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            if (InputKit.IsCtrlPressed)
            {
                AtWf.OpenFormWin(new WfFormInfo(e.Row.Long("prcd_id"), e.Row.Long("item_id"), WfFormUsage.Edit));
            }
            else if (e.IsChanged)
            {
                var info = new WfFormInfo(e.Row.Long("prcd_id"), e.Row.Long("item_id"), WfFormUsage.Edit);
                var win = await AtWf.CreateFormWin(info);
                info.FormClosed += (s, arg) => Refresh();
                LoadMain(win);
            }
        }
    }

    [LvCall]
    public class CurrentTasksUI
    {
        public static void FormatTitle(Env e)
        {
            Grid grid = new Grid { ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Auto }, new ColumnDefinition { Width = GridLength.Auto }, new ColumnDefinition { Width = GridLength.Auto } } };
            var tbName = new TextBlock { Margin = new Thickness(0, 0, 4, 0), VerticalAlignment = VerticalAlignment.Center };
            grid.Children.Add(tbName);

            var rc = new Rectangle { Fill = Res.深灰2 };
            Grid.SetColumn(rc, 1);
            grid.Children.Add(rc);

            var tbAtv = new TextBlock { Margin = new Thickness(4, 2, 4, 2), Foreground = Res.WhiteBrush };
            Grid.SetColumn(tbAtv, 1);
            grid.Children.Add(tbAtv);

            rc = new Rectangle();
            Grid.SetColumn(rc, 2);
            grid.Children.Add(rc);

            var tbInfo = new TextBlock { Margin = new Thickness(4, 2, 4, 2), Foreground = Res.WhiteBrush };
            Grid.SetColumn(tbInfo, 2);
            grid.Children.Add(tbInfo);
            e.UI = grid;

            e.Set += c =>
            {
                tbName.Text = c.Row.Str("formname");
                tbAtv.Text = c.Row.Str("atvname");

                var kind = (WfiItemAssignKind)c.Row.Int("assign_kind");
                switch (kind)
                {
                    case WfiItemAssignKind.起始指派:
                        rc.Fill = Res.中绿;
                        tbInfo.Text = "发起";
                        break;

                    case WfiItemAssignKind.回退:
                        rc.Fill = Res.亮红;
                        tbInfo.Text = "回退";
                        break;

                    case WfiItemAssignKind.追回:
                        rc.Fill = Res.亮蓝;
                        tbInfo.Text = "追回";
                        break;

                    case WfiItemAssignKind.跳转:
                        rc.Fill = Res.BlackBrush;
                        tbInfo.Text = "跳转";
                        break;
                }
            };
        }
    }
}