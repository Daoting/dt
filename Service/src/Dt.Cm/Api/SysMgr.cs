#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
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
using static Dt.Core.MySqlAccess;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 系统管理Api
    /// </summary>
    [Api(GroupName = "系统工具")]
    public class SysMgr : BaseApi
    {
        /// <summary>
        /// 刷新模型版本
        /// </summary>
        /// <returns></returns>
        public bool RefreshModel()
        {
            return Glb.GetSvc<SqliteModelHandler>().Refresh();
        }
    }
}
