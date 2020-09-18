#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Dt.Core;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表描述信息
    /// 报表模板三种方式优先级：
    /// 1. 外部直接提供RptRoot对象
    /// 2. 通过Uri加载模板
    /// 3. 通过Name查询数据加载模板（调试状态在服务器端db，运行状态在本地db）
    /// </summary>
    public partial class RptInfo : DependencyObject
    {
        #region 静态内容
        /// <summary>
        /// 报表模板的位置
        /// </summary>
        public readonly static DependencyProperty UriProperty = DependencyProperty.Register(
            "Uri",
            typeof(Uri),
            typeof(RptInfo),
            new PropertyMetadata(null));

        /// <summary>
        /// 报表标题
        /// </summary>
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(RptInfo),
            new PropertyMetadata(null));

        /// <summary>
        /// 是否隐藏报表查询面板（报表组时无效），默认false
        /// </summary>
        public readonly static DependencyProperty HideSearchFormProperty = DependencyProperty.Register(
            "HideSearchForm",
            typeof(bool),
            typeof(RptInfo),
            new PropertyMetadata(false));

        /// <summary>
        /// 初次加载时是否自动执行查询，前提是Params参数值提供完备，默认false
        /// </summary>
        public readonly static DependencyProperty AutoQueryProperty = DependencyProperty.Register(
            "AutoQuery",
            typeof(bool),
            typeof(RptInfo),
            new PropertyMetadata(false));

        /// <summary>
        /// 是否显示查询菜单项，默认false
        /// </summary>
        public readonly static DependencyProperty ShowSearchMiProperty = DependencyProperty.Register(
            "ShowSearchMi",
            typeof(bool),
            typeof(RptInfo),
            new PropertyMetadata(false));

        /// <summary>
        /// 是否显示导出菜单项，默认true
        /// </summary>
        public readonly static DependencyProperty ShowExportMiProperty = DependencyProperty.Register(
            "ShowExportMi",
            typeof(bool),
            typeof(RptInfo),
            new PropertyMetadata(true));

        /// <summary>
        /// 是否显示打印菜单项，默认true
        /// </summary>
        public readonly static DependencyProperty ShowPrintMiProperty = DependencyProperty.Register(
            "ShowPrintMi",
            typeof(bool),
            typeof(RptInfo),
            new PropertyMetadata(true));

        /// <summary>
        /// Worksheet是否显示列头
        /// </summary>
        public readonly static DependencyProperty ShowColHeaderProperty = DependencyProperty.Register(
            "ShowColHeader",
            typeof(bool),
            typeof(RptInfo),
            new PropertyMetadata(false, OnShowColHeaderChanged));

        /// <summary>
        /// Worksheet是否显示行头
        /// </summary>
        public readonly static DependencyProperty ShowRowHeaderProperty = DependencyProperty.Register(
            "ShowRowHeader",
            typeof(bool),
            typeof(RptInfo),
            new PropertyMetadata(false, OnShowRowHeaderChanged));

        /// <summary>
        /// Worksheet是否显示网格
        /// </summary>
        public readonly static DependencyProperty ShowGridLineProperty = DependencyProperty.Register(
            "ShowGridLine",
            typeof(bool),
            typeof(RptInfo),
            new PropertyMetadata(false, OnShowGridLineChanged));

        /// <summary>
        /// AppBar关闭时的显示方式，Mini版有效
        /// </summary>
        public readonly static DependencyProperty ClosedDisplayModeProperty = DependencyProperty.Register(
            "ClosedDisplayMode",
            typeof(AppBarClosedDisplayMode),
            typeof(RptInfo),
            new PropertyMetadata(AppBarClosedDisplayMode.Minimal));

        static void OnShowColHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RptInfo info = (RptInfo)d;
            if (info._sheet != null)
                info._sheet.ColumnHeader.IsVisible = (bool)e.NewValue;
        }

        static void OnShowRowHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RptInfo info = (RptInfo)d;
            if (info._sheet != null)
                info._sheet.RowHeader.IsVisible = (bool)e.NewValue;
        }

        static void OnShowGridLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RptInfo info = (RptInfo)d;
            if (info._sheet != null)
                info._sheet.ShowGridLine = (bool)e.NewValue;
        }
        #endregion

        #region 成员变量
        protected Dict _params;
        protected Worksheet _sheet;
        protected Dictionary<string, RptData> _dataSet;
        #endregion

        public RptInfo(string p_name)
        {
            Name = p_name;
        }

        /// <summary>
        /// 获取设置报表名称，作为唯一标识识别窗口用
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取设置报表标题，未设置时使用报表名称
        /// </summary>
        public string Title
        {
            get
            {
                if (this.ExistLocalValue(TitleProperty))
                    return (string)GetValue(TitleProperty);
                return Name;
            }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置报表数据的查询参数，初始化时做为预输入参数
        /// </summary>
        public Dict Params
        {
            get { return _params; }
            set { _params = value; }
        }

        /// <summary>
        /// 获取设置报表模板的位置
        /// </summary>
        public Uri Uri
        {
            get { return (Uri)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        /// <summary>
        /// 获取设置是否隐藏报表查询面板（报表组时无效），默认false
        /// </summary>
        public bool HideSearchForm
        {
            get { return (bool)GetValue(HideSearchFormProperty); }
            set { SetValue(HideSearchFormProperty, value); }
        }

        /// <summary>
        /// 获取设置初次加载时是否自动执行查询，前提是Params参数值提供完备，默认false
        /// </summary>
        public bool AutoQuery
        {
            get { return (bool)GetValue(AutoQueryProperty); }
            set { SetValue(AutoQueryProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示查询菜单项，默认false
        /// </summary>
        public bool ShowSearchMi
        {
            get { return (bool)GetValue(ShowSearchMiProperty); }
            set { SetValue(ShowSearchMiProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示导出菜单项，默认true
        /// </summary>
        public bool ShowExportMi
        {
            get { return (bool)GetValue(ShowExportMiProperty); }
            set { SetValue(ShowExportMiProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示打印菜单项，默认true
        /// </summary>
        public bool ShowPrintMi
        {
            get { return (bool)GetValue(ShowPrintMiProperty); }
            set { SetValue(ShowPrintMiProperty, value); }
        }

        /// <summary>
        /// 获取设置Worksheet是否显示列头
        /// </summary>
        public bool ShowColHeader
        {
            get { return (bool)GetValue(ShowColHeaderProperty); }
            set { SetValue(ShowColHeaderProperty, value); }
        }

        /// <summary>
        /// 获取设置Worksheet是否显示行头
        /// </summary>
        public bool ShowRowHeader
        {
            get { return (bool)GetValue(ShowRowHeaderProperty); }
            set { SetValue(ShowRowHeaderProperty, value); }
        }

        /// <summary>
        /// 获取设置Worksheet是否显示网格
        /// </summary>
        public bool ShowGridLine
        {
            get { return (bool)GetValue(ShowGridLineProperty); }
            set { SetValue(ShowGridLineProperty, value); }
        }

        /// <summary>
        /// 获取设置AppBar关闭时的显示方式，默认Minimal，Mini版有效
        /// </summary>
        public AppBarClosedDisplayMode ClosedDisplayMode
        {
            get { return (AppBarClosedDisplayMode)GetValue(ClosedDisplayModeProperty); }
            set { SetValue(ClosedDisplayModeProperty, value); }
        }

        /// <summary>
        /// 获取报表要输出的Sheet
        /// </summary>
        public Worksheet Sheet
        {
            get { return _sheet; }
            internal set { _sheet = value; }
        }

        /// <summary>
        /// 获取报表数据集
        /// </summary>
        public Dictionary<string, RptData> DataSet
        {
            get { return _dataSet; }
            internal set { _dataSet = value; }
        }

        /// <summary>
        /// 根据报表描述信息链接到报表
        /// </summary>
        public Action<RptInfo> LinkReport { get; internal set; }

        #region 内部属性
        /// <summary>
        /// 获取设置报表模板根节点
        /// </summary>
        internal RptRoot Root { get; set; }

        /// <summary>
        /// 获取设置报表实例
        /// </summary>
        internal RptRootInst Inst { get; set; }
        #endregion

        /// <summary>
        /// 加载报表数据集，键为数据源名称，值为结果集Table
        /// </summary>
        /// <param name="p_dt"></param>
        public void LoadDataSet(Dict p_dt)
        {
            _dataSet = new Dictionary<string, RptData>();
            if (p_dt != null)
            {
                foreach (var item in p_dt)
                {
                    Table tbl = item.Value as Table;
                    if (tbl != null)
                        _dataSet[item.Key.ToLower()] = new RptData(tbl);
                }
            }
        }

        /// <summary>
        /// 初始化工具栏菜单
        /// </summary>
        /// <param name="p_menu"></param>
        public virtual void InitMenu(Menu p_menu)
        {
        }

        /// <summary>
        /// 点击单元格脚本
        /// </summary>
        /// <param name="p_id">脚本标识</param>
        /// <param name="p_text">单元格</param>
        public virtual void OnCellClick(string p_id, IRptCell p_text)
        {
        }
    }
}
