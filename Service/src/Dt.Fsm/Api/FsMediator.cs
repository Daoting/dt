#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Fsm
{
    /// <summary>
    /// 中介服务Api，在上传/下载前确定使用哪个服务
    /// </summary>
    [Api]
    public class FsMediator : BaseApi
    {
        public async Task<string> GetUploadSvc()
        {
            return null;
        }

        public async Task<string> GetDownloadSvc(string p_fileID)
        {
            return null;
        }
    }
}
