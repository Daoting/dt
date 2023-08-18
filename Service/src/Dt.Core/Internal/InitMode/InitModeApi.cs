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
        public async Task<string> TestConnect(List<string> p_list)
        {
            if (p_list == null || p_list.Count != 6)
                return "参数个数应为6个！";

            Log.Information("正在连接...");
            Kit.TraceSql = false;
            bool suc;
            if (p_list[0] == "0")
            {
                suc = await MySqlTools.TestConnect(p_list);
            }
            else if (p_list[0] == "1")
            {
                suc = await OracleTools.TestConnect(p_list);
            }
            else if (p_list[0] == "2")
            {
                suc = await SqlServerTools.TestConnect(p_list);
            }
            else if (p_list[0] == "3")
            {
                suc = await PostgreSqlTools.TestConnect(p_list);
            }
            else
            {
                return "不支持该数据库类型！";
            }

            if (suc)
            {
                Log.Information("数据库连接成功！");
                return null;
            }

            Log.Error("数据库连接失败，请检查输入！");
            return "数据库连接失败，请检查输入！";
        }

        public async Task<string> IsExists(List<string> p_list)
        {
            Log.Information("判断新库或新用户名已存在...");
            var msg = await GetTools(p_list).IsExists();
            if (!string.IsNullOrEmpty(msg))
            {
                Log.Warning(msg);
            }
            return msg;
        }

        public Task<bool> DoInit(List<string> p_list, int p_initType)
        {
            return GetTools(p_list).InitDb(p_initType);
        }

        IDbTools GetTools(List<string> p_list)
        {
            if (p_list == null || p_list.Count != 9)
                Throw.Msg("参数个数应为9个！");

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