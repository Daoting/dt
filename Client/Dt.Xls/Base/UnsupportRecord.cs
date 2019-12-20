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
    /// 
    /// </summary>
    public class UnsupportRecord : IUnsupportRecord
    {
        /// <summary>
        /// Unsupport item category
        /// </summary>
        public RecordCategory Category { get; internal set; }

        /// <summary>
        /// Unsupport item file type
        /// </summary>
        public ExcelFileType FileType { get; internal set; }

        /// <summary>
        /// Unsupport item content
        /// </summary>
        public object Value { get; internal set; }
    }
}

