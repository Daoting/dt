#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 反序列化结果的种类
    /// </summary>
    internal enum RpcResultType
    {
        /// <summary>
        /// 普通结果值
        /// </summary>
        Value,

        /// <summary>
        /// 服务端错误信息
        /// </summary>
        Error,

        /// <summary>
        /// 业务警告信息
        /// </summary>
        Message
    }
}
