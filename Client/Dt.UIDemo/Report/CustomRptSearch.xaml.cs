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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#endregion

namespace Dt.UIDemo
{
    public sealed partial class CustomRptSearch : RptSearchTab
    {
        public CustomRptSearch(RptInfo p_info) : base(p_info)
        {
            InitializeComponent();
            _row.AddCell("unuse", "选项二");
            _row.AddCell("idstr", "1");
            _fv.Data = _row;
        }
    }
}
