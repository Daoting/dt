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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 文件上传信息
    /// </summary>
    public interface IUploadFile
    {
        /// <summary>
        /// 获取设置待上传的文件
        /// </summary>
        StorageFile File { get; }

        /// <summary>
        /// 上传进度，可以为null
        /// </summary>
        ProgressDelegate UploadProgress { get; }
    }
}