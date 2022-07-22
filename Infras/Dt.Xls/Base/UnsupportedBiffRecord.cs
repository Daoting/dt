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
    /// Represents the unsupported biff records
    /// </summary>
    public class UnsupportedBiffRecord
    {
        /// <summary>
        /// The record type
        /// </summary>
        public int RecordType { get; internal set; }

        /// <summary>
        /// The record value in bytes.
        /// </summary>
        public byte[] RecordValue { get; set; }
    }
}

