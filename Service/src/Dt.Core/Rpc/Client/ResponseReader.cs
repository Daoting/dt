#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 读取服务器的返回流
    /// </summary>
    public class ResponseReader
    {
        HttpResponseMessage _response;
        Stream _responseStream;
        object _val;

        internal ResponseReader(HttpResponseMessage p_response)
        {
            _response = p_response;
        }

        internal async Task InitStream()
        {
            // wasm中增加上述设置后返回 stream 正常！
            _responseStream = await _response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 读取从服务器返回的下一帧数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MoveNext()
        {
            try
            {
                // _responseStream.ReadAsync 使用 CancellationToken 也只有第一次取消时有效，所以未使用！
                // 此处只在服务端取消连接时抛出异常！
                var data = await RpcClientKit.ReadFrame(_responseStream);
                _val = RpcKit.ParseBytes<object>(data);
                return true;
            }
            catch
            {
                Dispose();
            }
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

        // 客户端主动关闭时，android ios上异常 且 客户端在线状态未实时更新，只能统一手动调用服务关闭连接！！！
        ///// <summary>
        ///// 关闭流
        ///// </summary>
        //public void Close()
        //{
        //    IsClosedBySelf = true;
        //    Dispose();
        //}

        /// <summary>
        /// 是否已关闭
        /// </summary>
        public bool IsClosed => _response == null;

        void Dispose()
        {
            if (_responseStream != null)
            {
                _responseStream.Close();
                _responseStream = null;
            }

            if (_response != null)
            {
                _response.Dispose();
                _response = null;
            }
        }
    }
}
