#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// PhoneUI时的空白页，动态添加页面内容
    /// PhonePage.xaml文件不可删除，虽无用但uwp报错！
    /// </summary>
    public partial class PhonePage : Page
    {
        /// <summary>
        /// 导航到新页时的页面参数
        /// </summary>
        static PhonePageParameter _newParam;

        PhonePageParameter _param;

        public PhonePage()
        {
            _param = _newParam;

            // 原来在OnNavigatedFrom方法中卸载内容以便下次重用，但会造成页面返回时无动画！！！
            // 因此再次加载时需要卸载旧页面的内容！
            // elem.Parent在uno中仍然有效，uwp中因不在可视树无值，通过附加属性记录所属页面！
            var elem = _param.Content as FrameworkElement;
#if UWP
            PhonePage oldPage = PhonePageEx.GetParentPage(elem);
            if (oldPage != null)
                oldPage.Content = null;
            PhonePageEx.SetParentPage(elem, this);
#else
            if (elem.Parent is PhonePage oldPage)
                oldPage.Content = null;
#endif

            Content = elem;
        }

        /// <summary>
        /// 加载页面内容，导航到新页
        /// </summary>
        /// <param name="p_content">页面内容</param>
        public static void Show(IPhonePage p_content)
        {
            if (p_content == null || !(p_content is FrameworkElement))
                throw new Exception("PhonePage内容不是可视元素！");

            // 避免自己导航到自己的情况
            if (SysVisual.RootFrame.Content is PhonePage page
                && page.Content == p_content)
                return;

            _newParam = new PhonePageParameter(p_content, null);
            SysVisual.RootFrame.Navigate(typeof(PhonePage), _newParam);
        }

        /// <summary>
        /// 加载页面内容，导航到新页，可异步等待到关闭
        /// </summary>
        /// <param name="p_content">页面内容</param>
        public static Task ShowAsync(IPhonePage p_content)
        {
            if (p_content == null || !(p_content is FrameworkElement))
                throw new Exception("PhonePage内容不是可视元素！");

            // 避免自己导航到自己的情况
            if (SysVisual.RootFrame.Content is PhonePage page
                && page.Content == p_content)
                return Task.CompletedTask;

            var taskSrc = new TaskCompletionSource<bool>();
            _newParam = new PhonePageParameter(p_content, taskSrc);
            SysVisual.RootFrame.Navigate(typeof(PhonePage), _newParam);
            return taskSrc.Task;
        }

        /// <summary>
        /// 判断页面是否允许后退，OnNavigatingFrom中的取消导航无法实现异步等待！
        /// </summary>
        /// <returns></returns>
        internal Task<bool> IsAllowBack()
        {
            // 触发Closing事件，确定是否允许后退
            return _param.Content.OnClosing();
        }

#if UWP
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // UWP页面未缓存，后退时为下一页准备页面参数
            if (e.NavigationMode == NavigationMode.Back)
                _newParam = (PhonePageParameter)e.Parameter;
        }
#endif

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                // 结束异步等待
                if (_param.TaskSource != null && !_param.TaskSource.Task.IsCompleted)
                {
                    _param.TaskSource.SetResult(true);
                    _param.TaskSource = null;
                }
                // 触发Closed事件
                _param.Content.OnClosed();
            }
        }
    }
}