﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Tools;
using Dt.Core;
using Dt.Core.Model;
using Dt.Core.Rpc;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// UI交互代理类
    /// </summary>
    public static class AtUI
    {
        #region 窗口
        ///// <summary>
        ///// 根据菜单id打开菜单项窗口
        ///// </summary>
        ///// <param name="p_menuID">菜单ID</param>
        ///// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        //public static object OpenMenu(string p_menuID)
        //{
        //    return OpenMenu(AtLocal.QueryModelFirst<OmMenu>($"select * from OmMenu where id='{p_menuID}'"));
        //}

        ///// <summary>
        ///// 打开菜单项窗口，可以由点击菜单项或直接代码构造Menu的方式调用
        ///// </summary>
        ///// <param name="p_menu">OmMenu实例</param>
        ///// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        //public static object OpenMenu(OmMenu p_menu)
        //{
        //    if (p_menu == null)
        //    {
        //        AtKit.Msg("打开菜单项不可为空！");
        //        return null;
        //    }

        //    Type tp = GetViewType(p_menu.ViewName);
        //    if (tp == null)
        //    {
        //        AtKit.Msg(string.Format("打开菜单时未找到视图【{0}】！", p_menu.ViewName));
        //        return null;
        //    }

        //    Icons icon;
        //    Enum.TryParse(p_menu.Icon, out icon);
        //    object win = OpenWin(tp, p_menu.Name, icon, string.IsNullOrEmpty(p_menu.Params) ? null : p_menu.Params);

        //    // 保存点击次数，用于确定哪些是收藏菜单
        //    if (win != null && !AtSys.Stub.IsLocalMode)
        //    {
        //        Task.Run(() =>
        //        {
        //            if (AtLocal.GetModelScalar<int>($"select count(id) from ommenu where id=\"{p_menu.ID}\"") > 0)
        //            {
        //                // 点击次数保存在客户端
        //                Dict dt = new Dict();
        //                dt["userid"] = AtUser.ID;
        //                dt["menuid"] = p_menu.ID;
        //                int cnt = AtLocal.Execute("update menufav set clicks=clicks+1 where userid=:userid and menuid=:menuid", dt);
        //                if (cnt == 0)
        //                    AtLocal.Execute("insert into menufav (userid, menuid, clicks) values (:userid, :menuid, 1)", dt);
        //            }
        //            // 收集使用频率
        //            //await AtAuth.ClickMenu(p_menu.ID);
        //        });
        //    }
        //    return win;
        //}

        /// <summary>
        /// 根据视图名称激活旧窗口或打开新窗口
        /// </summary>
        /// <param name="p_viewName">窗口视图名称</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">启动参数</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenView(
            string p_viewName,
            string p_title = null,
            Icons p_icon = Icons.None,
            string p_params = null)
        {
            Type tp = GetViewType(p_viewName);
            if (tp == null)
            {
                AtKit.Msg(string.Format("【{0}】视图未找到！", p_viewName));
                return null;
            }
            return OpenWin(tp, p_title, p_icon, p_params);
        }

        /// <summary>
        /// 根据窗口类型和参数激活旧窗口或打开新窗口
        /// </summary>
        /// <param name="p_type">窗口类型</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">初始参数</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenWin(
            Type p_type,
            string p_title = null,
            Icons p_icon = Icons.None,
            string p_params = null)
        {
            if (p_type == null)
                AtKit.Throw("待显示的窗口类型不可为空！");

            // 激活旧窗口，比较窗口类型和初始参数
            IWin win;
            if (!AtSys.IsPhoneUI && (win = Desktop.Inst.ActiveWin(p_type, p_params)) != null)
            {
                Taskbar.Inst.ActiveTaskItem(win);
                return win;
            }

            // 打开新窗口
            TypeInfo info = p_type.GetTypeInfo();
            if (info.ImplementedInterfaces.Contains(typeof(IWin)))
            {
                if (string.IsNullOrEmpty(p_params))
                    win = (IWin)Activator.CreateInstance(p_type);
                else
                    win = (IWin)Activator.CreateInstance(p_type, p_params);

                if (string.IsNullOrEmpty(win.Title) && string.IsNullOrEmpty(p_title))
                    win.Title = "无标题";
                else if (!string.IsNullOrEmpty(p_title))
                    win.Title = p_title;

                if (p_icon != Icons.None)
                    win.Icon = p_icon;

                if (AtSys.IsPhoneUI)
                {
                    win.NaviToHome();
                }
                else
                {
                    Taskbar.LoadTaskItem(win);
                    Desktop.Inst.ShowNewWin(win);
                }
                return win;
            }

            // 处理自定义启动情况
            if (info.ImplementedInterfaces.Contains(typeof(IView)))
            {
                IView viewer = Activator.CreateInstance(p_type) as IView;
                viewer.Run(p_params);
                return viewer;
            }

            AtKit.Msg("打开窗口失败，窗口类型需要实现IWin或IView接口！");
            return null;
        }

        /// <summary>
        /// 获取视图类型
        /// </summary>
        /// <param name="p_typeName">类型名称</param>
        /// <returns>返回类型</returns>
        public static Type GetViewType(string p_typeName)
        {
            Type tp;
            if (!string.IsNullOrEmpty(p_typeName) && AtSys.Stub.ViewTypes.TryGetValue(p_typeName, out tp))
                return tp;
            return null;
        }
        #endregion

        #region 可视区域
        /// <summary>
        /// 可视区域宽度
        /// 手机：页面宽度
        /// PC上：除标题栏和外框的窗口内部宽度
        /// </summary>
        public static double ViewWidth
        {
            get { return ((FrameworkElement)Window.Current.Content).ActualWidth; }
        }

        /// <summary>
        /// 可视区域高度
        /// 手机：不包括状态栏的高度
        /// PC上：除标题栏和外框的窗口内部高度
        /// </summary>
        public static double ViewHeight
        {
            get
            {
                // ApplicationView.GetForCurrentView().VisibleBounds在uno中大小不正确！！！
                // Android上高度多50
                return ((FrameworkElement)Window.Current.Content).ActualHeight - SysVisual.StatusBarHeight;
            }
        }
        #endregion

        #region 工具
        /// <summary>
        /// 确保TextBox的Text实时更新到数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnUpdateSource(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            // 确保实时更新到数据源
            BindingExpression expresson = tb.GetBindingExpression(TextBox.TextProperty);
            if (expresson != null)
                expresson.UpdateSource();
        }

        /// <summary>
        /// 设置自启动
        /// </summary>
        /// <param name="p_win"></param>
        public static void SetAutoStart(IWin p_win)
        {
            if (p_win == null)
                return;

            AutoStartInfo info = new AutoStartInfo();
            info.WinType = p_win.GetType().AssemblyQualifiedName;
            info.Params = p_win.Params;
            info.Title = p_win.Title;
            info.Icon = p_win.Icon.ToString();
            AtLocal.SaveAutoStart(info);
            AtKit.Msg(string.Format("{0}已设置自启动！", p_win.Title));
        }

        /// <summary>
        /// 取消自启动
        /// </summary>
        public static void DelAutoStart()
        {
            AtLocal.DelAutoStart();
            AtKit.Msg("已取消自启动设置！");
        }
        #endregion

        #region 标准视图名称
        /// <summary>
        /// 主页视图名称
        /// </summary>
        public const string HomeView = "主页";

        /// <summary>
        /// 单机模式主页视图
        /// </summary>
        public const string LocalHomeView = "单机主页";
        #endregion

        #region Phone模式标题菜单
        static MenuFlyout _menu;

        /// <summary>
        /// Phone模式附加标题右键菜单
        /// </summary>
        /// <param name="p_elem">标题元素</param>
        /// <param name="p_win">所属窗口</param>
        internal static void OnPhoneTitleTapped(FrameworkElement p_elem, IWin p_win)
        {
            if (p_elem == null || p_win == null)
                return;

            p_elem.Tapped += (s, e) =>
            {
                if (_menu == null)
                {
                    _menu = new MenuFlyout();
                    _menu.Placement = FlyoutPlacementMode.Bottom;
                    var item = new MenuFlyoutItem { Text = "取消自启动" };
                    item.Command = new BaseCommand((p_params) => DelAutoStart());
                    _menu.Items.Add(item);
                    _menu.Items.Add(new MenuFlyoutItem { Text = "设置自启动" });
                    item = new MenuFlyoutItem { Text = "系统监视" };
                    item.Command = new BaseCommand((p_params) => SysTrace.ShowBox());
                    _menu.Items.Add(item);
                }

                var autoStart = AtLocal.GetAutoStart();
                if (autoStart != null
                    && autoStart.WinType == p_win.GetType().AssemblyQualifiedName
                    && autoStart.Params == p_win.Params)
                {
                    _menu.Items[0].Visibility = Visibility.Visible;
                    _menu.Items[1].Visibility = Visibility.Collapsed;
                }
                else
                {
                    _menu.Items[0].Visibility = Visibility.Collapsed;
                    _menu.Items[1].Visibility = Visibility.Visible;
                    ((MenuFlyoutItem)_menu.Items[1]).Command = new BaseCommand((p_params) => SetAutoStart(p_win));
                }
                _menu.ShowAt(p_elem);
            };
        }
        #endregion
    }
}