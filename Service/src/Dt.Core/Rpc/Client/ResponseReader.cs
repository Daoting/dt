#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System.IO;
using System.Text.Json;
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
            try
            {
                _val = RpcKit.ParseBytes<object>(await RpcClientKit.ReadFrame(_responseStream));
                return true;
            }
            catch { }

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

        /// <summary>
        /// 关闭流
        /// </summary>
        public void Close()
        {
            _responseStream.Close();
        }
    }
}
