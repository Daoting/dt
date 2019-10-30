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
    /// 远程回调结果包装类
    /// </summary>
    internal class RpcResult
    {
        /// <summary>
        /// 结果类型
        /// </summary>
        public RpcResultType ResultType { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 耗时
        /// </summary>
        public string Elapsed { get; set; }
    }
}
