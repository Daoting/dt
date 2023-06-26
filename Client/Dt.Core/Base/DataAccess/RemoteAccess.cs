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
    class RemoteAccess : IDataAccess
    {
        #region 构造方法
        readonly string _svc;

        public RemoteAccess(string p_svc)
        {
            _svc = p_svc;
        }
        #endregion

        #region 查询
        public Task<Table> Query(string p_sqlOrSp, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.Query",
                p_sqlOrSp,
                p_params
            ).Call<Table>();
        }

        public Task<Table<TEntity>> Query<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            return new UnaryRpc(
                _svc,
                "Da.Query",
                p_sqlOrSp,
                p_params
            ).Call<Table<TEntity>>();
        }

        public Task<Table> Page(int p_starRow, int p_pageSize, string p_sql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
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
                _svc,
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
                _svc,
                "Da.First",
                p_sqlOrSp,
                p_params
            ).Call<Row>();
        }

        public Task<TEntity> First<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            return new UnaryRpc(
                _svc,
                "Da.First",
                p_sqlOrSp,
                p_params
            ).Call<TEntity>();
        }

        public Task<List<T>> FirstCol<T>(string p_sqlOrSp, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.FirstCol",
                typeof(T).FullName,
                p_sqlOrSp,
                p_params
            ).Call<List<T>>();
        }

        public Task<T> GetScalar<T>(string p_sqlOrSp, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
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
                _svc,
                "Da.Exec",
                p_sqlOrSp,
                p_params
            ).Call<int>();
        }

        public Task<int> BatchExec(List<Dict> p_dts)
        {
            return new UnaryRpc(
                _svc,
                "Da.BatchExec",
                p_dts
            ).Call<int>();
        }
        #endregion

        #region 缓存
        public Task<string> StringGet(string p_key)
        {
            return Kit.Rpc<string>(
                _svc,
                "CacheAccess.StringGet",
                p_key
            );
        }

        public Task<bool> StringSet(string p_key, string p_value, double p_seconds = 0)
        {
            return Kit.Rpc<bool>(
                _svc,
                "CacheAccess.StringSet",
                p_key,
                p_value,
                p_seconds
            );
        }

        public Task<string> GetEntityJson(string p_key, string p_priKeyPrefix)
        {
            return Kit.Rpc<string>(
                _svc,
                "CacheAccess.GetEntityJson",
                p_key,
                p_priKeyPrefix
            );
        }

        public Task<Dict> HashGetAll(string p_key)
        {
            return Kit.Rpc<Dict>(
                _svc,
                "CacheAccess.HashGetAll",
                p_key
            );
        }

        public Task<string> HashGet(string p_key, string p_field)
        {
            return Kit.Rpc<string>(
                _svc,
                "CacheAccess.HashGet",
                p_key,
                p_field
            );
        }

        public Task<bool> KeyDelete(string p_key)
        {
            return Kit.Rpc<bool>(
                _svc,
                "CacheAccess.KeyDelete",
                p_key
            );
        }

        public Task BatchKeyDelete(List<string> p_keys)
        {
            return Kit.Rpc<object>(
                _svc,
                "CacheAccess.BatchKeyDelete",
                p_keys
            );
        }
        #endregion
    }
}
