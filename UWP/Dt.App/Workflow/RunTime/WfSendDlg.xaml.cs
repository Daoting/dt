#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 发起任务
    /// </summary>
    public sealed partial class WfSendDlg : Dlg
    {
        WfFormInfo _info;

        public WfSendDlg()
        {
            InitializeComponent();
        }

        public Task<bool> Show(WfFormInfo p_info)
        {
            _info = p_info;
            if (!Kit.IsPhoneUI)
            {
                Height = 400;
                Width = 500;
            }

            // 普通活动
            foreach (var recv in _info.NextRecvs.Atvs)
            {
                if (recv.Recvs != null && recv.Recvs.Count > 0)
                    CreateItem(recv);
            }

            // 同步活动
            if (_info.NextRecvs.SyncAtv != null)
                CreateItem(_info.NextRecvs.SyncAtv);

            if (_info.NextRecvs.FinishedAtv != null)
                _m[1].Visibility = Visibility.Visible;
            return ShowAsync();
        }

        void OnSend(object sender, Mi e)
        {
            int count = 0;
            foreach (var lv in _pnl.Children.OfType<Lv>())
            {
                AtvRecv ar = (AtvRecv)lv.Tag;
                ar.SelectedRecvs = null;
                if (lv.SelectedCount > 0)
                {
                    count++;
                    ar.SelectedRecvs = (from r in lv.SelectedRows
                                        select r.ID).ToList();
                }
                else if (_info.AtvDef.TransKind == WfdAtvTransKind.全部)
                {
                    Kit.Warn($"未选择{ar.Def.Name}的执行者！");
                    return;
                }
            }

            if (_info.AtvDef.TransKind == WfdAtvTransKind.自由选择 && count == 0)
            {
                Kit.Warn("请至少选择一个活动的执行者！");
                return;
            }
            if (_info.AtvDef.TransKind == WfdAtvTransKind.独占式选择 && count == 0)
            {
                Kit.Warn("请选择一个活动的执行者！");
                return;
            }
            if (_info.AtvDef.TransKind == WfdAtvTransKind.独占式选择 && count > 1)
            {
                Kit.Warn("只能选择一个活动的执行者！");
                return;
            }
            Close(true);
        }

        /// <summary>
        /// 每个活动接收者对应一个Lv
        /// </summary>
        /// <param name="p_ar"></param>
        /// <returns></returns>
        void CreateItem(AtvRecv p_ar)
        {
            // 分组栏
            Grid grid = new Grid
            {
                Background = Res.浅灰1,
                BorderBrush = Res.浅灰2,
                BorderThickness = new Thickness(0, 1, 0, 1),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto },
                },
                Children =
                {
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(10, 0, 10, 0),
                        Children =
                        {
                            new TextBlock { FontFamily = Res.IconFont, Text = "\uE02D", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) },
                            new TextBlock { Text = p_ar.Def.Name, TextWrapping = TextWrapping.NoWrap, VerticalAlignment = VerticalAlignment.Center }
                        }
                    },
                },
                Height = 40,
            };

            Lv lv = new Lv { View = Resources["ViewTemp"], ViewMode = ViewMode.Tile, Data = p_ar.Recvs, Tag = p_ar, Margin = new Thickness(0, 0, 0, 20) };
            var sp = new StackPanel { Orientation = Orientation.Horizontal };
            var btn = new Button { Content = "清除", Tag = lv, Background = Res.TransparentBrush };
            btn.Click += OnClearSelection;
            sp.Children.Add(btn);

            if (p_ar.MultiSelection && p_ar.Recvs.Count > 1)
            {
                btn = new Button { Content = "全选", Tag = lv, Background = Res.TransparentBrush };
                btn.Click += OnSelectAll;
                sp.Children.Add(btn);
                lv.SelectionMode = Base.SelectionMode.Multiple;
            }

            Grid.SetColumn(sp, 1);
            grid.Children.Add(sp);

            _pnl.Children.Add(grid);
            _pnl.Children.Add(lv);
        }

        void OnSelectAll(object sender, RoutedEventArgs e)
        {
            ((Lv)((Button)sender).Tag).SelectAll();
        }

        void OnClearSelection(object sender, RoutedEventArgs e)
        {
            ((Lv)((Button)sender).Tag).ClearSelection();
        }

        async void OnFinish(object sender, Mi e)
        {
            if (await Kit.Confirm("任务结束(完成)后将不可修改，确认完成吗？"))
            {
                _info.NextRecvs.FinishedAtv.IsSelected = true;
                Close(true);
            }
        }
    }
}
