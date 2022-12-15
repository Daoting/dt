#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Lv属性
    /// </summary>
    public partial class CList
    {
        /// <summary>
        /// 获取Lv对象
        /// </summary>
        public Lv Lv => _lv;

        /// <summary>
        /// 获取设置数据源对象，Table或集合对象
        /// </summary>
        public INotifyList Data
        {
            get { return _lv.Data; }
            set { _lv.Data = value; }
        }

        /// <summary>
        /// 获取设置行视图，DataTemplate、DataTemplateSelector、Cols列定义 或 IRowView
        /// </summary>
        public object View
        {
            get { return _lv.View; }
            set { _lv.View = value; }
        }

        /// <summary>
        /// 获取设置视图类型：列表、表格、磁贴，默认List
        /// </summary>
        public ViewMode ViewMode
        {
            get { return _lv.ViewMode; }
            set { _lv.ViewMode = value; }
        }

        /// <summary>
        /// 获取设置自定义行/项目样式的回调方法
        /// </summary>
        public Action<ItemStyleArgs> ItemStyle
        {
            get { return _lv.ItemStyle; }
            set { _lv.ItemStyle = value; }
        }

        /// <summary>
        /// 获取设置Phone模式下的视图类型，null时Win,Phone两模式统一采用ViewMode，默认null
        /// </summary>
        public ViewMode? PhoneViewMode
        {
            get { return _lv.PhoneViewMode; }
            set { _lv.PhoneViewMode = value; }
        }

        /// <summary>
        /// 获取设置选择模式，默认Single，只第一次设置有效！
        /// </summary>
        [CellParam("选择模式")]
        public SelectionMode SelectionMode
        {
            get { return _lv.SelectionMode; }
            set { _lv.SelectionMode = value; }
        }

        /// <summary>
        /// 获取设置筛选框配置，默认null
        /// </summary>
        public FilterCfg FilterCfg
        {
            get { return _lv.FilterCfg; }
            set { _lv.FilterCfg = value; }
        }

        /// <summary>
        /// 获取设置顶部的工具栏
        /// </summary>
        public Menu Toolbar
        {
            get { return _lv.Toolbar; }
            set { _lv.Toolbar = value; }
        }

    }
}