#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// Api调用模式
    /// </summary>
    public enum ApiCallMode
    {
        /// <summary>
        /// 普通请求/响应模式
        /// </summary>
        Unary,

        /// <summary>
        /// 客户端发送一个请求，服务端返回数据流响应
        /// </summary>
        ServerStream,

        /// <summary>
        /// 客户端发送请求数据流，服务端返回一个响应
        /// </summary>
        ClientStream,

        /// <summary>
        /// 客户端发送请求数据流，服务端返回数据流响应
        /// </summary>
        DuplexStream
    }
}

