#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.Json;
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
        readonly ApiInvoker _invoker;
        object _val;

        internal RequestReader(ApiInvoker p_invoker)
        {
            _invoker = p_invoker;
        }

        /// <summary>
        /// 读取客户端请求流的下一帧数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MoveNext()
        {
            try
            {
                _val = RpcKit.ParseBytes<object>(await RpcServerKit.ReadFrame(_invoker.Context.Request.BodyReader));
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
    }
}
