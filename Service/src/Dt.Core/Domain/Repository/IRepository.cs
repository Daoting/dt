#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 标志一个类型为仓库类
    /// </summary>
    public interface IRepository
    { }

    /// <summary>
    /// 包含"ID"主键的实体类仓库接口
    /// </summary>
    /// <typeparam name="TEntity">聚合根类型</typeparam>
    /// <typeparam name="TKey">聚合根主键类型</typeparam>
    public interface IRepository<TEntity, TKey> : IRepository
        where TEntity : class, IRoot<TKey>
    { }
}
