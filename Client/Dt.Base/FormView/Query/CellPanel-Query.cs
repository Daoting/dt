#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 单元格内部布局面板，查询相关
    /// </summary>
    public partial class CellPanel : Panel
    {
        const double _queryWidth = 40;
        static Menu _valMenu;
        static Menu _strMenu;

        internal void OnQueryFlagChanged()
        {
            if (_btnQuery == null)
                return;

            switch (_owner.QueryFlag)
            {
                case CompFlag.Equal:
                    _btnQuery.Content = "=";
                    _btnQuery.SetToolTip("等于");
                    break;

                case CompFlag.Unequal:
                    _btnQuery.Content = "≠";
                    _btnQuery.SetToolTip("不等于");
                    break;

                case CompFlag.Less:
                    _btnQuery.Content = "<";
                    _btnQuery.SetToolTip("小于");
                    break;

                case CompFlag.Ceil:
                    _btnQuery.Content = "≤";
                    _btnQuery.SetToolTip("小于等于");
                    break;

                case CompFlag.Greater:
                    _btnQuery.Content = ">";
                    _btnQuery.SetToolTip("大于");
                    break;

                case CompFlag.Floor:
                    _btnQuery.Content = "≥";
                    _btnQuery.SetToolTip("大于等于");
                    break;

                case CompFlag.StartsWith:
                    _btnQuery.Content = "a..";
                    _btnQuery.SetToolTip("以...开头");
                    break;

                case CompFlag.EndsWith:
                    _btnQuery.Content = "..a";
                    _btnQuery.SetToolTip("以...结束");
                    break;

                case CompFlag.Contains:
                    _btnQuery.Content = ".a.";
                    _btnQuery.SetToolTip("包含");
                    break;

                default:
                    _btnQuery.Content = "🛇";
                    _btnQuery.SetToolTip("忽略，不参与查询");
                    break;
            }
        }

        void OnQueryClick(object sender, RoutedEventArgs e)
        {
            // 不可编辑或无编辑器时
            if (_owner.Query != QueryType.Editable
                || _owner.ValBinding.Source is not ICell cell)
                return;

            if (cell.Type == typeof(string))
            {
                if (_strMenu == null)
                    CreateStrMenu();
                _strMenu.Tag = _owner;
                _strMenu.OpenContextMenu((Button)sender);
            }
            else
            {
                if (_valMenu == null)
                    CreateValMenu();
                _valMenu.Tag = _owner;
                _valMenu.OpenContextMenu((Button)sender);
            }
        }

        static void CreateValMenu()
        {
            _valMenu = new Menu { IsContextMenu = true, Placement = MenuPosition.BottomLeft };
            _valMenu.ItemClick += OnMiClick;
            Mi item = new Mi()
            {
                ID = "等于",
                Tag = CompFlag.Equal,
            };
            _valMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "不等于",
                Tag = CompFlag.Unequal,
            };
            _valMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "大于",
                Tag = CompFlag.Greater,
            };
            _valMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "大于等于",
                Tag = CompFlag.Floor,
            };
            _valMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "小于",
                Tag = CompFlag.Less,
            };
            _valMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "小于等于",
                Tag = CompFlag.Ceil,
            };

            item = new Mi()
            {
                ID = "忽略",
                Tag = CompFlag.Ignore,
            };
            _valMenu.Items.Add(item);
        }

        static void CreateStrMenu()
        {
            _strMenu = new Menu { IsContextMenu = true, Placement = MenuPosition.BottomLeft };
            _strMenu.ItemClick += OnMiClick;
            Mi item = new Mi()
            {
                ID = "以...开头",
                Tag = CompFlag.StartsWith,
            };
            _strMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "包含",
                Tag = CompFlag.Contains,
            };
            _strMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "以...结束",
                Tag = CompFlag.EndsWith,
            };
            _strMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "等于",
                Tag = CompFlag.Equal,
            };
            _strMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "不等于",
                Tag = CompFlag.Unequal,
            };

            item = new Mi()
            {
                ID = "忽略",
                Tag = CompFlag.Ignore,
            };
            _strMenu.Items.Add(item);
        }

        static void OnMiClick(object sender, Mi e)
        {
            ((FvCell)e.Owner.Tag).QueryFlag = (CompFlag)e.Tag;
        }
    }
}