#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
using MySqlConnector;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 默认数据库的表结构
    /// </summary>
    public static class DbSchema
    {
        /// <summary>
        /// 默认库的所有表结构，键名为小写表名
        /// </summary>
        public static IReadOnlyDictionary<string, TableSchema> Schema { get; private set; }

        /// <summary>
        /// 加载默认库的所有表结构，初次Mysql连接
        /// </summary>
        internal static void Init()
        {
            try
            {
                Schema = Kit.NewDataAccess().GetDbSchema();
                Log.Information("缓存表结构、同步时间成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "缓存表结构、同步时间失败！");
                throw;
            }
        }

        /// <summary>
        /// 刷新表结构信息
        /// </summary>
        public static void RefreshSchema()
        {
            Schema = Kit.NewDataAccess().GetDbSchema();
        }

        /// <summary>
        /// 获取表结构信息
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static TableSchema GetTableSchema(string p_tblName)
        {
            TableSchema schema;
            if (Schema.TryGetValue(p_tblName.ToLower(), out schema))
                return schema;
            throw new Exception($"未找到表{p_tblName}的结构信息！");
        }

        /// <summary>
        /// 关闭MySql连接池，释放资源
        /// </summary>
        internal static void Close()
        {
            try
            {
                MySqlConnection.ClearAllPools();
            }
            catch { }
        }
    }
}
