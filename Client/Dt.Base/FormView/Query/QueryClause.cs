#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-01-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class QueryClause
    {
        /// <summary>
        /// where后面的条件子句
        /// </summary>
        public string Where { get; set; }

        /// <summary>
        /// sql参数值
        /// </summary>
        public Dict Params { get; set; }
    }
}