#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 搜索面板
    /// 固定搜索项：由外部设置，事件参数格式为 "#按钮名称"，#前缀用于区别普通搜索
    /// 历史搜索项：自动记录历史搜索，可删，事件参数内容为普通文本
    /// 搜索事件：所有固定搜索项、搜索框、历史搜索项统一触发Search事件
    /// </summary>
    public partial class SearchNav : Nav
    {

        public SearchNav()
        {
            InitializeComponent();
            Title = "搜索";
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<string> Search;

        /// <summary>
        /// 获取设置查询框提示内容
        /// </summary>
        public string Placeholder
        {
            get { return _tb.PlaceholderText; }
            set { _tb.PlaceholderText = value; }
        }
    }
}