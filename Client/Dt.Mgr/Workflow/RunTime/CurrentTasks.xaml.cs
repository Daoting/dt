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
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Mgr.Workflow
{
    public partial class CurrentTasks : Tab
    {
        StartWorkflow _dlgStart;

        public CurrentTasks()
        {
            InitializeComponent();

            if (!Kit.IsPhoneUI)
                _lv.ViewMode = ViewMode.Table;
            _lv.FirstLoaded(Refresh);
        }

        async void Refresh()
        {
            _lv.Data = await WfdDs.GetMyTodoTasks();
        }

        void StartNewWf()
        {
            if (_dlgStart == null)
                _dlgStart = new StartWorkflow();
            _dlgStart.Show();
        }

        void HisTasks()
        {
            Kit.OpenWin(typeof(HistoryTasks), "历史任务", Icons.拆信);
        }

        void OnItemDoubleClick(object sender, object e)
        {
            OpenForm((Row)e);
        }

        void OnOpenForm(object sender, Mi e)
        {
            OpenForm((Row)e.Data);
        }

        void OnOpenLog(object sender, Mi e)
        {
            var row = (Row)e.Data;
            AtWf.ShowLog(row.Long("prci_id"), row.Long("prcd_id"));
        }

        void OnOpenList(object sender, Mi e)
        {
            var prc = ((Row)e.Data).Str("prcname");
            var tp = Kit.GetTypeByAlias(typeof(WfListAttribute), prc);
            Throw.IfNull(tp, $"未指定 [{prc}] 的管理窗口类型，请在管理窗口类型上添加 [WfList(\"{prc}\")] 标签！");
            Kit.OpenWin(tp, prc);
        }

        void OpenForm(Row p_row)
        {
            AtWf.OpenFormWin(p_itemID: p_row.Long("item_id"));
        }
    }

    [LvCall]
    public class CurrentTasksUI
    {
        public static void FormatKind(Env e)
        {
            var tbInfo = new TextBlock
            {
                FontFamily = Res.IconFont,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            e.UI = tbInfo;

            e.Set += c =>
            {
                var kind = (WfiItemAssignKind)c.Row.Int("assign_kind");
                switch (kind)
                {
                    case WfiItemAssignKind.起始指派:
                        tbInfo.Foreground = Res.中绿;
                        tbInfo.Text = "\uE02D";
                        ToolTipService.SetToolTip(e.Dot, "发起");
                        break;

                    case WfiItemAssignKind.回退:
                        tbInfo.Foreground = Res.亮红;
                        tbInfo.Text = "\uE036";
                        ToolTipService.SetToolTip(e.Dot, "回退");
                        break;

                    case WfiItemAssignKind.追回:
                        tbInfo.Foreground = Res.亮蓝;
                        tbInfo.Text = "\uE07C";
                        ToolTipService.SetToolTip(e.Dot, "追回");
                        break;

                    case WfiItemAssignKind.跳转:
                        tbInfo.Foreground = Res.亮红;
                        tbInfo.Text = "\uE07D";
                        ToolTipService.SetToolTip(e.Dot, "跳转");
                        break;

                    default:
                        tbInfo.Foreground = Res.BlackBrush;
                        tbInfo.Text = "\uE079";
                        ToolTipService.SetToolTip(e.Dot, "普通");
                        break;
                }
            };
        }

        public static void FormatNote(Env e)
        {
            var tbInfo = new TextBlock
            {
                FontFamily = Res.IconFont,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "\uE07F",
                Foreground = Res.主蓝,
            };
            e.Dot.Background = Res.TransparentBrush;
            e.UI = tbInfo;

            e.Set += c =>
            {
                e.Dot.Tapped -= OnShowNote;
                if (c.Str == "")
                {
                    tbInfo.Visibility = Visibility.Collapsed;
                }
                else
                {
                    tbInfo.Visibility = Visibility.Visible;
                    e.Dot.Tag = c.Str;
                    e.Dot.Tapped += OnShowNote;
                }
            };
        }

        static void OnShowNote(object sender, TappedRoutedEventArgs e)
        {
            var dot = (Dot)sender;
            Dlg dlg = new Dlg
            {
                Title = "留言",
                Content = new TextBlock { Text = dot.Tag.ToString(), Margin = new Thickness(10), TextWrapping = TextWrapping.Wrap },
                PlacementTarget = dot,
                WinPlacement = DlgPlacement.TargetBottomLeft,
                Width = 300,
                Height = 300,
            };
            dlg.Show();
        }

        public static void AtvAndLog(Env e)
        {
            TextBlock tb = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = Res.主蓝,
            };
            tb.Tapped += OnShowLog;
            e.UI = tb;

            e.Set += c =>
            {
                tb.Text = c.Str;
                tb.Tag = c.Row;
            };
        }

        static void OnShowLog(object sender, TappedRoutedEventArgs e)
        {
            var tb = sender as TextBlock;
            if (tb != null && tb.Tag is Row row)
                AtWf.ShowLog(row.Long("prci_id"), row.Long("prcd_id"));
        }
    }
}