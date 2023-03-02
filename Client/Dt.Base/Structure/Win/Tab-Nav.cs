#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Base
{
    public partial class Tab
    {
        #region 静态内容
        public static readonly DependencyProperty OwnDlgProperty = DependencyProperty.Register(
            "OwnDlg",
            typeof(Dlg),
            typeof(Tab),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PreTabProperty = DependencyProperty.Register(
            "PreTab",
            typeof(Tab),
            typeof(Tab),
            new PropertyMetadata(null));

        public static readonly DependencyProperty NextTabProperty = DependencyProperty.Register(
            "NextTab",
            typeof(Tab),
            typeof(Tab),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register(
            "Result",
            typeof(object),
            typeof(Tab),
            new PropertyMetadata(null));

        public static readonly DependencyProperty NaviParamsProperty = DependencyProperty.Register(
            "NaviParams",
            typeof(object),
            typeof(Tab),
            new PropertyMetadata(null));
        #endregion

        #region 成员变量
        TaskCompletionSource<bool> _taskSrc;
        #endregion

        #region 属性
        /// <summary>
        /// 带遮罩时的Dlg容器
        /// </summary>
        public Dlg OwnDlg
        {
            get { return (Dlg)GetValue(OwnDlgProperty); }
            internal set { SetValue(OwnDlgProperty, value); }
        }

        /// <summary>
        /// 是否为Tab区域内导航的首页
        /// </summary>
        public bool IsHome => PreTab == null;

        /// <summary>
        /// Tab内部导航时的前一标签Tab
        /// </summary>
        internal Tab PreTab
        {
            get { return (Tab)GetValue(PreTabProperty); }
            private set { SetValue(PreTabProperty, value); }
        }

        /// <summary>
        /// Tab内部导航时的下一标签Tab
        /// </summary>
        internal Tab NextTab
        {
            get { return (Tab)GetValue(NextTabProperty); }
            private set { SetValue(NextTabProperty, value); }
        }

        /// <summary>
        /// 导航返回值
        /// </summary>
        protected object Result
        {
            get { return GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        /// <summary>
        /// 导航时的入参
        /// </summary>
        object NaviParams
        {
            get { return GetValue(NaviParamsProperty); }
            set { SetValue(NaviParamsProperty, value); }
        }
        #endregion

        #region Tab区域内导航
        /// <summary>
        /// 向前导航到新Tab，可异步等待返回值
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="p_tab">导航到新Tab</param>
        /// <param name="p_params">输入参数</param>
        /// <param name="p_isModal">WinUI模式是否带遮罩，遮罩为了禁止对其他位置编辑(用Dlg实现)</param>
        /// <returns>返回时的输出参数</returns>
        public Task<T> Forward<T>(Tab p_tab, object p_params = null, bool p_isModal = false)
        {
            if (p_tab == null)
                Throw.Msg("向前导航到的新Tab不可为空！");

            Forward(p_tab, p_params, p_isModal);
            return p_tab.StartWait<T>();
        }

        /// <summary>
        /// 向前导航到新Tab
        /// </summary>
        /// <param name="p_tab">导航到新Tab</param>
        /// <param name="p_params">输入参数</param>
        /// <param name="p_isModal">WinUI模式是否带遮罩，遮罩为了禁止对其他位置编辑(用Dlg实现)</param>
        public void Forward(Tab p_tab, object p_params = null, bool p_isModal = false)
        {
            if (p_tab == null)
                Throw.Msg("向前导航到的新Tab不可为空！");

            if (p_params != null)
                p_tab.NaviParams = p_params;

            // 建立导航关系
            p_tab.PreTab = this;
            NextTab = p_tab;

            // PhoneUI模式
            if (Kit.IsPhoneUI)
            {
                // 若当前Tab在Dlg中，向前导航的Tab都在各自的新Dlg中
                if (p_isModal || OwnDlg != null)
                {
                    ShowDlg(p_tab);
                }
                else
                {
                    PhonePage.Show(p_tab);
                }
                return;
            }

            // WinUI模式
            if (p_isModal)
            {
                // 在新Dlg中显示
                ShowDlg(p_tab);
            }
            else
            {
                p_tab.OwnWin = OwnWin;
                p_tab.OwnDlg = OwnDlg;
                p_tab.BackButtonVisibility = Visibility.Visible;
                Owner.ReplaceItem(this, p_tab);
            }
        }

        /// <summary>
        /// 向后导航到上一Tab
        /// </summary>
        public async void Backward()
        {
            if (Kit.IsPhoneUI)
            {
                // 允许返回
                if (await BeforeClose())
                {
                    if (OwnDlg == null)
                    {
                        InputKit.GoBack();
                    }
                    else
                    {
                        // 带遮罩
                        OwnDlg.Close();
                    }
                    AfterBackward();
                }
                return;
            }

            // WinUI模式
            if (PreTab != null)
            {
                // 有上一Tab
                if (await BeforeClose())
                {
                    if (OwnDlg != null && PreTab.OwnDlg != OwnDlg)
                    {
                        // 窗口的首页Tab
                        OwnDlg.Close();
                    }
                    else
                    {
                        // 就地切换
                        Owner.ReplaceItem(this, PreTab);
                    }
                    AfterBackward();
                }
            }
            else if (OwnDlg != null)
            {
                // 无上一Tab，关闭Dlg
                if (await BeforeClose())
                {
                    OwnDlg.Close();
                    AfterBackward();
                }
            }
        }

        /// <summary>
        /// 切换到指定Tab，切换后新Tab的上一Tab为首页
        /// </summary>
        /// <param name="p_tab"></param>
        /// <param name="p_showBackBtn">是否显示返回按钮</param>
        public void Toggle(Tab p_tab, bool p_showBackBtn = false)
        {
            if (p_tab == null)
                Throw.Msg("待切换的Tab不可为空！");

            // 查找当前正在显示的Tab
            Tab current = this;
            while (current.NextTab != null)
            {
                current = current.NextTab;
            }

            // 正在显示
            if (p_tab == current)
                return;

            // PhoneUI模式
            Tab home = current;
            if (Kit.IsPhoneUI)
            {
                // 向后导航到首页
                while (home.PreTab != null)
                {
                    if (home.OwnDlg != null)
                    {
                        home.OwnDlg.Close();
                    }
                    else if (UITree.RootFrame.CanGoBack)
                    {
                        UITree.RootFrame.GoBack();
                    }
                    var tab = home;
                    home = home.PreTab;
                    tab.PreTab = null;
                    tab.NextTab = null;
                }

                // 若首页Tab在Dlg中，切换的Tab也在新Dlg中
                if (home.OwnDlg != null)
                {
                    ShowDlg(p_tab);
                }
                else
                {
                    PhonePage.Show(p_tab);
                }
                return;
            }

            // WinUI模式，查找首页
            while (home.PreTab != null)
            {
                var tab = home;
                home = home.PreTab;
                tab.PreTab = null;
                tab.NextTab = null;
            }

            // 建立导航关系
            p_tab.OwnWin = home.OwnWin;
            p_tab.OwnDlg = home.OwnDlg;
            p_tab.PreTab = home;
            home.NextTab = p_tab;

            if (p_showBackBtn)
            {
                if (p_tab.OwnDlg != null)
                {
                    p_tab.BackButtonVisibility = p_tab.OwnDlg.HideTitleBar ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    p_tab.BackButtonVisibility = Visibility.Visible;
                }
            }
            else if (p_tab.BackButtonVisibility == Visibility.Visible)
            {
                p_tab.BackButtonVisibility = Visibility.Collapsed;
            }
            Owner.ReplaceItem(current, p_tab);
        }

        /// <summary>
        /// Tab区域内导航时返回到首页
        /// </summary>
        public void BackToHome()
        {
            if (Kit.IsPhoneUI)
            {
                BackToPhoneHome();
                return;
            }

            // 查找当前正在显示的Tab
            Tab current = this;
            while (current.NextTab != null)
            {
                current = current.NextTab;
            }

            // 查找首页
            Tab home = current;
            while (home.PreTab != null)
            {
                if (home.OwnDlg != null && home.PreTab.OwnDlg != OwnDlg)
                {
                    // 窗口的首页Tab，关闭
                    home.OwnDlg.Close();
                    // 关闭窗口后的最上Tab最为被切换的页
                    current = home.PreTab;
                }

                var tab = home;
                home = home.PreTab;
                tab.NextTab = null;
                tab.PreTab = null;
            }
            home.NextTab = null;

            // 切换首页
            if (home != current)
                current.Owner.ReplaceItem(current, home);
        }

        void BackToPhoneHome()
        {
            // 查找当前正在显示的Tab
            Tab current = this;
            while (current.NextTab != null)
            {
                current = current.NextTab;
            }
            // 当前是首页
            if (current.PreTab == null)
                return;

            // 向后导航到首页
            Tab home = current;
            while (home.PreTab != null)
            {
                if (home.OwnDlg != null)
                {
                    home.OwnDlg.Close();
                }
                else if (UITree.RootFrame.CanGoBack)
                {
                    UITree.RootFrame.GoBack();
                }
                var tab = home;
                home = home.PreTab;
                tab.PreTab = null;
                tab.NextTab = null;
            }
        }
        #endregion

        #region Win内导航
        /// <summary>
        /// 导航到指定Tab
        /// </summary>
        /// <param name="p_tab"></param>
        public void NaviTo(Tab p_tab)
        {
            if (Kit.IsPhoneUI && OwnWin != null && p_tab != null)
                OwnWin.NaviToSingleTab(p_tab);
        }

        /// <summary>
        /// 导航到多页Tab
        /// </summary>
        /// <param name="p_ls"></param>
        public void NaviTo(List<Tab> p_ls)
        {
            if (!Kit.IsPhoneUI || p_ls == null || p_ls.Count == 0 || OwnWin == null)
                return;

            if (p_ls.Count == 1)
            {
                NaviTo(p_ls[0]);
                return;
            }

            var titles = p_ls[0].Title;
            for (int i = 1; i < p_ls.Count; i++)
            {
                titles = $"{titles},{p_ls[i].Title}";
            }
            OwnWin.NaviToMultiTabs(titles);
        }

        /// <summary>
        /// 导航到指定页，支持多页Tab形式
        /// </summary>
        /// <param name="p_tabTitle">多个页面时用逗号隔开(自动以Tab形式显示)，null时自动导航到第一个Tab</param>
        public void NaviTo(string p_tabTitle)
        {
            if (Kit.IsPhoneUI && OwnWin != null)
                OwnWin.NaviTo(p_tabTitle);
        }
        #endregion

        #region 虚方法
        /// <summary>
        /// 初始化，Loaded事件时调用一次
        /// </summary>
        /// <param name="p_params">初始化参数</param>
        protected virtual void OnInit(object p_params)
        {
        }

        /// <summary>
        /// 后退之前
        /// </summary>
        /// <returns>true 表允许关闭</returns>
        protected virtual Task<bool> OnClosing()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 后退完成
        /// </summary>
        protected virtual void OnClosed()
        {
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 只在第一次Loaded事件时调用，始终在OnLoadTemplate后调用
        /// </summary>
        protected override void OnControlLoaded()
        {
            // 初始化
            OnInit(NaviParams);
        }

        /// <summary>
        /// Tab区域内导航时，获取首个Tab的标题
        /// </summary>
        /// <returns></returns>
        internal string GetOriginalTitle()
        {
            Tab pre = PreTab;
            if (pre != null)
            {
                while (pre.PreTab != null)
                {
                    pre = pre.PreTab;
                }
                return pre.Title;
            }
            return Title;
        }

        async Task<bool> BeforeClose()
        {
            if (await OnClosing())
            {
                StopWait();
                return true;
            }
            return false;
        }

        void AfterBackward()
        {
            OnClosed();

            // 清除导航关系
            if (PreTab != null)
            {
                PreTab.NextTab = null;
                PreTab = null;
            }
            NextTab = null;
        }

        /// <summary>
        /// 等待当前Tab返回
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <returns>返回值</returns>
        async Task<T> StartWait<T>()
        {
            _taskSrc = new TaskCompletionSource<bool>();
            await _taskSrc.Task;
            if (Result != null)
                return Result.To<T>();
            return default;
        }

        /// <summary>
        /// 结束等待当前Tab
        /// </summary>
        void StopWait()
        {
            // Forward<T>的情况
            if (_taskSrc != null && !_taskSrc.Task.IsCompleted)
            {
                _taskSrc.SetResult(true);
                _taskSrc = null;
            }
        }

        /// <summary>
        /// 用带遮罩的Dlg承载Tab，遮罩为了禁止对其他位置编辑
        /// </summary>
        /// <param name="p_tab"></param>
        void ShowDlg(Tab p_tab)
        {
            Dlg dlg;
            if (Kit.IsPhoneUI)
            {
                dlg = new Dlg()
                {
                    HideTitleBar = true,
                    IsPinned = true,
                };
            }
            else
            {
                dlg = new Dlg()
                {
                    PlacementTarget = OwnTabs,
                    WinPlacement = DlgPlacement.TargetOverlap,
                    Resizeable = false,
                    HideTitleBar = true,
                    ShowVeil = true,
                    IsPinned = true,
                    BorderThickness = new Thickness(0),
                };
            }
            dlg.LoadTab(p_tab);
            dlg.Show();
        }
        #endregion
    }
}
