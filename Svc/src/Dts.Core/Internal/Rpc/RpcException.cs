#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 业务处理异常类型
    /// </summary>
    public class RpcException : Exception
    {
        public RpcException(string p_message)
            : base(p_message)
        {
        }
    }
}

