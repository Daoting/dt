#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 特殊处理CNum与Cell之间null的转换
    /// </summary>
    public class NumValConverter : IFvCall
    {
        public object Get(Mid m)
        {
            if (m.Val == null)
                return double.NaN;
            return m.Val;
        }

        public object Set(Mid m)
        {
            if (double.IsNaN((double)m.Val))
                return null;
            return m.Val;
        }
    }
}
