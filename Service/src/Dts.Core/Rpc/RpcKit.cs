#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// Rpc工具方法
    /// </summary>
    public static class RpcKit
    {
        

        public static byte[] GetObjData(object p_obj)
        {
            if (p_obj == null)
                throw new InvalidDataException(nameof(p_obj));

            StringBuilder sb = new StringBuilder();
            using (StringWriter sr = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sr))
            {
                writer.WriteStartArray();
                JsonRpcSerializer.Serialize(p_obj, writer);
                writer.WriteEndArray();
                writer.Flush();
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
