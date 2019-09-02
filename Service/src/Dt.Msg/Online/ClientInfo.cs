#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using Dt.Core.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 即时通信Api
    /// </summary>
    public class ClientInfo
    {
        BlockingCollection<string> _queue;

        public ClientSystem System { get; }

        public MsgInfo Take()
        {

        }
        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}
