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
        public Task<string> IsExists(List<string> p_list)
        {
            return GetTools(p_list).IsExists();
        }

        public Task<bool> DoInit(List<string> p_list, int p_initType)
        {
            return GetTools(p_list).InitDb(p_initType);
        }

        IDbTools GetTools(List<string> p_list)
        {
            if (p_list == null || p_list.Count != 7)
                Throw.Msg("参数个数应为7个！");

            if (p_list[0] == "0")
                return new MySqlTools(p_list);
            if (p_list[0] == "1")
                return new OracleTools(p_list);
            if (p_list[0] == "2")
                return new SqlServerTools(p_list);
            if (p_list[0] == "3")
                return new PostgreSqlTools(p_list);

            throw new Exception("不支持该数据库类型！");
        }
    }
}