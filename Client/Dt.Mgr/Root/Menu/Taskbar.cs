#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Tools;
using Dt.Mgr.Home;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.Specialized;
#endregion

namespace Dt.Mgr
{
    public class Taskbar : ITaskbar
    {
        Dlg _dlgMenu;
        Dlg _dlgNotice;

        /// <summary>
        /// 获取任务栏左侧的界面
        /// </summary>
        /// <returns></returns>
        public FrameworkElement GetStartUI()
        {
            var item = new Tray { Icon = Icons.收藏夹, MinWidth = 80 };
            ToolTipService.SetToolTip(item, "收藏夹");
            item.Click += BtnFavClick;
            MenuDs.FixedMenuLoaded += () => item.Notice();
            return item;
        }

        void BtnFavClick(Tray e)
        {
            if (_dlgMenu == null)
            {
                _dlgMenu = new Dlg
                {
                    HideTitleBar = true,
                    WinPlacement = DlgPlacement.FromTopLeft,
                    Width = 400,
                    Height = Math.Ceiling(Kit.ViewHeight * 3 / 5),
                    BorderThickness = new Thickness(4),
                    BorderBrush = Res.主蓝,
                    ClipElement = e,
                };
                _dlgMenu.LoadTab(new FavMenu());
            }
            _dlgMenu.Show();
        }

        /// <summary>
        /// 获取任务栏右侧的托盘界面
        /// </summary>
        /// <returns></returns>
        public FrameworkElement GetTrayUI()
        {
            var item = new Tray { Icon = Icons.铃铛, MinWidth = 80 };
            ToolTipService.SetToolTip(item, "通知");
            item.Click += BtnNoticeClick;
            Notice.Data.CollectionChanged += (s, e) => item.Digit = Notice.Data.Count;
            return item;
        }

        void BtnNoticeClick(Tray e)
        {
            if (_dlgNotice == null)
            {
                _dlgNotice = new Dlg
                {
                    HideTitleBar = true,
                    WinPlacement = DlgPlacement.FromTopRight,
                    Width = 300,
                    Height = Math.Ceiling(Kit.ViewHeight * 3 / 5),
                    BorderThickness = new Thickness(4),
                    BorderBrush = Res.主蓝,
                    ClipElement = e,
                };
                _dlgNotice.LoadTab(new NoticeCenter());
            }
            _dlgNotice.Show();
        }
    }

    public static class Notice
    {
        /// <summary>
        /// 添加到通知
        /// </summary>
        /// <param name="p_info"></param>
        public static void Add(NotifyInfo p_info)
        {
            var row = Data.NewRow(new
            { 
                time = Kit.Now.ToString("HH:mm:ss"),
                message = p_info.Message,
                level = p_info.NotifyType == NotifyType.Warning ? Res.RedBrush : Res.BlackBrush,
            });
            row.Tag = p_info;
            Data.Insert(0, row);
        }

        public static readonly Table Data = new Table
        {
            { "time" },
            { "message" },
            { "level", typeof(SolidColorBrush) },
        };
    }
}
