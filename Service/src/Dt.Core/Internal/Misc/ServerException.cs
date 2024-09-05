#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 服务器异常
    /// </summary>
    internal class ServerException : Exception
    {
        public ServerException(string p_title, string p_message)
            : base(p_message)
        {
            Title = "⚡" + p_title;
        }

        public string Title { get; }
    }
}
