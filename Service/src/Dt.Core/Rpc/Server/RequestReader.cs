#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 读取客户端请求流
    /// </summary>
    public class RequestReader
    {
        LobContext _lc;
        object _val;

        internal RequestReader(LobContext p_lc)
        {
            _lc = p_lc;
        }

        /// <summary>
        /// 读取客户端请求流的下一帧数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MoveNext()
        {
            try
            {
                byte[] data = await RpcServerKit.ReadFrame(_lc.Context.Request.BodyReader);
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
    }
}
