#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Buffers.Text;
using System.Buffers;
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
        public static void Serialize(object p_value, Utf8JsonWriter p_writer)
        {
            if (p_value == null)
            {
                p_writer.WriteNullValue();
                return;
            }

            // 自定义序列化
            if (p_value is IRpcJson xrs)
            {
                xrs.WriteRpcJson(p_writer);
                return;
            }

            Type tp = p_value.GetType();

            // 匿名对象转成Dict
            if (tp.Namespace == null && tp.IsNotPublic)
            {
                Dict dt = new Dict();
                foreach (var prop in tp.GetProperties())
                {
                    dt[prop.Name] = prop.GetValue(p_value);
                }
                ((IRpcJson)dt).WriteRpcJson(p_writer);
                return;
            }

            switch (Type.GetTypeCode(tp))
            {
                case TypeCode.String:
                    p_writer.WriteStringValue((string)p_value);
                    break;
                case TypeCode.Boolean:
                    p_writer.WriteBooleanValue((bool)p_value);
                    break;
                case TypeCode.DateTime:
                    p_writer.WriteStringValue((DateTime)p_value);
                    break;
                case TypeCode.Int64:
                    p_writer.WriteNumberValue((long)p_value);
                    break;
                case TypeCode.Int32:
                    p_writer.WriteNumberValue((int)p_value);
                    break;
                case TypeCode.Double:
                    p_writer.WriteNumberValue((double)p_value);
                    break;
                case TypeCode.Single:
                    p_writer.WriteNumberValue((float)p_value);
                    break;
                case TypeCode.Decimal:
                    p_writer.WriteNumberValue((decimal)p_value);
                    break;
                case TypeCode.UInt64:
                    p_writer.WriteNumberValue((ulong)p_value);
                    break;
                case TypeCode.UInt32:
                    p_writer.WriteNumberValue((uint)p_value);
                    break;
                case TypeCode.Int16:
                    p_writer.WriteNumberValue((short)p_value);
                    break;
                case TypeCode.UInt16:
                    p_writer.WriteNumberValue((ushort)p_value);
                    break;
                case TypeCode.Byte:
                    p_writer.WriteNumberValue((byte)p_value);
                    break;
                case TypeCode.SByte:
                    p_writer.WriteNumberValue((sbyte)p_value);
                    break;

                case TypeCode.Object:
                    if (tp == typeof(byte[]))
                    {
                        // 字节数组需要base64编码
                        p_writer.WriteStringValue(Convert.ToBase64String((byte[])p_value));
                    }
                    else if (p_value is IEnumerable)
                    {
                        // 列表
                        SerializeArray((IEnumerable)p_value, p_writer);
                    }
                    else
                    {
                        SerializeObject(p_value, p_writer);
                    }
                    break;

                default:
                    throw new Exception("未支持序列化类型" + tp.FullName);
            }
        }

        static void SerializeObject(object p_value, Utf8JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteStringValue("#" + SerializeTypeAlias.GetAlias(p_value.GetType()));
            JsonSerializer.Serialize(p_writer, p_value);
            p_writer.WriteEndArray();
        }

        static void SerializeArray(IEnumerable p_value, Utf8JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteStringValue("&" + SerializeTypeAlias.GetAlias(p_value.GetType()));
            if (p_value is List<object> lo)
            {
                // 记录item类型
                foreach (object item in lo)
                {
                    p_writer.WriteStartArray();

                    if (item != null)
                    {
                        var tp = item.GetType();
                        if (tp.FullName == "System." + tp.Name)
                        {
                            // 简单类型
                            p_writer.WriteStringValue(tp.Name);
                        }
                        else if (tp.IsGenericType && tp.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            // 可空值类型
                            p_writer.WriteStringValue(tp.GetGenericArguments()[0].Name + "?");
                        }
                        else
                        {
                            // 复杂类型空即可
                            p_writer.WriteStringValue("");
                        }
                        Serialize(item, p_writer);
                    }
                    else
                    {
                        // null按string
                        p_writer.WriteStringValue("String");
                        p_writer.WriteNullValue();
                    }

                    p_writer.WriteEndArray();
                }
            }
            else
            {
                foreach (object item in p_value)
                {
                    Serialize(item, p_writer);
                }
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
        public static T Deserialize<T>(ref Utf8JsonReader p_reader)
        {
            return (T)Deserialize(ref p_reader, typeof(T));
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="p_reader"></param>
        /// <param name="p_tgtType">目标类型，默认null，无目标类型，列表、字节数组不支持类型转换</param>
        /// <returns></returns>
        public static object Deserialize(ref Utf8JsonReader p_reader, Type p_tgtType = null)
        {
            if (p_tgtType == typeof(object))
                p_tgtType = null;

            // 参见源码：https://github.com/dotnet/runtime/blob/master/src/libraries/System.Text.Json/src/System/Text/Json/Reader/Utf8JsonReader.TryGet.cs
            switch (p_reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    {
                        string tp;
                        if (!p_reader.Read()
                            || p_reader.TokenType != JsonTokenType.String
                            || string.IsNullOrEmpty(tp = p_reader.GetString()))
                            throw new Exception("Json自定义数组中未包含类型名！");

                        // 前缀'#'表示对象
                        if (tp.StartsWith("#"))
                            return DeserializeObject(ref p_reader, tp.Substring(1), p_tgtType);

                        // 前缀'&'表示集合
                        if (tp.StartsWith("&"))
                            return DeserializeArray(ref p_reader, tp.Substring(1));
                        throw new Exception($"无法自动反序列化Json类型{tp}！");
                    }

                case JsonTokenType.String:
                    {
                        if (p_tgtType == null || p_tgtType == typeof(string))
                            return p_reader.GetString();
                        if (p_tgtType == typeof(DateTime) || p_tgtType == typeof(DateTime?))
                            return p_reader.GetDateTime();
                        // base64编码的字节数组
                        if (p_tgtType == typeof(byte[]))
                            return Convert.FromBase64String(p_reader.GetString());

                        return Convert.ChangeType(p_reader.GetString(), p_tgtType);
                    }

                case JsonTokenType.Number:
                    {
                        if (p_tgtType != null)
                        {
                            // 可空类型
                            if (p_tgtType.IsGenericType && p_tgtType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                p_tgtType = p_tgtType.GetGenericArguments()[0];

                            switch (Type.GetTypeCode(p_tgtType))
                            {
                                case TypeCode.Int64:
                                    return p_reader.GetInt64();
                                case TypeCode.Int32:
                                    return p_reader.GetInt32();
                                case TypeCode.Double:
                                    return p_reader.GetDouble();
                                case TypeCode.Single:
                                    return p_reader.GetSingle();
                                case TypeCode.Decimal:
                                    return p_reader.GetDecimal();
                                case TypeCode.UInt64:
                                    return p_reader.GetUInt64();
                                case TypeCode.UInt32:
                                    return p_reader.GetUInt32();
                                case TypeCode.Int16:
                                    return p_reader.GetInt16();
                                case TypeCode.UInt16:
                                    return p_reader.GetUInt16();
                                case TypeCode.Byte:
                                    return p_reader.GetByte();
                                case TypeCode.SByte:
                                    return p_reader.GetSByte();

                                case TypeCode.String:
                                    {
                                        if (p_reader.TryGetInt32(out int si))
                                            return si.ToString();
                                        if (p_reader.TryGetInt64(out long sl))
                                            return sl.ToString();
                                        if (p_reader.TryGetDouble(out double sd))
                                            return sd.ToString();
                                        throw new Exception("数值转换字符串失败");
                                    }

                                default:
                                    throw new Exception("数值无法转换成类型" + p_tgtType.FullName);
                            }
                        }

                        // 未指定数值类型时，按优先级 int -> long -> double 转换
                        if (p_reader.TryGetInt32(out int i))
                            return i;
                        if (p_reader.TryGetInt64(out long l))
                            return l;
                        if (p_reader.TryGetDouble(out double d))
                            return d;

                        throw new Exception("无法读取json数值");
                    }

                // bool需要转换放在外部！
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;

                case JsonTokenType.Null:
                    {
                        if (p_tgtType == null)
                            return null;
                        return p_tgtType.IsValueType ? Activator.CreateInstance(p_tgtType) : null;
                    }

                default:
                    throw new Exception("反序列化json失败，TokenType：" + p_reader.TokenType.ToString());
            }
        }

        static object DeserializeObject(ref Utf8JsonReader p_reader, string p_alias, Type p_tgtType)
        {
            Type type = SerializeTypeAlias.GetType(p_alias);
            if (p_tgtType != null && p_tgtType != type)
            {
                // 以子类型为准
                if (p_tgtType.IsSubclassOf(type))
                    type = p_tgtType;
                else if (!type.IsSubclassOf(p_tgtType))
                    throw new Exception($"{p_tgtType.Name} 与 {type.Name} 无继承关系！");
            }

            // 自定义序列化
            if (type.GetInterface("IRpcJson") != null)
            {
                // 无参数构造方法可能为private，如实体类型
                object tgt = Activator.CreateInstance(type, true);
                ((IRpcJson)tgt).ReadRpcJson(ref p_reader);
                return tgt;
            }

            // 标准序列化
            p_reader.Read();
            object obj = JsonSerializer.Deserialize(ref p_reader, type);
            return obj;
        }

        static object DeserializeArray(ref Utf8JsonReader p_reader, string p_alias)
        {
            // 只支持List<T>的情况
            Type type = SerializeTypeAlias.GetType(p_alias);
            Type itemType = type.GetGenericArguments()[0];
            if (itemType == typeof(object))
                return DeserializeObjsArray(ref p_reader);

            IList target = Activator.CreateInstance(type) as IList;
            while (p_reader.Read() && p_reader.TokenType != JsonTokenType.EndArray)
            {
                target.Add(Deserialize(ref p_reader, itemType));
            }
            return target;
        }

        static object DeserializeObjsArray(ref Utf8JsonReader p_reader)
        {
            List<object> ls = new List<object>();
            // 项起始 [
            while (p_reader.Read())
            {
                // 外层末尾 ]
                if (p_reader.TokenType == JsonTokenType.EndArray)
                    break;

                // 项
                p_reader.Read();
                string tpName = p_reader.GetString();
                p_reader.Read();
                if (tpName == string.Empty)
                {
                    ls.Add(Deserialize(ref p_reader));
                }
                else
                {
                    Type tp;
                    if (tpName.EndsWith("?"))
                    {
                        tp = Type.GetType("System." + tpName.TrimEnd('?'), true, false);
                        tp = typeof(Nullable<>).MakeGenericType(tp);
                    }
                    else
                    {
                        tp = Type.GetType("System." + tpName, true, false);
                    }
                    ls.Add(Deserialize(ref p_reader, tp));
                }
                // 项末尾 ]
                p_reader.Read();
            }
            return ls;
        }
        #endregion

        #region 扩展方法
        public static string ReadAsString(this ref Utf8JsonReader p_reader)
        {
            if (p_reader.Read())
                return p_reader.GetString();
            throw new Exception(_readError);
        }

        public static long ReadAsLong(this ref Utf8JsonReader p_reader)
        {
            if (p_reader.Read() && p_reader.TryGetInt64(out long v))
                return v;
            throw new Exception(_readError);
        }

        public static int ReadAsInt(this ref Utf8JsonReader p_reader)
        {
            if (p_reader.Read() && p_reader.TryGetInt32(out int v))
                return v;
            throw new Exception(_readError);
        }

        public static double ReadAsDouble(this ref Utf8JsonReader p_reader)
        {
            if (p_reader.Read() && p_reader.TryGetDouble(out double v))
                return v;
            throw new Exception(_readError);
        }

        public static bool ReadAsBool(this ref Utf8JsonReader p_reader)
        {
            if (p_reader.Read())
                return p_reader.GetBoolean();
            throw new Exception(_readError);
        }

        public static DateTime ReadAsDateTime(this ref Utf8JsonReader p_reader)
        {
            if (p_reader.Read() && p_reader.TryGetDateTime(out DateTime v))
                return v;
            throw new Exception(_readError);
        }

        const string _readError = "JsonReader读取失败！";
        #endregion
    }
}
