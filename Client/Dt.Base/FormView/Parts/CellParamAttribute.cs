#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 格参数标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CellParamAttribute : Attribute
    {
        public CellParamAttribute(string p_title)
        {
            Title = p_title;
        }

        /// <summary>
        /// 获取参数标题
        /// </summary>
        public string Title { get; }
    }
}
