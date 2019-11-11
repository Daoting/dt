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
using System.Collections.Generic;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Json Rpc序列化、反序列化
    /// </summary>
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
            if (p_value is IRpcJson xrs)
            {
                xrs.WriteRpcJson(p_writer);
                return;
            }

            Type tgtType = p_value.GetType();
            if (tgtType == typeof(string) || tgtType.IsValueType)
            {
                // string或值类型
                p_writer.WriteValue(p_value);
            }
            else if (p_value is IEnumerable)
            {
                if (tgtType == typeof(byte[]))
                {
                    // 字节数组需要base64编码
                    byte[] inArray = (byte[])p_value;
                    string str = Convert.ToBase64String(inArray);
                    p_writer.WriteStartArray();
                    p_writer.WriteValue("bytes");
                    p_writer.WriteValue(str);
                    p_writer.WriteEndArray();
                }
                else
                {
                    SerializeArray((IEnumerable)p_value, p_writer);
                }
            }
            else if (tgtType.Namespace == null && tgtType.IsNotPublic)
            {
                // 匿名对象转成Dict
                Dict dt = new Dict();
                foreach (var prop in tgtType.GetProperties())
                {
                    dt[prop.Name] = prop.GetValue(p_value);
                }
                ((IRpcJson)dt).WriteRpcJson(p_writer);
            }
            else
            {
                SerializeObject(p_value, p_writer);
            }
        }

        static void SerializeObject(object p_value, JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteValue("#" + SerializeTypeAlias.GetAlias(p_value.GetType()));
            JsonSerializer.Create().Serialize(p_writer, p_value);
            p_writer.WriteEndArray();
        }

        static void SerializeArray(IEnumerable p_value, JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteValue("&" + SerializeTypeAlias.GetAlias(p_value.GetType()));
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
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="p_reader"></param>
        /// <returns></returns>
        public static T Deserialize<T>(JsonReader p_reader)
        {
            object result;
            if (p_reader.TokenType == JsonToken.StartArray)
            {
                string tp;
                if (!p_reader.Read()
                    || p_reader.TokenType != JsonToken.String
                    || string.IsNullOrEmpty(tp = (string)p_reader.Value))
                    throw new Exception("Json自定义数组中未包含类型名！");

                // 前缀'#'表示对象
                if (tp.StartsWith("#"))
                {
                    Type type = SerializeTypeAlias.GetType(tp.Substring(1));
                    // 目标类型 T 可以为派生类
                    if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
                        throw new Exception($"{typeof(T).Name} 类型未继承 {type.Name}！");

                    // 自定义序列化
                    if (type.GetInterface("IRpcJson") != null)
                    {
                        T tgt = Activator.CreateInstance<T>();
                        ((IRpcJson)tgt).ReadRpcJson(p_reader);
                        return tgt;
                    }

                    // 标准序列化
                    p_reader.Read();
                    return JsonSerializer.Create().Deserialize<T>(p_reader);
                }

                // 前缀'&'表示集合
                if (tp.StartsWith("&"))
                {
                    result = DeserializeArray(p_reader, tp.Substring(1));
                }
                else if (tp == "bytes")
                {
                    // base64编码的字节数组
                    p_reader.Read();
                    byte[] data = Convert.FromBase64String(p_reader.Value.ToString());
                    p_reader.Read();
                    result = data;
                }
                else
                {
                    throw new Exception($"无法自动反序列化Json类型{tp}！");
                }
            }
            else
            {
                result = p_reader.Value;
            }

            T val = default(T);
            if (result == null)
            {
                // 空值
            }
            else if (typeof(T) == result.GetType())
            {
                // 结果对象与给定对象类型相同时
                val = (T)result;
            }
            else
            {
                // 特殊处理结果对象与给定对象类型不相同时
                val = (T)Convert.ChangeType(result, typeof(T));
            }
            return val;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="p_reader"></param>
        /// <returns></returns>
        public static object Deserialize(JsonReader p_reader)
        {
            if (p_reader.TokenType == JsonToken.StartArray)
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

            if (p_reader.TokenType == JsonToken.Integer && p_reader.ValueType == typeof(long))
            {
                // 需要int时传long会异常，反之正常
                long num = (long)p_reader.Value;
                if (num < int.MaxValue)
                    return (int)num;
            }
            return p_reader.Value;
        }

        static object DeserializeObject(JsonReader p_reader, string p_alias)
        {
            Type type = SerializeTypeAlias.GetType(p_alias);
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
            if (p_alias == "ls")
            {
                // json中int和long都按照long处理，在前面已特殊处理
                List<long> ls = new List<long>();
                while (p_reader.Read() && p_reader.TokenType != JsonToken.EndArray)
                {
                    var val = Deserialize(p_reader);
                    if (val is int c)
                        ls.Add(c);
                    else
                        ls.Add((long)val);
                }
                return ls;
            }

            // 不支持数组，支持List<T>
            // .Net Native不支持数组类型反序列化！
            // target = Activator.CreateInstance(Type.GetType("System.Collections.Generic.List`1[[" + elementType.FullName + ", " + elementType.Assembly.FullName + "]]")) as IList;
            Type type = SerializeTypeAlias.GetType(p_alias);
            IList target = Activator.CreateInstance(type, new object[0]) as IList;
            if (target == null)
                throw new Exception(type.FullName + " 类无法进行自动反序列化。");

            while (p_reader.Read() && p_reader.TokenType != JsonToken.EndArray)
            {
                target.Add(Deserialize(p_reader));
            }
            return target;
        }
        #endregion
    }
}
