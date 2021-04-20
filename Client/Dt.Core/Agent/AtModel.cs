#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Model;
using Dt.Core.Rpc;
using Dt.Core.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 本地sqlite模型库
    /// </summary>
    public class AtModel : SqliteProvider<Sqlite_model>
    {
        /// <summary>
        /// 查询指定表的所有列
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static IEnumerable<OmColumn> EachColumns(string p_tblName)
        {
            return _db.ForEach<OmColumn>($"select * from OmColumn where tabname='{p_tblName}'");
        }

    }

    public class Sqlite_model
    { }
}
