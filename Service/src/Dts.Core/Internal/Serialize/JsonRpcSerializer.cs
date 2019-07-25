#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Reflection;
#endregion

namespace Dts.Core
{
    public static class JsonRpcSerializer
    {
        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="p_value"></param>
        /// <param name="p_writer"></param>
        public static void Serialize(object p_value, JsonWriter p_writer)
        {
            if (p_value == null)
            {
                p_writer.WriteNull();
                return;
            }

            // 自定义序列化
            IRpcJson xrs = p_value as IRpcJson;
            if (xrs != null)
            {
                xrs.WriteRpcJson(p_writer);
                return;
            }

            Type tgtType = p_value.GetType();
            if (tgtType == typeof(string)
                || tgtType == typeof(bool)
                || tgtType == typeof(int)
                || tgtType == typeof(short)
                || tgtType == typeof(long)
                || tgtType == typeof(float)
                || tgtType == typeof(double)
                || tgtType == typeof(decimal))
            {
                p_writer.WriteValue(p_value);
            }
            else if (tgtType == typeof(DateTime))
            {
                p_writer.WriteValue(((DateTime)p_value).ToString("yyyy-MM-ddTHH:mm:ss.ffffff"));
            }
            else if (tgtType == typeof(byte[]))
            {
                byte[] inArray = (byte[])p_value;
                string str = Convert.ToBase64String(inArray);
                p_writer.WriteStartArray();
                p_writer.WriteValue("bytes");
                p_writer.WriteValue(str);
                p_writer.WriteEndArray();
            }
            else if (p_value is IEnumerable)
            {
                SerializeArray((IEnumerable)p_value, p_writer);
            }
            else if (p_value is Enum)
            {
                p_writer.WriteValue((int)p_value);
            }
            else
            {
                SerializeObject(p_value, p_writer);
            }
        }

        static void SerializeObject(object p_value, JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteValue("#" + Silo.GetSerializableTypeAlias(p_value.GetType()));
            JsonSerializer.Create().Serialize(p_writer, p_value);
            p_writer.WriteEndArray();
        }

        static void SerializeArray(IEnumerable p_value, JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteValue("&" + Silo.GetSerializableTypeAlias(p_value.GetType()));
            foreach (object item in p_value)
            {
                Serialize(item, p_writer);
            }
            p_writer.WriteEndArray();
        }
        #endregion

        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="p_reader"></param>
        /// <returns></returns>
        public static object Deserialize(JsonReader p_reader)
        {
            switch (p_reader.TokenType)
            {
                case JsonToken.StartArray:
                    {
                        string tp;
                        if (!p_reader.Read()
                            || p_reader.TokenType != JsonToken.String
                            || string.IsNullOrEmpty(tp = (string)p_reader.Value))
                            throw new Exception("Json自定义数组中未包含类型名！");

                        // 前缀'#'表示对象
                        if (tp.StartsWith("#"))
                            return DeserializeObject(p_reader, tp.Substring(1));

                        // 前缀'&'表示集合
                        if (tp.StartsWith("&"))
                            return DeserializeArray(p_reader, tp.Substring(1));

                        // base64编码的字节数组
                        if (tp == "bytes")
                        {
                            p_reader.Read();
                            byte[] data = Convert.FromBase64String(p_reader.Value.ToString());
                            p_reader.Read();
                            return data;
                        }
                        throw new Exception($"无法自动反序列化Json类型{tp}！");
                    }
                case JsonToken.Integer:
                    if (p_reader.ValueType == typeof(long))
                    {
                        // 需要int时传long会异常，反之正常
                        long num = (long)p_reader.Value;
                        if (num < int.MaxValue)
                            return (int)num;
                    }
                    return p_reader.Value;
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Float:
                case JsonToken.Date:
                case JsonToken.Bytes:
                case JsonToken.StartConstructor:
                case JsonToken.Null:
                case JsonToken.Undefined:
                case JsonToken.Raw:
                    return p_reader.Value;
                default:
                    throw new Exception(string.Format("不支持Json类型“{0}”的反序列化！", p_reader.TokenType));
            }
        }

        static object DeserializeObject(JsonReader p_reader, string p_alias)
        {
            Type type = Silo.GetSerializableType(p_alias);
            object result = Activator.CreateInstance(type);
            if (result is IRpcJson xrs)
            {
                xrs.ReadRpcJson(p_reader);
                return result;
            }

            p_reader.Read();
            object obj = JsonSerializer.Create().Deserialize(p_reader, type);
            return obj;
        }

        static object DeserializeArray(JsonReader p_reader, string p_alias)
        {
            Type type = Silo.GetSerializableType(p_alias);
            IList target = null;
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                target = Activator.CreateInstance(Type.GetType("System.Collections.Generic.List`1[[" + elementType.FullName + ", " + elementType.Assembly.FullName + "]]")) as IList;
            }
            else
            {
                if (type.GetInterface("System.Collections.IList") == null)
                    throw new Exception(type.FullName + " 类无法进行自动反序列化。");
                target = Activator.CreateInstance(type, new object[0]) as IList;
            }

            while (p_reader.Read() && p_reader.TokenType != JsonToken.EndArray)
            {
                target.Add(Deserialize(p_reader));
            }

            if (type.IsArray)
                return target.GetType().InvokeMember("ToArray", BindingFlags.InvokeMethod, null, target, null);
            return target;
        }
        #endregion
    }
}
