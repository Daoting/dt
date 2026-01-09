#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// AOT不支持类型反射方式序列化，替代 JsonSerializer 对普通对象序列化功能
    /// </summary>
    public class JsonObj
    {
        public void WriteJson(Utf8JsonWriter p_writer)
        {
            // 非内置对象
            //[
            //    "#object",
            //    {
            //        "key1":"val1",
            //        "key2":"val2"
            //    }
            //]
            p_writer.WriteStartArray();
            p_writer.WriteStringValue("#object");
            p_writer.WriteStartObject();

            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo prop in props)
            {
                if (!prop.CanWrite || prop.GetCustomAttribute<JsonIgnoreAttribute>(false) != null)
                    continue;

                p_writer.WritePropertyName(prop.Name);
                JsonRpcSerializer.Serialize(prop.GetValue(this), p_writer);
            }

            p_writer.WriteEndObject();
            p_writer.WriteEndArray();
        }

        public void ReadJson(ref Utf8JsonReader p_reader)
        {
            // 外层 {
            p_reader.Read();

            // 读取属性
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            while (p_reader.Read() && p_reader.TokenType == JsonTokenType.PropertyName)
            {
                var name = p_reader.GetString();
                // 属性值
                p_reader.Read();
                var prop = props.FirstOrDefault(p => p.Name == name);
                if (prop != null)
                {
                    var val = JsonRpcSerializer.Deserialize(ref p_reader, prop.PropertyType);
                    prop.SetValue(this, val);
                }
                else
                {
                    JsonRpcSerializer.Deserialize(ref p_reader);
                }
            }
            // 外层 }
        }
    }
}
