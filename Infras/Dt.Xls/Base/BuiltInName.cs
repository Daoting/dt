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
    /// A class implements <see cref="T:Dt.Xls.IFunction" /> used to represents  An interface used to represents  built-in name used in Excel
    /// </summary>
    public class BuiltInName : IBuiltInName
    {
        internal Tuple<int, byte[]> ExtraBits;
        internal Tuple<int, byte[]> NameBits;

        /// <summary>
        /// Create a new instance of BuiltInName
        /// </summary>
        /// <param name="name">the name of the BuiltInName instance</param>
        public BuiltInName(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// The built-in name
        /// </summary>
        public string Name { get; private set; }
    }
}

