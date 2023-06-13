#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 延时刷新
    /// </summary>
    public partial class Lv
    {
        #region 静态内容
        public readonly static DependencyProperty DeferLoadPanelProperty = DependencyProperty.Register(
            "DeferLoadPanel",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(false));

        public readonly static DependencyProperty DeferRefreshDataProperty = DependencyProperty.Register(
            "DeferRefreshData",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(false));

        public readonly static DependencyProperty DeferReloadProperty = DependencyProperty.Register(
            "DeferReload",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(false));
        #endregion

        /// <summary>
        /// 延迟刷新数据和视图
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// using (_lv.Defer())
        /// {
        ///     _lv.View = Resources["TableView"];
        ///     _lv.ViewMode = ViewMode.Table;
        ///     
        /// }
        /// </code>
        /// </example>
        public IDisposable Defer()
        {
            return new InternalCls(this);
        }

        int _updating;
        
        internal int Updating
        {
            get { return _updating; }
            set
            {
                _updating = value;
                if (_updating == 0)
                {
                    if (DeferLoadPanel)
                    {
                        LoadPanel();
                        ClearValue(DeferLoadPanelProperty);
                    }

                    if (DeferRefreshData)
                    {
                        _dataView?.Refresh();
                        ClearValue(DeferRefreshDataProperty);
                    }

                    if (DeferReload)
                    {
                        ReloadPanelContent();
                        ClearValue(DeferReloadProperty);
                    }
                }
            }
        }

        /// <summary>
        /// 是否延时调用 LoadPanel()
        /// </summary>
        internal bool DeferLoadPanel
        {
            get { return (bool)GetValue(DeferLoadPanelProperty); }
            set { SetValue(DeferLoadPanelProperty, value); }
        }

        /// <summary>
        /// 是否延时调用 _dataView.Refresh()
        /// </summary>
        internal bool DeferRefreshData
        {
            get { return (bool)GetValue(DeferRefreshDataProperty); }
            set { SetValue(DeferRefreshDataProperty, value); }
        }

        /// <summary>
        /// 是否延时调用 _panel.Reload()
        /// </summary>
        internal bool DeferReload
        {
            get { return (bool)GetValue(DeferReloadProperty); }
            set { SetValue(DeferReloadProperty, value); }
        }

        class InternalCls : IDisposable
        {
            Lv _owner;

            public InternalCls(Lv p_owner)
            {
                _owner = p_owner;
                _owner.Updating = _owner.Updating + 1;
            }

            public void Dispose()
            {
                _owner.Updating = _owner.Updating - 1;
            }
        }
    }
}