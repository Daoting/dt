#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 非调试状态下的友好异常提示
    /// </summary>
    public class FriendlyException : Exception
    {
        /// <summary>
        /// 构造 FriendlyException 类。
        /// </summary>
        /// <param name="message"></param>
        public FriendlyException(string message)
            : base(message)
        {
        }
    }
}
