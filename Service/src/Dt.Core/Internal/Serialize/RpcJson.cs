#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 属性自定义json序列化/反序列化，属性类型需实现IRpcJson接口
    /// </summary>
    public class RpcJsonConverter<T> : JsonConverter<T>
        where T : class, IRpcJson
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            IRpcJson rpc = Activator.CreateInstance(typeToConvert) as IRpcJson;
            rpc.ReadRpcJson(ref reader);
            return (T)rpc;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            value.WriteRpcJson(writer);
        }
    }

    /// <summary>
    /// 使用内部IRpcJson接口自定义json序列化/反序列化，如在属性类型为Table,Row,Dict时添加此标签，即可按内部方式，秒！
    /// </summary>
    public class RpcJsonAttribute : JsonConverterAttribute
    {
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            if (typeToConvert.GetInterface("IRpcJson") != null)
            {
                Type type = typeof(RpcJsonConverter<>).MakeGenericType(typeToConvert);
                return (JsonConverter)Activator.CreateInstance(type);
            }
            throw new Exception($"类型{typeToConvert.FullName}未实现IRpcJson，无法序列化");
        }
    }
}
