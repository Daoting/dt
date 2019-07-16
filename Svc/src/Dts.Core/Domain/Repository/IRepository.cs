#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dts.Core
{

    public interface IRepository<TEntity>
        where TEntity : class, IEntity
    {

    }

    public interface IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
    }
}
