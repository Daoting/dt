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
    class RemoteAccess : IEntityAccess
    {
        #region 构造方法
        readonly string _svc;

        public RemoteAccess(string p_svc)
        {
            _svc = p_svc;
        }
        #endregion

        #region 查询
        public Task<Table> Query(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.Query",
                p_keyOrSql,
                p_params
            ).Call<Table>();
        }

        public Task<Table<TEntity>> Query<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return new UnaryRpc(
                _svc,
                "Da.Query",
                p_keyOrSql,
                p_params
            ).Call<Table<TEntity>>();
        }

        public Task<Table> Page(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.Page",
                p_starRow,
                p_pageSize,
                p_keyOrSql,
                p_params
            ).Call<Table>();
        }

        public Task<Table<TEntity>> Page<TEntity>(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return new UnaryRpc(
                _svc,
                "Da.Page",
                p_starRow,
                p_pageSize,
                p_keyOrSql,
                p_params
            ).Call<Table<TEntity>>();
        }

        public Task<Row> First(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.First",
                p_keyOrSql,
                p_params
            ).Call<Row>();
        }

        public Task<TEntity> First<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return new UnaryRpc(
                _svc,
                "Da.First",
                p_keyOrSql,
                p_params
            ).Call<TEntity>();
        }

        public Task<List<T>> FirstCol<T>(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.FirstCol",
                typeof(T).FullName,
                p_keyOrSql,
                p_params
            ).Call<List<T>>();
        }

        public Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.GetScalar",
                p_keyOrSql,
                p_params
            ).Call<T>();
        }

        public Task<IEnumerable<Row>> Each(string p_keyOrSql, object p_params = null)
        {
            throw new NotSupportedException();
        }

        public Task<IEnumerable<TEntity>> Each<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            throw new NotSupportedException();
        }

        public Task<IEnumerable<T>> EachFirstCol<T>(string p_keyOrSql, object p_params = null)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region 增删改
        public Task<int> Exec(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.Exec",
                p_keyOrSql,
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
    }
}
