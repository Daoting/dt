#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Events;
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 日志的Trace输出内容
    /// </summary>
    public static class TraceLogs
    {
        const int _maxTrace = 50;
        static readonly List<object> _details = new List<object>();
        static int _startIndex = 0;

        /// <summary>
        /// Trace日志列表
        /// </summary>
        internal static readonly Nl<TraceLogItem> Data = new Nl<TraceLogItem>();

        /// <summary>
        /// 向Trace窗口输出信息
        /// </summary>
        /// <param name="p_logEvent"></param>
        internal static void AddItem(LogEvent p_logEvent)
        {
            var item = new TraceLogItem { Log = p_logEvent };
            Kit.RunAsync(() =>
            {
                using (Data.Defer())
                {
                    Data.Add(item);
                    if (Data.Count > _maxTrace)
                    {
                        // 确保输出行数不超过给定的最大行数
                        Data.RemoveAt(0);
                    }
                }
            });
        }

        /// <summary>
        /// 清空输出
        /// </summary>
        internal static void Clear()
        {
            Data.Clear();
            _details.Clear();
            _startIndex = 0;
        }

        /// <summary>
        /// 除某项外清空
        /// </summary>
        /// <param name="p_item"></param>
        internal static void ClearExcept(TraceLogItem p_item)
        {
            Kit.RunAsync(() =>
            {
                using (Data.Defer())
                {
                    Data.Clear();
                    Data.Add(p_item);
                }

                // 先生成 Detail 内容，避免清空后无法生成，否则算法复杂，巧！！！
                var str = p_item.Detial;
                _details.Clear();
                _startIndex = 0;
            });
        }

        /// <summary>
        /// 缓存日志项中详细数据，返回索引
        /// <para>如：rpc调用或返回的json数据，sqlite查询参数等。优点：</para>
        /// <para>1. 避免无用的byte[] -> string 转换</para>
        /// <para>2. 避免将 json 内容写进日志文件</para>
        /// </summary>
        /// <param name="p_detail"></param>
        /// <returns></returns>
        public static string AddDetail(object p_detail)
        {
            _details.Add(p_detail);
            if (_details.Count > _maxTrace)
            {
                _details.RemoveAt(0);
                _startIndex++;
            }
            return (_details.Count + _startIndex - 1).ToString();
        }

        /// <summary>
        /// 根据索引获取详细数据的内容
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        internal static string GetDetail(string p_id)
        {
            if (!int.TryParse(p_id, out int id))
                return null;

            int index = id - _startIndex;
            if (index >= 0
                && index < _details.Count
                && _details[index] != null)
            {
                var obj = _details[index];

                // 查询一次已构造detail内容不再需要，置null释放
                _details[index] = null;

                if (obj is byte[] json)
                {
                    try
                    {
                        // 对Json格式化，带缩进
                        return JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(json), JsonOptions.IndentedSerializer);
                    }
                    catch { }
                }

                if (obj is object[] ls)
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var writer = new Utf8JsonWriter(stream, JsonOptions.IndentedWriter))
                        {
                            writer.WriteStartArray();
                            if (ls != null && ls.Length > 0)
                            {
                                foreach (var par in ls)
                                {
                                    JsonRpcSerializer.Serialize(par, writer);
                                }
                            }
                            writer.WriteEndArray();
                        }
                        return Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
            }
            return null;
        }
    }
}