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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 发起任务
    /// </summary>
    public sealed partial class WfSendDlg
    {
        WfFormInfo _info;

        public WfSendDlg()
        {
            InitializeComponent();
        }

        public void Show(WfFormInfo p_info)
        {
            _info = p_info;
            if (!AtSys.IsPhoneUI)
            {
                Height = 400;
                Width = 500;
            }

            foreach (var recv in _info.NextRecvs)
            {
                CreateItem(recv);
            }
            Show();
        }

        void OnSend(object sender, Mi e)
        {
            int count = 0;
            foreach (var lv in _pnl.Children.OfType<Lv>())
            {
                AtvRecv ar = (AtvRecv)lv.Tag;
                if (lv.SelectedCount > 0)
                {
                    count++;
                    ar.SelectedRecvs = (from r in lv.SelectedRows
                                        select r.ID).ToList();
                }
                else if (_info.AtvDef.TransKind == WfdAtvTransKind.全部)
                {
                    AtKit.Msg(ar.Def.Name + "的执行者不能为空！");
                    return;
                }
            }

            if (_info.AtvDef.TransKind == WfdAtvTransKind.自由选择 && count == 0)
            {
                AtKit.Msg("请至少选择一个活动的执行者！");
                return;
            }
            if (_info.AtvDef.TransKind == WfdAtvTransKind.独占式选择 && count == 0)
            {
                AtKit.Msg("请选择一个活动的执行者！");
                return;
            }
            if (_info.AtvDef.TransKind == WfdAtvTransKind.独占式选择 && count > 1)
            {
                AtKit.Msg("【" + _info.AtvDef.Name + "】只能选择一个后续活动！");
                return;
            }

            _info.CmdSend.DoSend(true);
            Close();
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
                Background = AtRes.浅灰背景,
                BorderBrush = AtRes.浅灰边框,
                BorderThickness = new Thickness(0, 0, 0, 1),
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
                            new TextBlock { FontFamily = AtRes.IconFont, Text = "\uE045", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) },
                            new TextBlock { Text = p_ar.Def.Name, TextWrapping = TextWrapping.NoWrap, VerticalAlignment = VerticalAlignment.Center }
                        }
                    },
                },
            };

            Lv lv = new Lv { View = Resources["ViewTemp"], ViewMode = ViewMode.Tile, Data = p_ar.Recvs, Tag = p_ar };
            if (p_ar.MultiSelection)
            {
                var sp = new StackPanel { Orientation = Orientation.Horizontal };
                var btn = new Button { Content = "全选", Tag = lv, Background = AtRes.TransparentBrush };
                btn.Click += OnSelectAll;
                sp.Children.Add(btn);
                btn = new Button { Content = "清除", Tag = lv, Background = AtRes.TransparentBrush };
                btn.Click += OnClearSelection;
                sp.Children.Add(btn);

                Grid.SetColumn(sp, 1);
                grid.Children.Add(sp);

                lv.SelectionMode = Base.SelectionMode.Multiple;
            }
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
    }
}
