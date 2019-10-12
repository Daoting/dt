#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 客户端发送请求数据流，服务端返回数据流的处理类
    /// </summary>
    public class DuplexStreamHandler : RpcHandler
    {
        public DuplexStreamHandler(LobContext p_c)
            : base(p_c)
        { }

        /// <summary>
        /// 调用服务方法
        /// </summary>
        /// <returns></returns>
        protected override async Task<bool> CallMethod()
        {
            try
            {
                // 补充参数
                List<object> objs = new List<object>();
                if (_c.Args != null && _c.Args.Length > 0)
                    objs.AddRange(_c.Args);
                objs.Add(new RequestReader(_c));
                objs.Add(new ResponseWriter(_c));

                await (Task)_c.Api.Method.Invoke(_tgt, objs.ToArray());
            }
            catch (Exception ex)
            {
                LogCallError(ex);
                return false;
            }
            return true;
        }
    }
}