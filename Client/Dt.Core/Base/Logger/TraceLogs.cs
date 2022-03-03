#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Events;
using System.Text.Json;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 日志的Trace输出内容
    /// </summary>
    internal static class TraceLogs
    {
        const int _maxTrace = 30;
        static readonly List<byte[]> _rpcJsons = new List<byte[]>();
        static int _startIndex = 0;

        /// <summary>
        /// Trace日志列表
        /// </summary>
        public static readonly Nl<TraceLogItem> Data = new Nl<TraceLogItem>();

        /// <summary>
        /// 向Trace窗口输出信息
        /// </summary>
        /// <param name="p_logEvent"></param>
        public static void AddItem(LogEvent p_logEvent)
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
        public static void Clear()
        {
            Data.Clear();
            _rpcJsons.Clear();
            _startIndex = 0;
        }

        /// <summary>
        /// 缓存rpc调用或结果的json数据，返回索引，优点：
        /// 1. 避免无用的byte[] -> string 转换
        /// 2. 避免将 json 内容写进日志文件
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        public static string AddRpcJson(byte[] p_data)
        {
            _rpcJsons.Add(p_data);
            if (_rpcJsons.Count > _maxTrace)
            {
                _rpcJsons.RemoveAt(0);
                _startIndex++;
            }
            return (_rpcJsons.Count + _startIndex - 1).ToString();
        }

        /// <summary>
        /// 根据索引获取json内容
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public static string GetRpcJson(string p_id)
        {
            if (!int.TryParse(p_id, out int id))
                return null;

            int index = id - _startIndex;
            if (index >= 0 && index < _rpcJsons.Count)
            {
                try
                {
                    // 对Json格式化，带缩进
                    return JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(_rpcJsons[index]), JsonOptions.IndentedSerializer);
                }
                catch { }
            }
            return null;
        }
    }
}