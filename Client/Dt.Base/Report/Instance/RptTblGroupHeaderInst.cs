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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml;
using Windows.UI;
using Windows.UI.Xaml.Media;

#endregion

namespace Dt.Base.Report
{
    internal class RptTblGroupHeaderInst : RptTblPartInst
    {
        public RptTblGroupHeaderInst(RptItemBase p_item)
            : base(p_item)
        {
        }

        protected override void DoOutput()
        {
            OutputChildren();
        }
    }
}
