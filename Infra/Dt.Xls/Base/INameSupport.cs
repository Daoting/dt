#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Defines a generalized property that a value or class implements to create type-specific property for get its name.
    /// </summary>
    public interface INameSupport
    {
        /// <summary>
        /// Get the name of the instance or type.
        /// </summary>
        string Name { get; }
    }
}

