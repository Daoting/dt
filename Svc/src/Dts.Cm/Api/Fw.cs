#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Dts.Cm.Sqlite;
using Dts.Core;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static Dts.Core.Db;
#endregion

namespace Dts.Cm
{
    /// <summary>
    /// 基础框架Api (Framework)
    /// </summary>
    [Api]
    public class Fw : BaseApi
    {
        /// <summary>
        /// 获取参数配置，包括模型文件版本号
        /// </summary>
        /// <returns></returns>
        public Dict GetConfig()
        {
            return new Dict { { "ver", SqliteModel.Version }, { "now", Glb.Now } };
        }

        public async Task<Dict> GetUserInfo()
        {
            long id = _c.GetUserID();
            Dict res = new Dict();
            if (id < 0)
            {
                res["valid"] = false;
                res["error"] = "无认证信息！";
                return res;
            }

            res["valid"] = true;
            User user = await Users.GetUser(id);
            if (user != null)
            {
                res["name"] = user.Name;
                res["sex"] = user.Sex;
            }
            res["roles"] = Glb.AnyoneID;
            return res;
        }

        [Authorize]
        public string Test()
        {
            return "asd";
        }
    }
}
