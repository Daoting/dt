#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
using Serilog.Events;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Sqlite库本地数据访问
    /// </summary>
    class SqliteAccess : IDataAccess
    {
        #region 成员变量
        static ILogger _log;
        readonly string _dbName;
        readonly SqliteConnectionEx _db;
        #endregion

        #region 构造方法
        static SqliteAccess()
        {
            if (Log.IsEnabled(LogEventLevel.Debug))
                _log = Log.ForContext("SourceContext", "Sqlite");
        }

        public SqliteAccess(string p_dbName)
        {
            _dbName = p_dbName;
            _db = OpenDb();
        }
        #endregion

        #region 查询
        public Task<Table> Query(string p_sql, object p_params = null)
        {
            if (_log != null)
                Trace("Query", p_sql, p_params);
            return _db.Query(p_sql, p_params);
        }

        public Task<Table<TEntity>> Query<TEntity>(string p_sql, object p_params = null)
            where TEntity : Entity
        {
            if (_log != null)
                Trace("Query", p_sql, p_params);
            return _db.Query<TEntity>(p_sql, p_params);
        }

        public Task<Table> Page(int p_starRow, int p_pageSize, string p_sql, object p_params = null)
        {
            string sql = $"select * from ({p_sql}) a limit {p_starRow},{p_pageSize} ";
            if (_log != null)
                Trace("Page", sql);
            return Query(sql, p_params);
        }

        public Task<Table<TEntity>> Page<TEntity>(int p_starRow, int p_pageSize, string p_sql, object p_params = null)
            where TEntity : Entity
        {
            string sql = $"select * from ({p_sql}) a limit {p_starRow},{p_pageSize} ";
            if (_log != null)
                Trace("Page", sql);
            return Query<TEntity>(sql, p_params);
        }

        public async Task<Row> First(string p_sql, object p_params = null)
        {
            if (_log != null)
                Trace("First", p_sql, p_params);
            return (await _db.ForEach<Row>(p_sql, p_params)).FirstOrDefault();
        }

        public async Task<TEntity> First<TEntity>(string p_sql, object p_params = null)
            where TEntity : Entity
        {
            if (_log != null)
                Trace("First", p_sql, p_params);
            return (await _db.ForEach<TEntity>(p_sql, p_params)).FirstOrDefault();
        }

        public Task<List<T>> FirstCol<T>(string p_sql, object p_params = null)
        {
            if (_log != null)
                Trace("FirstCol", p_sql, p_params);
            return _db.GetFirstCol<T>(p_sql, p_params);
        }

        public Task<T> GetScalar<T>(string p_sql, object p_params = null)
        {
            if (_log != null)
                Trace("GetScalar", p_sql, p_params);
            return _db.GetScalar<T>(p_sql, p_params);
        }

        public Task<IEnumerable<Row>> Each(string p_sql, object p_params = null)
        {
            if (_log != null)
                Trace("Each", p_sql, p_params);
            return _db.ForEach<Row>(p_sql, p_params);
        }

        public Task<IEnumerable<TEntity>> Each<TEntity>(string p_sql, object p_params = null)
            where TEntity : Entity
        {
            if (_log != null)
                Trace("Each", p_sql, p_params);
            return _db.ForEach<TEntity>(p_sql, p_params);
        }

        public Task<IEnumerable<T>> EachFirstCol<T>(string p_sql, object p_params = null)
        {
            if (_log != null)
                Trace("EachFirstCol", p_sql, p_params);
            return _db.EachFirstCol<T>(p_sql, p_params);
        }
        #endregion

        #region 增删改
        public Task<int> Exec(string p_sql, object p_params = null)
        {
            if (_log != null)
                Trace("Exec", p_sql, p_params);
            return Task.FromResult(_db.Execute(p_sql, p_params));
        }

        public Task<int> BatchExec(List<Dict> p_dts)
        {
            if (_log != null)
                Trace("BatchExec", p_dts);
            return Task.FromResult(_db.BatchExec(p_dts));
        }
        #endregion

        #region 库管理
        /// <summary>
        /// 打开Sqlite库，自动创建、同步库表结构
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        SqliteConnectionEx OpenDb()
        {
            SqliteConnectionEx db;
            try
            {
                var path = Path.Combine(Kit.DataPath, _dbName + ".db");
                bool exists = File.Exists(path);
                db = new SqliteConnectionEx("Data Source=" + path);
                db.Open();
                var msg = $"打开 {_dbName} 库";

                // 后台任务独立启动时为null
                if (Stub.Inst != null
                    && Stub.Inst.GetSqliteDbInfo(_dbName) is SqliteTblsInfo dbInfo
                    && dbInfo != null)
                {
                    // 初次运行、库表结构版本变化或文件被删除时创建库表结构
                    path = Path.Combine(Kit.DataPath, $"{_dbName}-{dbInfo.Version}.ver");
                    if (!exists || !File.Exists(path))
                    {
                        db.InitDb(dbInfo.Tables);

                        // 删除旧版本号文件
                        foreach (var file in new DirectoryInfo(Kit.DataPath).GetFiles($"{_dbName}-*.ver"))
                        {
                            try { file.Delete(); } catch { }
                        }

                        // 创建空文件，文件名是库表结构的版本号
                        File.Create(path);
                        msg += "，初始化库表结构";
                    }
                }

                Log.Information(msg);
            }
            catch (Exception ex)
            {
                throw new Exception($"打开sqlite库[{_dbName}]异常，请重新启动应用！{ex.Message}");
            }
            return db;
        }

        void Trace(string p_method, params object[] p_params)
        {
            var id = TraceLogs.AddDetail(p_params);
            _log.ForContext("Detail", id)
                .Debug($"{_dbName}.{p_method}");
        }
        #endregion
    }
}
