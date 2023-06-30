#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
#endregion

namespace Dt.Core.Rpc
{
    class InitModeApi
    {
        public bool ExistsDb(List<string> p_list)
        {
            return true;
        }

        public bool ExistsUser(List<string> p_list)
        {
            Log.Warning("用户名已存在！");
            return true;
        }

        public bool DoInit(List<string> p_list)
        {
            Log.Information("初始化成功");
            return true;
        }
    }
}