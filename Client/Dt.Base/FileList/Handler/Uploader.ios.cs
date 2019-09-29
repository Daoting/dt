#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// iOS版文件上传
    /// </summary>
    public static class Uploader
    {
        static readonly AsyncLocker _locker = new AsyncLocker();
        readonly static HttpClient _client = new HttpClient(new HttpClientHandler
        {
            // 验证时服务端证书始终有效！
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });

        public static async Task<List<string>> Handle(List<IUploadFile> p_uploadFiles, CancellationToken p_token)
        {
            return null;
        }

        /// <summary>
        /// 获取当前是否已锁定文件传输
        /// </summary>
        public static bool IsLocked
        {
            get { return _locker.IsLocked; }
        }
    }
}
#endif