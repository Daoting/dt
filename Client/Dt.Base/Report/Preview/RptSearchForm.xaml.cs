#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class RptSearchForm : UserControl
    {

        public RptSearchForm(RptInfo p_info, Tab p_tab)
        {
            InitializeComponent();
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<RptInfo> Query;

        /// <summary>
        /// 执行查询
        /// </summary>
        internal void DoQuery()
        {

        }
    }
}
