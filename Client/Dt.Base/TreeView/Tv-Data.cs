#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.TreeViews;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 数据源相关
    /// </summary>
    public partial class Tv
    {
        #region 静态内容
        public readonly static DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(ITreeData),
            typeof(Tv),
            new PropertyMetadata(null, OnDataChanged));

        public readonly static DependencyProperty FixedRootProperty = DependencyProperty.Register(
            "FixedRoot",
            typeof(object),
            typeof(Tv),
            new PropertyMetadata(null));

        public static readonly DependencyProperty FilterCfgProperty = DependencyProperty.Register(
            "FilterCfg",
            typeof(FilterCfg),
            typeof(Tv),
            new PropertyMetadata(null, OnFilterCfgChanged));

        static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Tv tv = (Tv)d;
            if (tv._dataView != null)
                tv._dataView.Destroy();

            if (e.NewValue == null)
            {
                tv._dataView = null;
                tv.ClearItems();
            }
            else
            {
                tv._dataView = new TvDataView(tv, (ITreeData)e.NewValue);
            }

            tv._dataView?.Refresh();
            tv.OnDataChanged();
        }

        static void OnFilterCfgChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Tv tv = (Tv)d;
            if (tv._isLoaded)
                tv._panel.Reload();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置数据源对象，Table已实现ITreeData
        /// </summary>
        public ITreeData Data
        {
            get { return (ITreeData)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// 获取设置固定根节点，切换数据源时不变
        /// </summary>
        public object FixedRoot
        {
            get { return GetValue(FixedRootProperty); }
            set { SetValue(FixedRootProperty, value); }
        }

        /// <summary>
        /// 获取设置筛选框配置，默认null
        /// </summary>
        public FilterCfg FilterCfg
        {
            get { return (FilterCfg)GetValue(FilterCfgProperty); }
            set { SetValue(FilterCfgProperty, value); }
        }
        #endregion

        /// <summary>
        /// 加载数据行
        /// </summary>
        internal void LoadItems()
        {
            ClearSelectionOnDataChanged();
            if (_isLoaded)
                _panel.OnRowsChanged();
        }

        /// <summary>
        /// 清空所有行
        /// </summary>
        internal void ClearItems()
        {
            ClearAllTvItems();
            ClearSelectionOnDataChanged();
            if (_isLoaded)
                _panel.OnRowsChanged();
        }
        
        void ClearSelectionOnDataChanged()
        {
            if (_selectedRows.Count > 0)
            {
                // 中间过程，不触发事件
                _selectedRows.CollectionChanged -= OnSelectedItemsChanged;
                _selectedRows.Clear();
                _selectedRows.CollectionChanged += OnSelectedItemsChanged;
            }
        }

        /// <summary>
        /// 清空并释放所有的TvItem
        /// </summary>
        internal void ClearAllTvItems()
        {
            if (RootItems.Count > 0)
            {
                // 统一清除
                TvCleaner.Add(RootItems);
                RootItems = new TvRootItems(this);
            }
        }
    }
}