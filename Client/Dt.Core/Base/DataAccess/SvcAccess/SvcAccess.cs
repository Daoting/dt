#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端数据访问提供者，服务端DataAccess的代理
    /// </summary>
    class SvcAccess : IDataAccess
    {
        #region 构造方法
        readonly SvcAccessInfo _ai;

        public SvcAccess(SvcAccessInfo p_ai)
        {
            _ai = p_ai;
        }
        #endregion

        #region 查询
        public Task<Table> Query(string p_sqlOrSp, object p_params = null)
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.Query",
                p_sqlOrSp,
                p_params
            ).Call<Table>();
        }

        public Task<Table<TEntity>> Query<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.Query",
                p_sqlOrSp,
                p_params
            ).Call<Table<TEntity>>();
        }

        public Task<Table> Page(int p_starRow, int p_pageSize, string p_sql, object p_params = null)
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.Page",
                p_starRow,
                p_pageSize,
                p_sql,
                p_params
            ).Call<Table>();
        }

        public Task<Table<TEntity>> Page<TEntity>(int p_starRow, int p_pageSize, string p_sql, object p_params = null)
            where TEntity : Entity
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.Page",
                p_starRow,
                p_pageSize,
                p_sql,
                p_params
            ).Call<Table<TEntity>>();
        }

        public Task<Row> First(string p_sqlOrSp, object p_params = null)
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.First",
                p_sqlOrSp,
                p_params
            ).Call<Row>();
        }

        public Task<TEntity> First<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.First",
                p_sqlOrSp,
                p_params
            ).Call<TEntity>();
        }

        public Task<List<T>> FirstCol<T>(string p_sqlOrSp, object p_params = null)
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.FirstCol",
                typeof(T).FullName,
                p_sqlOrSp,
                p_params
            ).Call<List<T>>();
        }

        public Task<object> FirstCol(Type p_type, string p_sqlOrSp, object p_params = null)
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.FirstCol",
                p_type.FullName,
                p_sqlOrSp,
                p_params
            ).Call<object>();
        }

        public Task<T> GetScalar<T>(string p_sqlOrSp, object p_params = null)
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.GetScalar",
                p_sqlOrSp,
                p_params
            ).Call<T>();
        }

        public Task<IEnumerable<Row>> Each(string p_sqlOrSp, object p_params = null)
        {
            throw new NotSupportedException();
        }

        public Task<IEnumerable<TEntity>> Each<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            throw new NotSupportedException();
        }

        public Task<IEnumerable<T>> EachFirstCol<T>(string p_sqlOrSp, object p_params = null)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region 增删改
        public Task<int> Exec(string p_sqlOrSp, object p_params = null)
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.Exec",
                p_sqlOrSp,
                p_params
            ).Call<int>();
        }

        public Task<int> BatchExec(List<Dict> p_dts)
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.BatchExec",
                p_dts
            ).Call<int>();
        }
        #endregion

        #region 新ID和序列
        public Task<long> NewID()
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.NewID"
            ).Call<long>();
        }

        public Task<int> NewSeq(string p_seqName)
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.NewSeq",
                p_seqName
            ).Call<int>();
        }
        #endregion

        #region 缓存
        public Task<string> StringGet(string p_key)
        {
            return Kit.Rpc<string>(
                _ai.Name,
                "CacheAccess.StringGet",
                p_key
            );
        }

        public Task<bool> StringSet(string p_key, string p_value, double p_seconds = 0)
        {
            return Kit.Rpc<bool>(
                _ai.Name,
                "CacheAccess.StringSet",
                p_key,
                p_value,
                p_seconds
            );
        }

        public Task<string> GetEntityJson(string p_key, string p_priKeyPrefix)
        {
            return Kit.Rpc<string>(
                _ai.Name,
                "CacheAccess.GetEntityJson",
                p_key,
                p_priKeyPrefix
            );
        }

        public Task<Dict> HashGetAll(string p_key)
        {
            return Kit.Rpc<Dict>(
                _ai.Name,
                "CacheAccess.HashGetAll",
                p_key
            );
        }

        public Task<string> HashGet(string p_key, string p_field)
        {
            return Kit.Rpc<string>(
                _ai.Name,
                "CacheAccess.HashGet",
                p_key,
                p_field
            );
        }

        public Task<bool> KeyDelete(string p_key)
        {
            return Kit.Rpc<bool>(
                _ai.Name,
                "CacheAccess.KeyDelete",
                p_key
            );
        }

        public Task BatchKeyDelete(List<string> p_keys)
        {
            return Kit.Rpc<object>(
                _ai.Name,
                "CacheAccess.BatchKeyDelete",
                p_keys
            );
        }
        #endregion

        #region 库信息
        public async Task<DatabaseType> GetDbType()
        {
            if (!_dbType.HasValue)
            {
                _dbType = await new UnaryRpc(
                    _ai.Name,
                    "Da.GetDbType"
                ).Call<DatabaseType>();
            }
            return _dbType.Value;
        }

        public Task<string> GetDbKey()
        {
            return new UnaryRpc(
                _ai.Name,
                "Da.GetDbKey"
            ).Call<string>();
        }

#if !SERVER
        public IAccessInfo AccessInfo => _ai;
#endif
        DatabaseType? _dbType;
        #endregion

        #region 实体写入器
        public IEntityWriter NewWriter() => new EntityWriter(this);
        #endregion

        #region 表结构
        public Task<IReadOnlyDictionary<string, TableSchema>> GetDbSchema()
        {
            throw new NotSupportedException();
        }

        public Task<List<string>> GetAllTableNames()
        {
            throw new NotSupportedException();
        }

        public Task<TableSchema> GetTableSchema(string p_tblName)
        {
            throw new NotSupportedException();
        }

        public Task SyncDbTime()
        {
            throw new NotSupportedException();
        }
        #endregion

        #region 其他
        public Task Close(bool p_commitTrans)
        {
            return Task.CompletedTask;
        }
        #endregion
    }
}
