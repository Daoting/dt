#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// A class implements <see cref="T:Dt.Xls.IFunction" /> used to represents a custom function
    /// </summary>
    public class ExcelCustomFunction : IFunction
    {
        internal Tuple<int, byte[]> ExternNameBits;

        /// <summary>
        /// The custom function name
        /// </summary>
        public string Name { get; internal set; }
    }
}

