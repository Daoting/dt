#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// Api调用结果种类
    /// </summary>
    public enum ApiResponseType
    {
        /// <summary>
        /// 调用成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 调用过程中出错
        /// </summary>
        Error = 1,

        /// <summary>
        /// 调用过程中产生业务警告提示
        /// </summary>
        Warning = 2,
    }
}

