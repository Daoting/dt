#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 可序列化的键值集合，键忽略大小写
    /// 可以描述存储过程、Sql语句参数列表
    /// 值为嵌套的Dict时可描述复杂的数据结构
    /// 各值可为不同类型
    /// </summary>
    public class Dict : Dictionary<string, object>, IRpcJson
    {
        /// <summary>
        /// 构造方法，键比较时忽略大小写
        /// </summary>
        public Dict()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// 构造方法，指定容量
        /// </summary>
        /// <param name="capacity"></param>
        public Dict(int capacity)
            : base(capacity, StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// 返回字符串值
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>字符串值</returns>
        public string Str(string p_key)
        {
            return GetVal<string>(p_key);
        }

        /// <summary>
        /// 返回Table对象
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>Table</returns>
        public Table Tbl(string p_key)
        {
            return GetVal<Table>(p_key);
        }

        /// <summary>
        /// 返回Dict对象
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>Dict</returns>
        public Dict Dt(string p_key)
        {
            return GetVal<Dict>(p_key);
        }

        /// <summary>
        /// 返回字符串列表
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>字符串列表</returns>
        public List<string> StrList(string p_key)
        {
            return GetVal<List<string>>(p_key);
        }

        /// <summary>
        /// 返回字符串数组
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>字符串数组</returns>
        public string[] StrArray(string p_key)
        {
            return GetVal<string[]>(p_key);
        }

        /// <summary>
        /// 返回bool值
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>bool值</returns>
        public bool Bool(string p_key)
        {
            return GetVal<bool>(p_key);
        }

        /// <summary>
        /// 返回bool值列表
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>bool值列表</returns>
        public List<bool> BoolList(string p_key)
        {
            return GetVal<List<bool>>(p_key);
        }

        /// <summary>
        /// 返回bool值数组
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>bool值数组</returns>
        public bool[] BoolArray(string p_key)
        {
            return GetVal<bool[]>(p_key);
        }

        /// <summary>
        /// 返回double值
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>double值</returns>
        public double Double(string p_key)
        {
            return GetVal<double>(p_key);
        }

        /// <summary>
        /// 返回double值列表
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>double值列表</returns>
        public List<double> DoubleList(string p_key)
        {
            return GetVal<List<double>>(p_key);
        }

        /// <summary>
        /// 返回double值数组
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>double值数组</returns>
        public double[] DoubleArray(string p_key)
        {
            return GetVal<double[]>(p_key);
        }

        /// <summary>
        /// 返回整数值
        /// </summary>
        /// <param name="p_key">列名</param>
        /// <returns>整数值</returns>
        public int Int(string p_key)
        {
            return GetVal<int>(p_key);
        }

        /// <summary>
        /// 返回整数值列表
        /// </summary>
        /// <param name="p_key">列名</param>
        /// <returns>整数值列表</returns>
        public List<int> IntList(string p_key)
        {
            return GetVal<List<int>>(p_key);
        }

        /// <summary>
        /// 返回整数值数组
        /// </summary>
        /// <param name="p_key">列名</param>
        /// <returns>整数值数组</returns>
        public int[] IntArray(string p_key)
        {
            return GetVal<int[]>(p_key);
        }

        /// <summary>
        /// 返回long值
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>long值</returns>
        public long Long(string p_key)
        {
            return GetVal<long>(p_key);
        }

        /// <summary>
        /// 返回DateTime值
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>DateTime值</returns>
        public DateTime Date(string p_key)
        {
            return GetVal<DateTime>(p_key);
        }

        /// <summary>
        /// 返回char值
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>char值</returns>
        public char Char(string p_key)
        {
            return GetVal<char>(p_key);
        }

        /// <summary>
        /// 返回byte[]值
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>byte[]值</returns>
        public byte[] ByteArray(string p_key)
        {
            return GetVal<byte[]>(p_key);
        }

        /// <summary>
        /// 返回指定键的值
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns>值</returns>
        public T GetVal<T>(string p_key)
        {
            object val;
            if (!TryGetValue(p_key, out val))
                throw new Exception(string.Format("Dict中不包含 {0} 键！", p_key));

            Type type = typeof(T);
            if (val == null)
            {
                // 字符串返回Empty！！！
                if (type == typeof(string))
                    return (T)(object)string.Empty;
                return default(T);
            }

            // 若指定类型和当前类型匹配
            if (type == val.GetType())
                return (T)val;

            // bool特殊处理1
            if (type == typeof(bool))
            {
                string str = val.ToString().ToLower();
                bool suc = (str == "1" || str == "true");
                return (T)(object)suc;
            }

            // 若类型不匹配执行转换
            object result;
            try
            {
                result = Convert.ChangeType(val, type);
            }
            catch
            {
                throw new Exception($"Dict项[{p_key}]转换异常：无法将[{val}]转换到[{type}]类型！");
            }
            return (T)result;
        }

        #region IRpcJson
        void IRpcJson.ReadRpcJson(JsonReader p_reader)
        {
            // #dict下的 {
            p_reader.Read();
            while (p_reader.Read() && p_reader.TokenType == JsonToken.PropertyName)
            {
                string name = p_reader.Value.ToString();
                p_reader.Read();
                try
                {
                    this[name] = JsonRpcSerializer.Deserialize(p_reader);
                }
                catch (Exception exception)
                {
                    throw new Exception("反序列化Dict时异常: " + exception.Message);
                }
            }
            // 最外层 ]
            p_reader.Read();
        }

        void IRpcJson.WriteRpcJson(JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteValue("#dict");

            p_writer.WriteStartObject();
            foreach (var item in this)
            {
                p_writer.WritePropertyName(item.Key);
                JsonRpcSerializer.Serialize(item.Value, p_writer);
            }
            p_writer.WriteEndObject();

            p_writer.WriteEndArray();
        }
        #endregion
    }
}
