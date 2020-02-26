#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 任务栏
    /// </summary>
    public partial class Taskbar : Control
    {
        #region 成员变量
        const double _maxWidth = 260;
        double _itemWidth;
        static Win _autoStartWin;

        /// <summary>
        /// 获取桌面实例
        /// </summary>
        public static Taskbar Inst { get; internal set; }

        /// <summary>
        /// 任务栏项数据源
        /// </summary>
        internal readonly ObservableCollection<TaskbarItem> TaskItems;
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public Taskbar()
        {
            DefaultStyleKey = typeof(Taskbar);
            Inst = this;

            TaskItems = new ObservableCollection<TaskbarItem>();
            if (_autoStartWin != null)
            {
                // 存在自启动窗口，因创建窗口时Taskbar还不存在
                var item = new TaskbarItem();
                item.DataContext = _autoStartWin;
                item.IsActive = true;
                TaskItems.Add(item);
                _autoStartWin = null;
            }
            TaskItems.CollectionChanged += OnTaskItemsChanged;
            SizeChanged += OnSizeChanged;
        }

        #region 外部方法
        /// <summary>
        /// 加载并激活窗口对应的任务栏按扭
        /// </summary>
        /// <param name="p_win"></param>
        internal static void LoadTaskItem(Win p_win)
        {
            if (Inst == null)
            {
                // 存在自启动窗口时Taskbar还不存在
                _autoStartWin = p_win;
                return;
            }

            TaskbarItem item = Inst.GetTaskItem(p_win);
            if (item == null)
            {
                item = new TaskbarItem();
                item.DataContext = p_win;
                Inst.TaskItems.Add(item);
            }
            Inst.ActiveTaskItem(p_win);
        }

        /// <summary>
        /// 返回待激活的窗口
        /// </summary>
        /// <param name="p_win"></param>
        /// <returns></returns>
        internal Win GetNextActiveItem(Win p_win)
        {
            if (TaskItems.Count <= 1)
                return null;

            int next = -1;
            for (int i = 0; i < TaskItems.Count; i++)
            {
                var item = TaskItems[i];
                if ((Win)item.DataContext == p_win)
                {
                    if (item.IsActive)
                        next = i + 1;
                    break;
                }
            }

            if (next == -1)
                return null;

            // 最后位置
            if (next >= TaskItems.Count)
                next = TaskItems.Count - 2;
            return (Win)TaskItems[next].DataContext;
        }

        /// <summary>
        /// 移除任务栏按扭
        /// </summary>
        /// <param name="p_win"></param>
        /// <returns></returns>
        internal void RemoveTaskItem(Win p_win)
        {
            for (int i = 0; i < TaskItems.Count; i++)
            {
                if ((Win)TaskItems[i].DataContext == p_win)
                {
                    TaskItems.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 激活指定的任务栏按扭
        /// </summary>
        /// <param name="p_win"></param>
        internal void ActiveTaskItem(Win p_win)
        {
            foreach (var item in TaskItems)
            {
                item.IsActive = ((Win)item.DataContext == p_win);
            }
        }

        /// <summary>
        /// 激活指定的任务栏按扭
        /// </summary>
        /// <param name="p_item"></param>
        internal void ActiveTaskItem(TaskbarItem p_item)
        {
            foreach (var item in TaskItems)
            {
                item.IsActive = (p_item == item);
            }
        }

        /// <summary>
        /// 将所有按扭置为非激活状态
        /// </summary>
        internal void RemoveActiveState()
        {
            foreach (var item in TaskItems)
            {
                item.IsActive = false;
            }
        }

        /// <summary>
        /// 根据ID获取任务栏按扭
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        TaskbarItem GetTaskItem(Win p_win)
        {
            return (from elem in TaskItems
                    where elem != null && elem.DataContext == p_win
                    select elem).FirstOrDefault();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 应用模板
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ItemsControl con = GetTemplateChild("TaskItemsControl") as ItemsControl;
            if (con != null)
                con.ItemsSource = TaskItems;

            Button btn = GetTemplateChild("HomePageButton") as Button;
            if (btn != null)
            {
                btn.Click -= OnShowHomePage;
                btn.Click += OnShowHomePage;
            }
        }

        void OnTaskItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResizeAllItems();
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeAllItems();
        }

        /// <summary>
        /// 重置任务栏按扭的宽度
        /// </summary>
        void ResizeAllItems()
        {
            if (ActualWidth < 300 || TaskItems.Count == 0)
                return;

            double width = Math.Floor((ActualWidth - 180) / TaskItems.Count);
            if (width < _maxWidth && width != _itemWidth)
            {
                _itemWidth = width;
                foreach (var item in TaskItems)
                {
                    item.Width = _itemWidth;
                }
            }
        }

        /// <summary>
        /// 显示主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnShowHomePage(object sender, RoutedEventArgs e)
        {
            Desktop.Inst.ShowHomePage();
            RemoveActiveState();
        }
        #endregion
    }
}
