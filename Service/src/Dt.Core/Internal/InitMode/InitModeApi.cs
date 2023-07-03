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
        public Task<bool> ExistsDb(List<string> p_list)
        {
            return GetTools(p_list).ExistsDb();
        }

        public Task<bool> ExistsUser(List<string> p_list)
        {
            return GetTools(p_list).ExistsUser();
        }

        public Task<bool> DoInit(List<string> p_list)
        {
            return GetTools(p_list).InitDb();
        }

        IDbTools GetTools(List<string> p_list)
        {
            if (p_list == null || p_list.Count != 7)
                Throw.Msg("参数个数应为7个！");

            if (p_list[0] == "0")
                return new MySqlTools(p_list);
            return null;
        }
    }
}