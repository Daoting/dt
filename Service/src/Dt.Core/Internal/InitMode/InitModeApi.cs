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
                suc = await PostgreSqlTools.TestConnect(p_list);
            }
            else if(p_list[0] == "1")
            {
                suc = await MySqlTools.TestConnect(p_list);
            }
            else if (p_list[0] == "2")
            {
                suc = await OracleTools.TestConnect(p_list);
            }
            else if (p_list[0] == "3")
            {
                suc = await SqlServerTools.TestConnect(p_list);
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

        public async Task<string> IsExists(List<string> p_list, int p_isNewDb)
        {
            var db = GetTools(p_list);
            bool existsDb = await db.ExistsDb();
            bool existsUser = await db.ExistsUser();

            string msg = null;
            if (p_isNewDb == 1)
            {
                if (existsDb)
                {
                    msg = "数据库";
                }
                if (existsUser)
                {
                    if (msg == null)
                        msg = "用户";
                    else
                        msg += "、用户";
                }

                if (msg != null)
                {
                    msg += "已存在";
                    Log.Information(msg);
                    msg += "，\r\n点击【确定】将删除重建！\r\n需要【确定】多次避免误操作！";
                }
                else
                {
                    Log.Information("数据库、用户都不存在");
                }
            }
            else
            {
                if (!existsDb)
                {
                    msg = "数据库";
                }
                if (!existsUser)
                {
                    if (msg == null)
                        msg = "用户";
                    else
                        msg += "、用户";
                }

                if (msg != null)
                {
                    msg += "不存在！";
                    Log.Information(msg);
                }
                else if (!await db.IsPwdCorrect())
                {
                    msg = "密码不正确！";
                    Log.Information(msg);
                }
                else
                {
                    Log.Information("数据库连接成功");
                }
            }
            return msg;
        }

        public Task<bool> DoInit(List<string> p_list, int p_initType, int p_isNewDb)
        {
            var db = GetTools(p_list);
            return p_isNewDb == 1 ? db.InitDb(p_initType) : db.ImportToDb(p_initType);
        }

        IDbTools GetTools(List<string> p_list)
        {
            if (p_list == null || p_list.Count != 9)
                Throw.Msg("参数个数应为9个！");

            if (p_list[0] == "0")
                return new PostgreSqlTools(p_list);
            if (p_list[0] == "1")
                return new MySqlTools(p_list);
            if (p_list[0] == "2")
                return new OracleTools(p_list);
            if (p_list[0] == "3")
                return new SqlServerTools(p_list);

            throw new Exception("不支持该数据库类型！");
        }
    }
}