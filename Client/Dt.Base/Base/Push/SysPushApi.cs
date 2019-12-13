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
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 系统内置推送处理
    /// </summary>
    [PushApi]
    public class SysPushApi
    {
        /// <summary>
        /// 该账户从其它位置登录时停止接收推送
        /// </summary>
        public void StopPush()
        {
            PushHandler.StopPush = true;
            AtKit.Msg("您已从其它位置登录！");
        }


    }
}
