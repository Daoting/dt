#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 属性自定义json序列化/反序列化，属性类型需实现IRpcJson接口
    /// </summary>
    public class RpcJson : JsonConverter
    {
        public override bool CanConvert(Type p_objectType)
        {
            return p_objectType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IRpcJson));
        }

        public override object ReadJson(JsonReader p_reader, Type p_objectType, object p_existingValue, JsonSerializer p_serializer)
        {
            p_reader.Read();
            IRpcJson rpc = Activator.CreateInstance(p_objectType) as IRpcJson;
            rpc.ReadRpcJson(p_reader);
            return rpc;
        }

        public override void WriteJson(JsonWriter p_writer, object p_value, JsonSerializer p_serializer)
        {
            if (p_value is IRpcJson rpc)
                rpc.WriteRpcJson(p_writer);
        }
    }
}
