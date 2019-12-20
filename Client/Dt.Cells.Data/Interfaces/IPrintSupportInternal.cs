#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Interface that supports opertation for save xml for printing.
    /// </summary>
    public interface IPrintSupportInternal
    {
        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="stream">The stream to which the object is serialized.</param>
        void WriteXml(Stream stream);
    }
}

