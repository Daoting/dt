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
    public sealed partial class ChartForm : UserControl
    {
        RptDesignWin _owner;

        public ChartForm(RptDesignWin p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
        }

        /// <summary>
        /// 切换当前报表项
        /// </summary>
        /// <param name="p_item"></param>
        internal void LoadItem(RptChart p_item)
        {
            
        }
    }
}
