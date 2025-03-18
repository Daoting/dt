#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 查询格
    /// </summary>
    public partial class FvCell
    {
        #region 静态内容
        public static readonly DependencyProperty QueryProperty = DependencyProperty.Register(
            "Query",
            typeof(QueryType),
            typeof(FvCell),
            new PropertyMetadata(QueryType.Disable, OnQueryTypeChanged));

        public static readonly DependencyProperty QueryFlagProperty = DependencyProperty.Register(
            "QueryFlag",
            typeof(CompFlag),
            typeof(FvCell),
            new PropertyMetadata(CompFlag.Ignore, OnQueryFlagChanged));

        static void OnQueryTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            if (cell._panel != null)
                cell._panel.UpdateChildren();
        }

        static void OnQueryFlagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            if (cell._panel != null)
                cell._panel.OnQueryFlagChanged();
        }
        #endregion

        /// <summary>
        /// 获取设置查询时对条件比较符控制：无比较符、可修改比较符、比较符只读
        /// </summary>
        [CellParam("控制查询条件比较符")]
        public QueryType Query
        {
            get { return (QueryType)GetValue(QueryProperty); }
            set { SetValue(QueryProperty, value); }
        }

        /// <summary>
        /// 获取设置查询时的条件比较符
        /// </summary>
        [CellParam("查询条件比较符")]
        public CompFlag QueryFlag
        {
            get { return (CompFlag)GetValue(QueryFlagProperty); }
            set { SetValue(QueryFlagProperty, value); }
        }
    }
}