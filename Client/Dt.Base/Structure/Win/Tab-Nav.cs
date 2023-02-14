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
            new PropertyMetadata(null, OnPreTabChanged));

        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register(
            "Result",
            typeof(object),
            typeof(Tab),
            new PropertyMetadata(null));

        static void OnPreTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tab = (Tab)d;
            if (e.NewValue != null)
            {
                if (tab.OwnDlg != null)
                {
                    tab.BackButtonVisibility = tab.OwnDlg.HideTitleBar ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (!Kit.IsPhoneUI)
                {
                    tab.BackButtonVisibility = Visibility.Visible;
                }
            }
            else
            {
                tab.BackButtonVisibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region 成员变量
        TaskCompletionSource<bool> _taskSrc;
        object _params;
        #endregion

        /// <summary>
        /// 带遮罩时的Dlg容器
        /// </summary>
        public Dlg OwnDlg
        {
            get { return (Dlg)GetValue(OwnDlgProperty); }
            internal set { SetValue(OwnDlgProperty, value); }
        }

        /// <summary>
        /// Tab内部导航时的前一标签Tab
        /// </summary>
        internal Tab PreTab
        {
            get { return (Tab)GetValue(PreTabProperty); }
            private set { SetValue(PreTabProperty, value); }
        }

        /// <summary>
        /// 导航返回值
        /// </summary>
        protected object Result
        {
            get { return GetValue(ResultProperty); }
            private set { SetValue(ResultProperty, value); }
        }

        #region Tab内导航
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
            p_tab._params = p_params;
            if (p_isModal)
            {
                ShowDlg(p_tab);
                return;
            }

            if (Kit.IsPhoneUI)
            {
                PhonePage.Show(p_tab);
                return;
            }

            p_tab.OwnWin = OwnWin;
            p_tab.OwnDlg = OwnDlg;
            p_tab.PreTab = this;
            Owner.ReplaceItem(this, p_tab);
        }

        /// <summary>
        /// 向后导航到上一内容
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
                    AfterClosed();
                }
                return;
            }

            // WinUI模式
            if (PreTab != null)
            {
                // 有上一Tab
                if (await BeforeClose())
                {
                    Owner.ReplaceItem(this, PreTab);
                    AfterClosed();
                }
            }
            else if (OwnDlg != null)
            {
                // 无上一Tab，关闭Dlg
                if (await BeforeClose())
                {
                    OwnDlg.Close();
                    AfterClosed();
                }
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
            OnInit(_params);
        }

        /// <summary>
        /// Tab内部导航时，获取首个Tab的标题
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

        void AfterClosed()
        {
            OnClosed();
        }

        /// <summary>
        /// 等待当前Mv返回
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
        /// 结束等待当前Mv
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
                    PlacementTarget = p_tab.OwnTabs,
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
