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
using System.IO;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 读取服务器的返回流
    /// </summary>
    public class ResponseReader
    {
        readonly Stream _responseStream;
        object _val;

        internal ResponseReader(Stream p_stream)
        {
            _responseStream = p_stream;
        }

        /// <summary>
        /// 读取从服务器返回的下一帧数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MoveNext()
        {
            if (_responseStream == null)
                return false;

            try
            {
                var data = await RpcKit.ReadFrame(_responseStream);
                if (data != null && data.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(data))
                    using (StreamReader sr = new StreamReader(ms))
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        reader.Read();
                        _val = JsonRpcSerializer.Deserialize(reader);
                    }
                    return true;
                }
            }
            catch
            { }
            return false;
        }

        /// <summary>
        /// 获取当前帧的指定类型值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        public T Val<T>()
        {
            return RpcKit.GetVal<T>(_val);
        }
    }
}
