#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.Utility
{
    /// <summary>
    /// Serializer builder
    /// </summary>
    internal class SerializerBuilder
    {
        private static Dictionary<string, IStreamSerializer> serializerCache = new Dictionary<string, IStreamSerializer>();

        /// <summary>
        /// Gets the serializer by type.
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>serializer</returns>
        public static IStreamSerializer GetSerializerByType(Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (serializerCache.ContainsKey(type.FullName))
            {
                return serializerCache[type.FullName];
            }
            IStreamSerializer serializer = new StreamSerializer(type);
            if (serializer != null)
            {
                serializerCache.Add(type.FullName, serializer);
            }
            return serializer;
        }
    }
}

