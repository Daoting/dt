#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
using MySqlConnector;
using System.Collections.Concurrent;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 默认数据库的表结构
    /// </summary>
    public static class DbSchema
    {
        /// <summary>
        /// 默认库的所有表结构，键名为表名，库里原始大小写，比较时大小写不敏感
        /// </summary>
        static ConcurrentDictionary<string, TableSchema> _schema { get; } = new ConcurrentDictionary<string, TableSchema>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 同步数据库时间
        /// </summary>
        internal static void SyncDbTime()
        {
            try
            {
                bool trace = Kit.TraceSql;
                Kit.TraceSql = false;

                var task = Kit.NewDataAccess().SyncDbTime();
                task.Wait();
                
                Kit.TraceSql = trace;
                Log.Information("同步数据库时间成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "同步数据库时间失败！");
                throw;
            }
        }

        /// <summary>
        /// 获取表结构信息
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static async Task<TableSchema> GetTableSchema(string p_tblName)
        {
            TableSchema schema;
            if (_schema.TryGetValue(p_tblName, out schema))
                return schema;

            schema = await Kit.NewDataAccess().GetTableSchema(p_tblName);
            if (schema != null)
            {
                _schema.TryAdd(p_tblName, schema);
                return schema;
            }
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
