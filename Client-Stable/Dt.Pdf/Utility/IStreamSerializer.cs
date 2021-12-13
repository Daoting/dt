#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
#endregion

namespace Dt.Pdf.Utility
{
    /// <summary>
    /// Stream serializer
    /// </summary>
    internal interface IStreamSerializer
    {
        /// <summary>
        /// Deserializes the specified stream.
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns>deserialized object</returns>
        object Deserialize(Stream stream);
        /// <summary>
        /// Serializes the specified stream.
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="obj">obj</param>
        void Serialize(Stream stream, object obj);

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        System.Type Type { get; }
    }
}

